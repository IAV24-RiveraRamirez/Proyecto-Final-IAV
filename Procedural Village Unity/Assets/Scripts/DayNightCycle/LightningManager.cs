using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


[ExecuteAlways]
public class LightningManager : MonoBehaviour
{
    // References
    [SerializeField] private Light directionalLight = null;
    [SerializeField] private LightningPreset preset = null;
    [SerializeField] private SimulationManager simManager = null;
    
   

    private void OnValidate()
    {

        if (directionalLight != null && simManager != null) return;

        if(simManager == null)
        {
            simManager = GameObject.FindObjectOfType<SimulationManager>();
        }
        if(RenderSettings.sun != null)
        {
            directionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();

            foreach(Light l in lights)
            {
                if (l.type == LightType.Directional)
                {
                    directionalLight = l;
                    break;
                }
            }
        }
    }

    private void UpdateLightning(float timepercent)
    {
        RenderSettings.ambientLight = preset.AmbientColor.Evaluate(timepercent);
        RenderSettings.fogColor = preset.FogColor.Evaluate(timepercent);

        if(directionalLight!=null)
        {
            directionalLight.color = preset.DirectionColor.Evaluate(timepercent);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3(timepercent * 360f - 90f, 170f, 0));
        }
    }
    
    private void Update()
    {
        if (preset == null) return;

        UpdateLightning(simManager.GetTimePercent());
    }
}
