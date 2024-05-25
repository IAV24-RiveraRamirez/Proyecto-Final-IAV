using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

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
    //Offset del perlin, se usa principalmente para aleatorizar más el resultado
    float offsetX;
    float offsetY;
    int houseSpace = 6; //El tamaño de terreno que aplana una casa 
    bool spawnWater = true;

    float[,] heights;
    Terrain terrain;
    PerlinNoiseGenerator perlinGenerator;
    StructuresGenerator structuresGenerator;
    Vector3 buildingPosition;

    void Start()
    {
        if (width != height) //Para hacer el mapa cuadrado
        {
            if (width > height) height = width;
            else width = height;
        }

        //(Opcional) Aleatoriza la generación
        //offsetX = Random.Range(0f, 10000f);
        //offsetY = Random.Range(0f, 10000f);

        perlinGenerator = new PerlinNoiseGenerator(numOctaves, scale, offsetX, offsetY);

        terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);

        if (spawnWater && water != null) //Generación de agua
        {
            Vector3 spawnPosition = new Vector3(gameObject.transform.position.x + (width / 2), gameObject.transform.position.y + 1, gameObject.transform.position.z + (height / 2));
            GameObject waterRef = GameObject.Instantiate(water, spawnPosition, Quaternion.identity);
            waterRef.transform.localScale = new Vector3(width/2, 1, width/2);
        }
        structuresGenerator = new StructuresGenerator(width);
        StartCoroutine(findStartPosition());
    }

    IEnumerator findStartPosition()
    {
        yield return new WaitForSeconds(0.1f); //Esperamos porque si no los raycast no detectan el agua
        if (structuresGenerator.findStartSpot(ref buildingPosition))
        {
            Vector3 originPoint = buildingPosition;
            PlaceBuilding();
            for(int i = 0; i < housesToSpawn-1; i++)
            {
                if (structuresGenerator.findNearSpots(ref buildingPosition)) PlaceBuilding();
                else buildingPosition = originPoint;
                yield return new WaitForSeconds(0.05f); //Esperamos porque si no los raycast no detectan el agua
            }
        }
    }

    void PlaceBuilding() //Allana el terreno donde está el edificio colocado
    {
        buildingPosition.x = Mathf.Clamp(buildingPosition.x, houseSpace / 2, width - (houseSpace / 2)); //Comprobamos que la casa tenga espacio para aplanar el terreno
        buildingPosition.z = Mathf.Clamp(buildingPosition.z, houseSpace / 2, width - (houseSpace / 2));
        GameObject.Instantiate(house, buildingPosition, Quaternion.identity);
        Vector3 initialPos = new Vector3(buildingPosition.z - (houseSpace / 2), buildingPosition.y, buildingPosition.x - (houseSpace / 2));
        float initial = heights[(int)buildingPosition.z, (int)buildingPosition.x];
        for (int x = 0; x < houseSpace; ++x)
        {
            for (int y = 0; y < houseSpace; y++)
            {
                heights[(int)initialPos.x + x, (int)initialPos.z + y] = initial;
            }
        }
        terrain.terrainData.SetHeights(0, 0, heights);
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

    float CalculateHeight(int x, int y) //Método que usa el Perlin de Unity
    {
        float xCoor = (float)x / width * scale + offsetX;
        float yCoor = (float)y / height * scale + offsetY;

        return Mathf.PerlinNoise(xCoor, yCoor);
    }
}
     

