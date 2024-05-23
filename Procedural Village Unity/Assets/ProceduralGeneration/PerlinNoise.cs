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
    float scale = 10f;
    //Offset del perlin, se usa principalmente para aleatorizar más el resultado
    float offsetX;
    float offsetY;
    
    void Start()
    {
        //(Opcional)
        offsetX = Random.Range(0f, 10000f);
        offsetY = Random.Range(0f, 10000f);

        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
        gameObject.transform.position = new Vector3(-width/2,0,-height/2); //Centra el terreno
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
                heights[x, y] = CalculateHeight(x, y);
            }
        }
        return heights;
    }

    float CalculateHeight (int x, int y)
    {
        float xCoor = (float)x / width * scale + offsetX;
        float yCoor = (float)y / height * scale + offsetY;

        return Mathf.PerlinNoise(xCoor, yCoor);
    }
}
