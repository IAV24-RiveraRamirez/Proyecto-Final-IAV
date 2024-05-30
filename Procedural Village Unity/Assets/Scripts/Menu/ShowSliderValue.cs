using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShowSliderValue : MonoBehaviour
{
    SettingsManager settingsManager;
    public UnityEvent<float> settingsManagerEvent;
    private void Start()
    {
        float val = gameObject.transform.parent.GetComponent<Slider>().value;
        gameObject.GetComponent<TextMeshProUGUI>().text = val.ToString();
        settingsManager = GameObject.Find("SettingsManager").GetComponent<SettingsManager>();
        settingsManagerEvent.Invoke(val);
    }

    public void ValueChanged(float value)
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = value.ToString();
        settingsManagerEvent.Invoke(value);
    }
}
