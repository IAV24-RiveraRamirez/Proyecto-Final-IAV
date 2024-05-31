using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWater : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SettingsManager settings = GameObject.Find("SettingsManager").GetComponent<SettingsManager>();
        if (!settings.toogleWater) Destroy(gameObject);
    }
}
