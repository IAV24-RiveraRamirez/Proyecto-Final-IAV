using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructuresGenerator : MonoBehaviour
{
    [SerializeField,Min(1)]
    int maxNumRayCastTests = 300;
    [SerializeField, Range(0, 180)]
    float maxHouseAngle = 15f;
    [SerializeField,Min(0)]
    float maxHouseHeightDifference = 3f;
    [SerializeField, Min(-1)]
    float maxVillageSeparation = -1f;
    [SerializeField, Min(0)]
    float maxTreeSpawnHeight = 10; //Áltura máxima a la que se pueden generar los árboles
    [SerializeField, Range(0, 180)]
    float maxTreeAngle = 30; 
    [SerializeField, Min(1)]
    float treeGenerationInterval = 2.5f; //Intervalo de generación de árboles, a menor número más comprobaciones para generar árboles
    [SerializeField, Range(0,1)]
    float minTreeValueGenerator = 0.6f; //Valor de perlin que se comprueba para saber si se puede o no generar un árbol

    int terrainDimensions;
    int buildingDimensions;

    private void Start()
    {
        GameObject SM = GameObject.Find("SettingsManager");
        if (SM)
        {
            SettingsManager settingsManager = SM.GetComponent<SettingsManager>();
            if (settingsManager)
            {
                maxHouseHeightDifference = settingsManager.maxHousesHeightDiff;
                maxVillageSeparation = settingsManager.maxVillageSeparation;
                maxTreeSpawnHeight = settingsManager.maxTreeHeightSpawn;
                treeGenerationInterval = settingsManager.treeSpawnInterval;
                minTreeValueGenerator = settingsManager.treeSpawnMinValue;
                maxHouseAngle = settingsManager.maxHouseAngle;
            }
            
        }
    }
    public void setValues(int dimensions, int bDimensions)
    {
        terrainDimensions = dimensions;
        buildingDimensions = bDimensions;
    }
    public bool findStartSpot(ref Vector3 buildingPos, in float [,]heights ) //Busca de manera aleatoria una posici�n para crear las casas
    {
        double bestVarianceFound= double.MaxValue; //Varianza original
        bool found = false;
        int nTests = 0;
        RaycastHit hitResult = new RaycastHit();
        while (nTests < maxNumRayCastTests)
        {
            Vector3 o = new Vector3(Random.Range(0, terrainDimensions - 1), 100, Random.Range(0, terrainDimensions-1));
            Vector3 dir = new Vector3(0, -300, 0);
            bool hasHit = Physics.Raycast(o, dir, out hitResult, 200);
            if (hasHit && hitResult.transform.gameObject.tag == "Terrain" && isTerrainValid(hitResult.normal, maxHouseAngle))
            {
                Vector3 hitPoint = hitResult.point;
                if (!isPointInsideBoundaries(hitPoint)) //Movemos el primer edificio si no guarda unos márgenes mínimos con el borde del terreno
                {
                    hitPoint.x = Mathf.Clamp(hitPoint.x, buildingDimensions + 1, terrainDimensions - (buildingDimensions + 1));
                    hitPoint.z = Mathf.Clamp(hitPoint.z, buildingDimensions + 1, terrainDimensions - (buildingDimensions + 1));
                }
                double calculatedVariance = calculateVariance(heights, hitPoint);
                if (calculatedVariance < bestVarianceFound)
                {
                    bestVarianceFound = calculatedVariance;
                    buildingPos = hitPoint;
                    found = true;
                }
                
            }
            nTests++;
        }
        return found;
    }

    double calculateVariance(in float[,] heights, Vector3 hitPoint)
    {
        double variance = 0;
        int gridHalfSize = 3;
        float sum = 0;
        for(int x = -gridHalfSize; x < gridHalfSize; ++x)
        {
            for(int  y = -gridHalfSize; y < gridHalfSize; ++y)
            {
                sum += heights[(int)(hitPoint.z + x), (int)(hitPoint.x + y)];
            }
        }
        int nVals = ((gridHalfSize * 2) * (gridHalfSize * 2));
        float average = sum/nVals;
        sum = 0;
        for (int x = -gridHalfSize; x < gridHalfSize; ++x)
        {
            for (int y = -gridHalfSize; y < gridHalfSize; ++y)
            {
                float val = heights[(int)(hitPoint.z + x), (int)(hitPoint.x + y)];
                sum += Mathf.Pow((val - average), 2);
            }
        }
        variance = sum / (nVals);
        return variance;
    }

    public bool findNearSpots(ref Vector3 buildingPos, Vector3 startPosition) //Método usado para la creación de casas adyacentes
    {
        float originalHeight = startPosition.y;
        float bestHeightDiff = Mathf.Abs(originalHeight - float.MaxValue); //Busca el edificio con la menor diferencia de altura con respecto al punto de partida
        bool found = false;
        int nTests = 0;
        Vector3 bestPositionFound = buildingPos;
        RaycastHit hitResult = new RaycastHit();
        while (nTests < 4) //Hacemos todos los test aunque encontremos un sitio válido porque queremos el mejor sitio de todos los válidos encontrados
        {
            Vector3 o;
            int direction = nTests;
            Color color = Color.white;
            if(direction <= 0)
            {
                o = new Vector3(buildingPos.x + 7.5f, 100, buildingPos.z);
            }
            else if(direction <= 1)
            {
                o = new Vector3(buildingPos.x - 7.5f, 100, buildingPos.z);
                color = Color.red;
            }
            else if (direction <= 2)
            {
                o = new Vector3(buildingPos.x, 100, buildingPos.z + 10);
                color = Color.green;
            }
            else
            {
                color = Color.blue;
                o = new Vector3(buildingPos.x, 100, buildingPos.z - 10);
            }
            Vector3 dir = new Vector3(0, -200, 0);
            bool hasHit = Physics.Raycast(o, dir, out hitResult, 200);
            Debug.DrawRay(o, dir, color);
            if (hasHit && hitResult.transform.gameObject.tag == "Terrain" && isTerrainValid(hitResult.normal, maxHouseAngle) && isPointInsideBoundaries(hitResult.point) && Mathf.Abs(hitResult.point.y - originalHeight) <= maxHouseHeightDifference && (maxVillageSeparation < 0 || Vector3.Distance(hitResult.point, startPosition) <= maxVillageSeparation))
            {
                if(Mathf.Abs(originalHeight - hitResult.point.y) < bestHeightDiff)
                {
                    bestPositionFound = hitResult.point;
                    found = true;
                }
            }
            nTests++;
        }
        buildingPos = bestPositionFound;
        return found;
    }

    bool isTerrainValid(Vector3 hitNormal, float maxAngle) //Solo genera terreno si el �nuglo que forma la normal de impacto con el vector "up" es menor o igual que un umbral
    {
        return (Mathf.Rad2Deg * (Mathf.Acos(Vector3.Dot(hitNormal, Vector3.up)))) <= maxAngle;
    }

    bool isPointInsideBoundaries(Vector3 point) //Comprobamos si el edificio mantiene una cierta distancia con los bordes
    {
        return (point.x > buildingDimensions && point.z > buildingDimensions && point.x < terrainDimensions - buildingDimensions && point.z < terrainDimensions - buildingDimensions);
    }

    public void spawnEnvironmentAssets(ref GameObject[] GOList, int dimensions, float scale) //Usamos el Perlin de Unity para crear zonas de árboles
    {
        RaycastHit hitResult = new RaycastHit();
        Vector3 o, dir;
        float offSet = Random.Range(0, 10000);
        for (float x = 0; x < dimensions; x+= treeGenerationInterval)
        {
            for (float y = 0; y < dimensions; y+= treeGenerationInterval)
            {
                float val = Mathf.PerlinNoise(offSet + x / dimensions * scale, offSet + y / dimensions * scale);
                if(val > minTreeValueGenerator) //Si el valor de Perlin en una coordenada dada supera el mínimo fijado se intenta generar un árbol
                {
                    o = new Vector3(x, 200, y);
                    dir = new Vector3(0, -300, 0);
                    if (Physics.Raycast(o, dir, out hitResult, 300) && hitResult.transform.gameObject.tag == "Terrain" && hitResult.point.y < maxTreeSpawnHeight && isTerrainValid(hitResult.normal, maxTreeAngle))
                    {
                        GameObject tree = GameObject.Instantiate(GOList[Random.Range(0, GOList.Length)], hitResult.point - new Vector3(0,1,0), Quaternion.identity);
                        tree.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitResult.normal);
                        float newScale = Mathf.InverseLerp(minTreeValueGenerator / 2, 1, val);
                        newScale *= 2;
                        tree.transform.localScale = new Vector3(newScale, newScale, newScale);
                    }
                }
            }
        }
        
    }
}

