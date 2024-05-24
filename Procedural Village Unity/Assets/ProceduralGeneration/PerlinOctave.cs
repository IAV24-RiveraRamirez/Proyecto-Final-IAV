using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Xml;
using UnityEngine;

public class PerlinOctave : MonoBehaviour
{
    public static float xOffset;
    public static float yOffset;
    public static float calculateValue(float x, float y)
    {
        // Determina las coordenadas de las esquinas de la casilla
        int x0 = (int)x;
        int y0 = (int)y;
        int x1 = x0 + 1;
        int y1 = y0 + 1;

        // Distancias
        float sx = x - (float)x0;
        float sy = y - (float)y0;

        // Interpola las dos esquinas superiores
        float tl = scaledHeight(x0, y0, x, y); 
        float tr = scaledHeight(x1, y0, x, y);
        float t = Mathf.Lerp(tl, tr, sx);

        // Interpola las dos esquinas inferiores
        float bl = scaledHeight(x0, y1, x, y);
        float br = scaledHeight(x1, y1, x, y);
        float b = Mathf.Lerp(bl, br, sx);

        //Interpolación final para obtener el valor que tiene en cuenta las 4 intersecciones cercanas y las distancias a las mismas
        float value = Mathf.Lerp(t, b, sy);
        return value;
    }

    static float scaledHeight(int ix, int iy, float x, float y)
    {
        //Creamos vectores unitarios pseudo aleatorios en las intersecciones
        Vector2 gradient = randomGradient(ix, iy);

        //Distancias entre el punto a consular y los puntos de intersecciones
        float dx = x - (float)ix;
        float dy = y - (float)iy;
        
        return (dx * gradient.x + dy * gradient.y);
    }

    static Vector2 randomGradient(int ix, int iy) //Generador pseudo aleatorio determinista (si los valores de generación del terreno no cambian, el resultado será siempre el mismo)
    {
        // No precomputed gradients mean this works for any number of grid coordinates
        const int w = 8 * sizeof(uint);
        const int s = w / 2;
        uint a = (uint)ix, b = (uint)iy;
        a *= 3284157443;

        b ^= a << s | a >> w - s;
        b *= 1911520717;

        a ^= b << s | b >> w - s;
        a *= 2048419325;
        float random = (float)(a * (3.14159265 / ~(~0u >> 1))); // in [0, 2*Pi]

        Vector2 v;
        v.x = Mathf.Sin(random + xOffset);
        v.y = Mathf.Cos(random + yOffset);

        return v;
    }
}
