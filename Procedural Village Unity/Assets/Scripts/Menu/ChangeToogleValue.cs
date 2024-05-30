using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChangeToogleValue : MonoBehaviour
{
    SettingsManager settingsManager;
    public UnityEvent settingsManagerEvent;
    private void Start()
    {
        bool val = gameObject.GetComponent<Toggle>().isOn;
        settingsManager = GameObject.Find("SettingsManager").GetComponent<SettingsManager>();
    }

    public void ValueChanged()
    {
        settingsManagerEvent.Invoke();
    }
}
