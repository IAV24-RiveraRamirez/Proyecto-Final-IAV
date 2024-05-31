using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    //Para que funcione bien, el ancho y el alto tienen que ser potencias de 2
    [SerializeField, Range(2, 1024)]
    int dimensions = 256;
    //Altura del terreno
    [SerializeField, Range(0, 100)]
    int depth = 20;
    [SerializeField, Range(1, 50)]
    int numOctaves = 15;
    [SerializeField, Min(20)]
    float scale = 10f;
    [SerializeField, Min(1)]
    int housesToSpawn = 6;
    [SerializeField]
    bool spawnWater = true;
    [SerializeField]
    bool randomizeOffset = true;
    [SerializeField]
    GameObject water = null;
    [SerializeField]
    public List<GameObject> residentialBuildings;
    [SerializeField]
    public List<GameObject> workBuildings;
    [SerializeField]
    public List<GameObject> leisureBuildings;
    [SerializeField]
    GameObject market = null;
    [SerializeField]
    GameObject woodShop = null;
    [SerializeField]
    GameObject sawMill = null;
    [SerializeField]
    TerrainRegions[] regionsParameters;
    [SerializeField]
    GameObject[] environmentAssets;

    //Offset del perlin, se usa principalmente para aleatorizar más el resultado
    float offsetX;
    float offsetY;
    int houseSpace = 6; //El tamaño de terreno que aplana una casa 
    int numOfHabs = 0; //El número de habitantes de la aldea
    int workCapacity = 0; //La capacidad de las zonas de trabajo de la aldea
    int leisureCapacity = 0; //La capacidad de las zonas de ocio de la aldea

    bool startCameraMovement = false;

    float[,] heights;
    Terrain terrain;
    PerlinNoiseGenerator perlinGenerator;
    StructuresGenerator structuresGenerator;
    Vector3 buildingPosition;
    GameObject cameraRef;
    Vector3 cameraPosition;
    SettingsManager settingsManager;
    GameObject expectedNextBuilding = null;
    List<GameObject> woodShopDependencies;
    bool firstWorkPlace = true;
    bool expectedLeisure = false;

    [System.Serializable]
    public struct TerrainRegions
    {
        public string Name;
        public float Height;
        public Color color;
    }

    void setValues() //Ajusta sus valores a los que tenga el manager 
    {
        depth = settingsManager.terrainDepth;
        numOctaves = settingsManager.nOctaves;
        scale = settingsManager.perlinScale;
        housesToSpawn = settingsManager.nHouses;
        spawnWater = settingsManager.toogleWater;
        randomizeOffset = settingsManager.randomOffsets;
    }
    int buildingComparer(GameObject b1, GameObject b2) //Función que ordena los edificios de más a menos capacidad
    {
        return b2.GetComponentInChildren<NPCBuilding>().GetMaxNPCs().CompareTo(b1.GetComponentInChildren<NPCBuilding>().GetMaxNPCs());
    }

    void Start()
    {
        GameObject SM = GameObject.Find("SettingsManager");
        if(SM) settingsManager = SM.GetComponent<SettingsManager>();
        if (settingsManager) setValues();

        residentialBuildings.Sort(buildingComparer);
        workBuildings.Sort(buildingComparer);
        leisureBuildings.Sort(buildingComparer);
        woodShopDependencies = new List<GameObject>();

        if (randomizeOffset)
        {
            //(Opcional) Aleatoriza la generación
            offsetX = UnityEngine.Random.Range(0f, 10000f);
            offsetY = UnityEngine.Random.Range(0f, 10000f);
        }
        perlinGenerator = gameObject.AddComponent<PerlinNoiseGenerator>();
        perlinGenerator.setValues(numOctaves, scale, offsetX, offsetY);

        terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);

        structuresGenerator = gameObject.GetComponent<StructuresGenerator>();
        structuresGenerator.setValues(dimensions, houseSpace);

        StartCoroutine(StartGeneration());

        cameraRef = Camera.main.gameObject;//Se puede hacer mejor
        if(cameraRef) cameraPosition = cameraRef.transform.position;
    }

    private void Update()
    {
        if(cameraRef && startCameraMovement)
        {
            cameraRef.transform.position = Vector3.Lerp(cameraRef.transform.position, cameraPosition, Time.deltaTime); //Movimiento de la cámara para apuntar hacia la aldea
        }
    }

    IEnumerator StartGeneration()
    {
        yield return new WaitForNextFrameUnit(); //Esperamos porque si no los raycast no detectan el agua
        if (structuresGenerator.findStartSpot(ref buildingPosition, heights))
        {
            List<Vector3> places = new List<Vector3>();
            Vector3 originPoint = buildingPosition;
            places.Add(originPoint);
            PlaceBuilding();
            float maxVillageSize = 10;
            cameraPosition = originPoint + new Vector3(0,80,-50);
            Vector3 center = originPoint;

            Stack<Vector3> positions = new Stack<Vector3>();
            positions.Push(originPoint);

            while (places.Count < housesToSpawn - 1 && positions.Count > 0)
            {
                buildingPosition = positions.Peek();
                if (structuresGenerator.findNearSpots(ref buildingPosition, originPoint))
                {
                    PlaceBuilding();
                    center += buildingPosition;
                    places.Add(buildingPosition);
                    positions.Push(buildingPosition);
                }
                else {
                    positions.Pop();
                }
                yield return new WaitForNextFrameUnit();
            }

            if(expectedNextBuilding != null)
            {
                expectedNextBuilding = null;
                foreach(GameObject b in woodShopDependencies)
                {
                    Vector3 pos = b.transform.position;
                    SimulationManager.Instance.RemoveBuilding(b.GetComponentInChildren<NPCBuilding>());
                    Destroy(b);
                    if(b == market) GameObject.Instantiate(leisureBuildings[0], pos, Quaternion.identity);
                    else GameObject.Instantiate(leisureBuildings[0], pos, Quaternion.identity);
                }
                woodShopDependencies.Clear();
            }
            center = center / places.Count; //Aproximación del centro de la aldea
            foreach (Vector3 v in places)
            {
                maxVillageSize = Mathf.Max(maxVillageSize, Vector3.Distance(center, v));
            }
            maxVillageSize *= 2f; //Lo agrandamos para asegurar que llega
            CreateNavMesh(maxVillageSize, center);

            terrain.terrainData.terrainLayers[0].tileSize = new Vector2(dimensions, dimensions);
            if (!spawnWater && regionsParameters.Length >= 2) regionsParameters[0] = regionsParameters[1];
            terrain.terrainData.terrainLayers[0].diffuseTexture = CreateDynamicTerrainTexture(); //Creación de textura adaptada al terreno

            structuresGenerator.spawnEnvironmentAssets(ref environmentAssets, dimensions, scale);

            yield return new WaitForSeconds(1);
            Debug.Log(numOfHabs + " " + workCapacity + " " + leisureCapacity);
            startCameraMovement = true;
            SimulationManager.Instance.SpawnNPCs();
        }
    }

    void PlaceBuilding() //Allana el terreno donde está el edificio colocado
    {
        ChooseHouse();
        Vector3 initialPos = new Vector3(buildingPosition.z - (houseSpace / 2), buildingPosition.y, buildingPosition.x - (houseSpace / 2));
        float initialHeight = heights[(int)buildingPosition.z, (int)buildingPosition.x];
        for (int x = 0; x <= houseSpace; ++x)
        {
            for (int y = 0; y <= houseSpace; y++)
            {
                float distanceToCenter = Vector2.Distance(new Vector2(x, y), new Vector2(houseSpace / 2, houseSpace / 2));
                if (distanceToCenter > 2.5) //Distancia estándar
                {
                    float t = Mathf.InverseLerp(houseSpace / 2, 0, distanceToCenter); //Interpolamos alturas
                    float interpolatedHeight = Mathf.Lerp(heights[(int)initialPos.x + x, (int)initialPos.z + y], initialHeight, t);
                    heights[(int)initialPos.x + x, (int)initialPos.z + y] = interpolatedHeight;
                }
                else //Si es un vértice muy cercano al punto de generación, se aplana directamente
                {
                    heights[(int)initialPos.x + x, (int)initialPos.z + y] = initialHeight;
                }
            }
        }
        terrain.terrainData.SetHeights(0, 0, heights);
    }
    void SpawnResidential()
    {
        GameObject buildingGO = null;
        if (numOfHabs == 0)
        {
            buildingGO = GameObject.Instantiate(residentialBuildings[0], buildingPosition, Quaternion.identity);
            (buildingGO.GetComponentInChildren<NPCBuilding>() as House).SetNPCsToSpawn(2);
            numOfHabs += 2;
        }
        else
        {
            foreach (GameObject b in residentialBuildings)
            {
                if (b.GetComponentInChildren<NPCBuilding>().GetMaxNPCs() + numOfHabs <= workCapacity || numOfHabs == 0)
                {
                    buildingGO = GameObject.Instantiate(b, buildingPosition, Quaternion.identity);
                    numOfHabs += (buildingGO.GetComponentInChildren<NPCBuilding>() as House).GetNPCsToSpawn();
                    break;
                }
            }
        }
    }

    void SpawnWorkPlace()
    {
        GameObject buildingGO;
        if (expectedNextBuilding == sawMill)
        {
            buildingGO = GameObject.Instantiate(expectedNextBuilding, buildingPosition, Quaternion.identity);
            workCapacity += buildingGO.GetComponentInChildren<NPCBuilding>().GetMaxNPCs();
            woodShopDependencies.Add(buildingGO);
            expectedNextBuilding = market;
        }
        else
        {
            int random = UnityEngine.Random.Range(0, 101);
            if (random > 80 || firstWorkPlace)
            {
                buildingGO = GameObject.Instantiate(woodShop, buildingPosition, Quaternion.identity);
                workCapacity += buildingGO.GetComponentInChildren<NPCBuilding>().GetMaxNPCs();
                woodShopDependencies.Add(buildingGO);
                expectedNextBuilding = sawMill;
            }
            else
            {
                random = UnityEngine.Random.Range(0, workBuildings.Count);
                buildingGO = GameObject.Instantiate(workBuildings[random], buildingPosition, Quaternion.identity);
                workCapacity += buildingGO.GetComponentInChildren<NPCBuilding>().GetMaxNPCs();
            }
            firstWorkPlace = false;
        }
    }
    void SpawnLeisurePlace()
    {
        GameObject buildingGO;
        if (expectedNextBuilding == market)
        {
            buildingGO = GameObject.Instantiate(expectedNextBuilding, buildingPosition, Quaternion.identity);
            leisureCapacity += buildingGO.GetComponentInChildren<NPCBuilding>().GetMaxNPCs();
            woodShopDependencies.Clear();
            expectedLeisure = true;
            expectedNextBuilding = null;
        }
        else
        {
            int random = UnityEngine.Random.Range(0, leisureBuildings.Count);
            buildingGO = GameObject.Instantiate(leisureBuildings[random], buildingPosition, Quaternion.identity);
            leisureCapacity += buildingGO.GetComponentInChildren<NPCBuilding>().GetMaxNPCs();
            expectedLeisure = false;
        }
    }
    void ChooseHouse() //generador de casas en base a la información de la aldea
    {
        if (expectedLeisure)
        {
            SpawnLeisurePlace();
            return;
        }
        if (numOfHabs == 0 || numOfHabs < workCapacity)
        {
            SpawnResidential();
        }
        //if (buildingGO != null) return;
        if (workCapacity < numOfHabs || (residentialBuildings[residentialBuildings.Count - 1].GetComponentInChildren<NPCBuilding>().GetMaxNPCs() + numOfHabs > workCapacity && leisureCapacity * 1.5 >= workCapacity))
        {
            SpawnWorkPlace();
        }
        else
        {
            SpawnLeisurePlace();
        }
    }

    void CreateNavMesh(float maxDimension, Vector3 center) //Crea la malla de navegación de la IA
    {
        NavMeshSurface navMeshSurface = gameObject.GetComponentInChildren<NavMeshSurface>();
        //navMeshSurface.collectObjects = CollectObjects.Volume; //Ajustamos su alcance a un volumen
        navMeshSurface.size = new Vector3(maxDimension, depth, maxDimension);//volumen del tamaño de la aldea
        navMeshSurface.center = center;
        navMeshSurface.BuildNavMesh();
    }
    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = dimensions + 1;
        terrainData.size = new Vector3(dimensions, depth, dimensions);
        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    float[,] GenerateHeights()
    {
        heights = new float[dimensions, dimensions];
        for (int x = 0; x < dimensions; ++x)
        {
            for (int y = 0; y < dimensions; ++y)
            {
                heights[x, y] = perlinGenerator.GetValue(x,y); //Calcula el valor de Perlin en un punto dado
                //heights[x, y] = CalculateHeight(x, y);
            }
        }
        return heights;
    }

    float CalculateHeight(int x, int y) //Método que usa el Perlin de Unity
    {
        float xCoor = (float)x / dimensions * scale + offsetX;
        float yCoor = (float)y / dimensions * scale + offsetY;

        return Mathf.PerlinNoise(xCoor, yCoor);
    }

    Texture2D CreateDynamicTerrainTexture() //Crea una textura dinámica según la altura
    {
        Texture2D texture = new Texture2D(dimensions, dimensions);
        for(int x = 0; x < dimensions; ++x)
        {
            for(int y = 0; y < dimensions; ++y)
            {
                float val = heights[y, x];

                foreach(TerrainRegions r in regionsParameters)
                {
                    if(val <= r.Height)
                    {
                        texture.SetPixel(x, y, r.color);
                        break;
                    } 
                }
            }
        }
        texture.Apply(); //Método necesario para aplicar los cambios a la textura
        return texture;
    }
}
     

