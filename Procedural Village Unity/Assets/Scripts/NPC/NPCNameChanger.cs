using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cambia el nombre de un NPC y le añade un número según su orden de instancia
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
