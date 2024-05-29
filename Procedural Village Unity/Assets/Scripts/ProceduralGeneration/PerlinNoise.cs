using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

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
    GameObject residentialBuilding = null;
    [SerializeField]
    GameObject workBuilding = null;
    [SerializeField]
    GameObject leisureBuilding = null;
    //Offset del perlin, se usa principalmente para aleatorizar más el resultado
    float offsetX;
    float offsetY;
    int houseSpace = 6; //El tamaño de terreno que aplana una casa 
    int numOfHabs = 0; //El número de habitantes de la aldea
    int workCapacity = 0; //La capacidad de las zonas de trabajo de la aldea
    int leisureCapacity = 0; //La capacidad de las zonas de ocio de la aldea

    float[,] heights;
    Terrain terrain;
    PerlinNoiseGenerator perlinGenerator;
    StructuresGenerator structuresGenerator;
    Vector3 buildingPosition;
    GameObject cameraRef;
    Vector3 cameraPosition;

    void Start()
    {

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

        if (spawnWater && water != null) //Generación de agua
        {
            Vector3 spawnPosition = new Vector3(gameObject.transform.position.x + (dimensions / 2), gameObject.transform.position.y + 1, gameObject.transform.position.z + (dimensions / 2));
            GameObject waterRef = GameObject.Instantiate(water, spawnPosition, Quaternion.identity);
            waterRef.transform.localScale = new Vector3(dimensions / 2, 1, dimensions / 2);
        }
        structuresGenerator = gameObject.GetComponent<StructuresGenerator>();
        structuresGenerator.setValues(dimensions, houseSpace);

        StartCoroutine(findStartPosition());

        cameraRef = GameObject.Find("Main Camera");//Se puede hacer mejor
        if(cameraRef) cameraPosition = cameraRef.transform.position;
    }

    private void Update()
    {
        if(cameraRef)
        {
            cameraRef.transform.position = Vector3.Lerp(cameraRef.transform.position, cameraPosition, Time.deltaTime); //Movimiento de la cámara para apuntar hacia la aldea
        }
    }

    IEnumerator findStartPosition()
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
            
            for (int i = 0; i < housesToSpawn-1; i++)
            {
                if (structuresGenerator.findNearSpots(ref buildingPosition, originPoint))
                {
                    PlaceBuilding();
                    center += buildingPosition;
                    places.Add(buildingPosition);
                }
                else buildingPosition = originPoint;
                yield return new WaitForNextFrameUnit();
            }
            center = center / places.Count; //Aproximación del centro de la aldea
            foreach (Vector3 v in places)
            {
                maxVillageSize = Mathf.Max(maxVillageSize, Vector3.Distance(center, v));
            }
            maxVillageSize *= 2f; //Lo agrandamos para asegurar que llega
            CreateNavMesh(maxVillageSize, center);
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

    void ChooseHouse() //generador de casas en base a la información de la aldea
    {
        GameObject buildingGO;
        if (numOfHabs == 0 || (numOfHabs < workCapacity && residentialBuilding.GetComponentInChildren<NPCBuilding>().GetMaxNPCs() + numOfHabs <= workCapacity))
        {
            buildingGO = GameObject.Instantiate(residentialBuilding, buildingPosition, Quaternion.identity);
            numOfHabs += buildingGO.GetComponentInChildren<NPCBuilding>().GetMaxNPCs();
        }
        else if (workCapacity < numOfHabs || (residentialBuilding.GetComponentInChildren<NPCBuilding>().GetMaxNPCs() + numOfHabs > workCapacity && leisureCapacity*1.5 >= workCapacity))
        {
            buildingGO = GameObject.Instantiate(workBuilding, buildingPosition, Quaternion.identity);
            workCapacity += buildingGO.GetComponentInChildren<NPCBuilding>().GetMaxNPCs();
        }
        else
        {
            buildingGO = GameObject.Instantiate(leisureBuilding, buildingPosition, Quaternion.identity);
            leisureCapacity += buildingGO.GetComponentInChildren<NPCBuilding>().GetMaxNPCs();
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
}
     

