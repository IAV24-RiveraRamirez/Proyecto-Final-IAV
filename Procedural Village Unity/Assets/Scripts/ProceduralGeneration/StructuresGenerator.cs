using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructuresGenerator : MonoBehaviour
{
    const int MAX_NUM_TEST = 200;
    const float MAX_ANGLE_TOLERANCE = 15f;

    int terrainDimensions;
    int buildingDimensions;
    public StructuresGenerator(int dimensions, int bDimensions)
    {
        terrainDimensions = dimensions;
        buildingDimensions = bDimensions;
    }
    public bool findStartSpot(ref Vector3 buildingPos) //Busca de manera aleatoria una posici�n para crear las casas
    {
        bool hasHit = false;
        int nTests = 0;
        RaycastHit hitResult = new RaycastHit();
        while (!hasHit && nTests < MAX_NUM_TEST)
        {
            Vector3 o = new Vector3(Random.Range(0, 0), 100, Random.Range(terrainDimensions-1, terrainDimensions-1));
            Vector3 dir = new Vector3(0, -200, 0);
            hasHit = Physics.Raycast(o, dir, out hitResult, 200);
            if (hasHit && (hitResult.transform.gameObject.tag != "Terrain" || !isTerrainValid(hitResult.normal))) hasHit = false;
            if (hasHit)
            {
                buildingPos = hitResult.point;
                if (!isPointInsideBoundaries(hitResult.point)) //Movemos el primer edificio si no guarda unos márgenes mínimos con el borde del terreno
                {
                    buildingPos.x = Mathf.Clamp(buildingPos.x, buildingDimensions + 1, terrainDimensions - (buildingDimensions + 1));
                    buildingPos.z = Mathf.Clamp(buildingPos.z, buildingDimensions + 1, terrainDimensions - (buildingDimensions + 1));
                }
            }
            nTests++;
        }
        return hasHit;
    }

    public bool findNearSpots(ref Vector3 buildingPos) //Método usado para la creación de casas adyacentes
    {
        bool hasHit = false;
        int nTests = 0;
        RaycastHit hitResult = new RaycastHit();
        while (!hasHit && nTests < MAX_NUM_TEST)
        {
            Vector3 o = new Vector3(Random.Range((buildingPos.x - 10), (buildingPos.x + 10)), 100, Random.Range((buildingPos.z - 10), (buildingPos.z + 10)));
            Vector3 dir = new Vector3(0, -200, 0);
            hasHit = Physics.Raycast(o, dir, out hitResult, 200);
            if (hasHit && (hitResult.transform.gameObject.tag != "Terrain" || !isTerrainValid(hitResult.normal) ||  !isPointInsideBoundaries(hitResult.point))) hasHit = false;
            if (hasHit)
            {
                buildingPos = hitResult.point;
            }
            nTests++;
        }
        return hasHit;
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

