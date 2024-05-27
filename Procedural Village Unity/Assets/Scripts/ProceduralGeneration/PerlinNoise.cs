using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField]
    int width = 256;
    [SerializeField]
    int height = 256;
    //Altura del terreno
    [SerializeField]
    int depth = 20;
    [SerializeField]
    int numOctaves = 15;
    [SerializeField]
    float scale = 10f;
    [SerializeField]
    int housesToSpawn = 6;
    [SerializeField]
    GameObject water = null;
    [SerializeField]
    GameObject house = null;
    //Offset del perlin, se usa principalmente para aleatorizar m�s el resultado
    float offsetX;
    float offsetY;
    int houseSpace = 6; //El tama�o de terreno que aplana una casa 
    bool spawnWater = true;

    float[,] heights;
    Terrain terrain;
    PerlinNoiseGenerator perlinGenerator;
    StructuresGenerator structuresGenerator;
    Vector3 buildingPosition;
    GameObject cameraRef;
    Vector3 cameraPosition;

    void Start()
    {
        if (width != height) //Para hacer el mapa cuadrado
        {
            if (width > height) height = width;
            else width = height;
        }

        //(Opcional) Aleatoriza la generaci�n
        //offsetX = Random.Range(0f, 10000f);
        //offsetY = Random.Range(0f, 10000f);

        perlinGenerator = new PerlinNoiseGenerator(numOctaves, scale, offsetX, offsetY);

        terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);

        if (spawnWater && water != null) //Generaci�n de agua
        {
            Vector3 spawnPosition = new Vector3(gameObject.transform.position.x + (width / 2), gameObject.transform.position.y + 1, gameObject.transform.position.z + (height / 2));
            GameObject waterRef = GameObject.Instantiate(water, spawnPosition, Quaternion.identity);
            waterRef.transform.localScale = new Vector3(width/2, 1, width/2);
        }
        structuresGenerator = new StructuresGenerator(width);
        StartCoroutine(findStartPosition());

        cameraRef = GameObject.Find("Main Camera");//Se puede hacer mejor
        if(cameraRef) cameraPosition = cameraRef.transform.position;
    }

    private void Update()
    {
        if(cameraRef)
        {
            cameraRef.transform.position = Vector3.Lerp(cameraRef.transform.position, cameraPosition, Time.deltaTime); //Movimiento de la c�mara para apuntar hacia la aldea
        }
    }

    IEnumerator findStartPosition()
    {
        yield return new WaitForSeconds(0.1f); //Esperamos porque si no los raycast no detectan el agua
        if (structuresGenerator.findStartSpot(ref buildingPosition))
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
                if (structuresGenerator.findNearSpots(ref buildingPosition))
                {
                    PlaceBuilding();
                    center += buildingPosition;
                    places.Add(buildingPosition);
                }
                else buildingPosition = originPoint;
                yield return new WaitForSeconds(0.05f); //Esperamos porque si no los raycast no detectan el agua
            }
            center = center / places.Count; //Aproximaci�n del centro de la aldea
            foreach (Vector3 v in places)
            {
                maxVillageSize = Mathf.Max(maxVillageSize, Vector3.Distance(center, v));
            }
            maxVillageSize *= 2f; //Lo agrandamos para asegurar que llega
            CreateNavMesh(maxVillageSize, center);
        }
    }

    void PlaceBuilding() //Allana el terreno donde est� el edificio colocado
    {
        buildingPosition.x = Mathf.Clamp(buildingPosition.x, houseSpace / 2 +1, width - (houseSpace / 2) + 1); //Comprobamos que la casa tenga espacio para aplanar el terreno
        buildingPosition.z = Mathf.Clamp(buildingPosition.z, houseSpace / 2 +1, width - (houseSpace / 2) + 1);
        GameObject.Instantiate(house,buildingPosition, Quaternion.identity);
        Vector3 initialPos = new Vector3(buildingPosition.z - (houseSpace / 2), buildingPosition.y, buildingPosition.x - (houseSpace / 2));
        float initialHeight = heights[(int)buildingPosition.z, (int)buildingPosition.x];
        for (int x = 0; x <= houseSpace; ++x)
        {
            for (int y = 0; y <= houseSpace; y++)
            {
                float distanceToCenter = Vector2.Distance(new Vector2(x, y), new Vector2(houseSpace / 2, houseSpace / 2));
                if (distanceToCenter > 2.5) //Distancia est�ndar
                {
                    float t = Mathf.InverseLerp(houseSpace / 2, 0, distanceToCenter); //Interpolamos alturas
                    float interpolatedHeight = Mathf.Lerp(heights[(int)initialPos.x + x, (int)initialPos.z + y], initialHeight, t);
                    heights[(int)initialPos.x + x, (int)initialPos.z + y] = interpolatedHeight;
                }
                else //Si es un v�rtice muy cercano al punto de generaci�n, se aplana directamente
                {
                    heights[(int)initialPos.x + x, (int)initialPos.z + y] = initialHeight;
                }
            }
        }
        terrain.terrainData.SetHeights(0, 0, heights);
    }

    void CreateNavMesh(float maxDimension, Vector3 center) //Crea la malla de navegaci�n de la IA
    {
        GameObject navMesh = new GameObject();
        navMesh.transform.SetParent(terrain.gameObject.transform);
        navMesh.name = "NavMeshGO";
        navMesh.AddComponent<NavMeshSurface>();
        NavMeshSurface navMeshSurface = navMesh.GetComponent<NavMeshSurface>();
        navMeshSurface.collectObjects = CollectObjects.Volume; //Ajustamos su alcance a un volumen
        navMeshSurface.size = new Vector3(maxDimension, depth, maxDimension);//volumen del tama�o de la aldea
        navMeshSurface.center = center;
        int defaultLayer = LayerMask.NameToLayer("Default");
        navMeshSurface.layerMask = 1 << defaultLayer; //Solo cogemos los elementos del terreno (no bakeamos el agua para ahorrar tiempo)
        navMeshSurface.BuildNavMesh();
    }
    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);
        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    float[,] GenerateHeights()
    {
        heights = new float[width, height];
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                heights[x, y] = perlinGenerator.GetValue(x,y); //Calcula el valor de Perlin en un punto dado
                //heights[x, y] = CalculateHeight(x, y);
            }
        }
        return heights;
    }

    float CalculateHeight(int x, int y) //M�todo que usa el Perlin de Unity
    {
        float xCoor = (float)x / width * scale + offsetX;
        float yCoor = (float)y / height * scale + offsetY;

        return Mathf.PerlinNoise(xCoor, yCoor);
    }
}
     

