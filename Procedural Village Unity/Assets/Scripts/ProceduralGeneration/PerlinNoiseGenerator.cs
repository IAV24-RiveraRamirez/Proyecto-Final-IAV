using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseGenerator : MonoBehaviour
{
    List<PerlinOctave> octaves;
    List<float> weights;

    int numWeights;
    float scale;
    public void setValues(int nWeights, float Scale, float offsetX, float offsetY)
    {
        numWeights = nWeights; //N�mero de "capas" que tendr� el ruido
        scale = Scale; //Escala, ser�a como decir el tama�o de las celdas
        PerlinOctave.xOffset = offsetX; //Valores usados para aleatorizar el proceso
        PerlinOctave.yOffset = offsetY;
    }

    public float GetValue(int x, int y) //Calcula el valor para cada punto en todas las capas
    {
        float val = 0;
        float size = 1;
        float weight = 1;
        for (int i = 0; i < numWeights; i++) //Se suman todas las octavas, cada octava tiene m�s resoluci�n pero menos peso
        {
            val += PerlinOctave.calculateValue(x * size / scale, y * size / scale) * weight;
            size *= 2;
            weight /= 2;
        }
        return val;
    }
}
