using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructuresGenerator : MonoBehaviour
{
    [SerializeField,Min(1)]
    int MAX_NUM_TEST = 300;
    [SerializeField, Range(0, 180)]
    float MAX_ANGLE_TOLERANCE = 15f;
    [SerializeField,Min(0)]
    float MAX_HEIGHT_DIFF_TOLERANCE = 3f;
    [SerializeField, Min(-1)]
    float MAX_VILLAGE_SEPARATION = -1f;

    int terrainDimensions;
    int buildingDimensions;
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
        while (nTests < MAX_NUM_TEST)
        {
            Vector3 o = new Vector3(Random.Range(0, terrainDimensions - 1), 100, Random.Range(0, terrainDimensions-1));
            Vector3 dir = new Vector3(0, -200, 0);
            bool hasHit = Physics.Raycast(o, dir, out hitResult, 200);
            if (hasHit && hitResult.transform.gameObject.tag == "Terrain" && isTerrainValid(hitResult.normal))
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
        while (nTests < MAX_NUM_TEST) //Hacemos todos los test aunque encontremos un sitio válido porque queremos el mejor sitio de todos los válidos encontrados
        {
            Vector3 o = new Vector3(Random.Range((buildingPos.x - 20), (buildingPos.x + 20)), 100, Random.Range((buildingPos.z - 20), (buildingPos.z + 20)));
            Vector3 dir = new Vector3(0, -200, 0);
            bool hasHit = Physics.Raycast(o, dir, out hitResult, 200);
            if (hasHit && hitResult.transform.gameObject.tag == "Terrain" && isTerrainValid(hitResult.normal) && isPointInsideBoundaries(hitResult.point) && Mathf.Abs(hitResult.point.y - originalHeight) <= MAX_HEIGHT_DIFF_TOLERANCE && (MAX_VILLAGE_SEPARATION < 0 || Vector3.Distance(hitResult.point, startPosition) <= MAX_VILLAGE_SEPARATION))
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

    bool isTerrainValid(Vector3 hitNormal) //Solo genera terreno si el �nuglo que forma la normal de impacto con el vector "up" es menor o igual que un umbral
    {
        return (Mathf.Rad2Deg * (Mathf.Acos(Vector3.Dot(hitNormal, Vector3.up)))) <= MAX_ANGLE_TOLERANCE;
    }

    bool isPointInsideBoundaries(Vector3 point) //Comprobamos si el edificio mantiene una cierta distancia con los bordes
    {
        return (point.x > buildingDimensions && point.z > buildingDimensions && point.x < terrainDimensions - buildingDimensions && point.z < terrainDimensions - buildingDimensions);
    }
}

