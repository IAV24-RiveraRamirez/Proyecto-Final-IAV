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
    GameObject water = null;
    //Offset del perlin, se usa principalmente para aleatorizar más el resultado
    float offsetX;
    float offsetY;

    PerlinNoiseGenerator generator;

    void Start()
    {
        if (width != height) //Para hacer el mapa cuadrado
        {
            if (width > height) height = width;
            else width = height;
        }

        //(Opcional) Aleatoriza la generación
        offsetX = Random.Range(0f, 10000f);
        offsetY = Random.Range(0f, 10000f);

        generator = new PerlinNoiseGenerator(numOctaves, scale, offsetX, offsetY);

        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
        gameObject.transform.position = new Vector3(-width / 2, 0, -height / 2); //Centra el terreno

        if (water != null) //Generación de agua
        {
            Vector3 spawnPosition = new Vector3(gameObject.transform.position.x + (width / 2), gameObject.transform.position.y + 1, gameObject.transform.position.z + (height / 2));
            GameObject waterRef = GameObject.Instantiate(water, spawnPosition, Quaternion.identity);
            waterRef.transform.localScale = new Vector3(width/2, 1, width/2);
        }
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
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                heights[x, y] = generator.GetValue(x,y); //Calcula el valor de Perlin en un punto dado
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
     

