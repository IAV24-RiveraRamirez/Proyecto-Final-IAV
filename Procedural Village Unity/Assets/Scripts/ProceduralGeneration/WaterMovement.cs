using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaterMovement : MonoBehaviour
{
    // Start is called before the first frame update
    Transform waterTransform;
    float elapsedTime;
    void Start()
    {
        waterTransform = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        waterTransform.position = new Vector3(waterTransform.position.x, waterTransform.position.y + (Mathf.Sin(elapsedTime)/2000), waterTransform.position.z);
    }
}
