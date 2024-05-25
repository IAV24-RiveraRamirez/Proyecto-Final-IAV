using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructuresGenerator : MonoBehaviour
{
    const int MAX_NUM_TEST = 200;
    const float MAX_ANGLE_TOLERANCE = 15f;

    int terrainDimensions;
    public StructuresGenerator(int dimensions)
    {
        terrainDimensions = dimensions;
    }
    public bool findStartSpot(ref Vector3 buildingPos) //Busca de manera aleatoria una posici�n para crear las casas
    {
        bool hasHit = false;
        int nTests = 0;
        RaycastHit hitResult = new RaycastHit();
        while (!hasHit && nTests < MAX_NUM_TEST)
        {
            Vector3 o = new Vector3(Random.Range(0, terrainDimensions), 100, Random.Range(0, terrainDimensions));
            Vector3 dir = new Vector3(0, -200, 0);
            hasHit = Physics.Raycast(o, dir, out hitResult, 200);
            if (hasHit && (hitResult.transform.gameObject.tag != "Terrain" || !isTerrainValid(hitResult.normal))) hasHit = false;
            Color color = Color.red;
            if (hasHit)
            {
                color = Color.green;
                buildingPos = hitResult.point;
            }
            //Debug.DrawRay(o, dir, color, 100);
            nTests++;
        }
        return hasHit;
    }

    public bool findNearSpots(ref Vector3 buildingPos)
    {
        bool hasHit = false;
        int nTests = 0;
        RaycastHit hitResult = new RaycastHit();
        while (!hasHit && nTests < 100)
        {
            Vector3 o = new Vector3(Random.Range((buildingPos.x - 10), (buildingPos.x + 10)), 100, Random.Range((buildingPos.z - 10), (buildingPos.z + 10)));
            Vector3 dir = new Vector3(0, -200, 0);
            hasHit = Physics.Raycast(o, dir, out hitResult, 200);
            if (hasHit && (hitResult.transform.gameObject.tag != "Terrain" || !isTerrainValid(hitResult.normal))) hasHit = false;
            Color color = Color.yellow;
            if (hasHit)
            {
                color = Color.blue;
                buildingPos = hitResult.point;
            }
            //Debug.DrawRay(o, dir, color, 100);
            nTests++;
        }
        return hasHit;
    }

    bool isTerrainValid(Vector3 hitNormal) //Solo genera terreno si el �nuglo que forma la normal de impacto con el vector "up" es menor o igual que un umbral
    {
        return (Mathf.Rad2Deg * (Mathf.Acos(Vector3.Dot(hitNormal, Vector3.up)))) <= MAX_ANGLE_TOLERANCE;
    }
}

