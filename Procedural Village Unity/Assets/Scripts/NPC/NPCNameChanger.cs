using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
