using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cambia el nombre de un NPC y le a�ade un n�mero seg�n su orden de instancia
/// </summary>
public class NPCNameChanger : MonoBehaviour
{
    // Variables
    static int numInstances = 0;

    void Start()
    {
        gameObject.name = "NPC_" + numInstances.ToString();
        ++numInstances;
        Destroy(this);
    }

}
