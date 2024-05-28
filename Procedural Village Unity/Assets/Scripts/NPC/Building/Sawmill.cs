using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Sawmill : NPCBuilding
{
    // References
    [SerializeField] TextMeshProUGUI woodText = null;

    // Parameters
    [SerializeField] int maxWoodStored = 100;

    // Variables
    int currentWoodStored = 0;
    bool isStorageFull = false;

    protected override void Start()
    {
        type = BuildingType.WORK;
        base.Start();
        currentWoodStored = 0;
        isStorageFull = false;

        if (!woodText) woodText = transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        if (!woodText) Debug.LogError("Not Text for showing wood on Sawmill object was found");
        else UpdateText();
    }

    // Own Methods
    public void AddWood(int wood)
    {
        currentWoodStored += wood;
        if (currentWoodStored > maxWoodStored) currentWoodStored = maxWoodStored;
        isStorageFull = currentWoodStored == maxWoodStored;
        UpdateText();
    }

    public int GetWoodStored() 
    {
        int returnValue = currentWoodStored;
        currentWoodStored = 0;
        isStorageFull = false;
        UpdateText();
        return returnValue; 
    }

    public bool IsStorageFull() { return isStorageFull; }

    void UpdateText()
    {
        if(woodText) woodText.text = currentWoodStored.ToString();
    }
}