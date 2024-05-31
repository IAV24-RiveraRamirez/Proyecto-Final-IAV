using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class Sawmill : NPCBuilding
{
    // References
    [SerializeField] TextMeshProUGUI woodText = null;

    // Parameters
    [SerializeField] float timeToProduce = 0.5f;
    [SerializeField] int woodProduced = 3;
    [SerializeField] int maxWoodStored = 100;

    // Variables
    Dictionary<NPCInfo, float> timePerWorker = new Dictionary<NPCInfo, float>();
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

    public void Work(NPCInfo info)
    {
        if (!timePerWorker.ContainsKey(info)) {
            timePerWorker.Add(info, 0);
        }

        timePerWorker[info] += Time.deltaTime;
        if (timePerWorker[info] > timeToProduce)
        {
            timePerWorker[info] = 0;
            AddWood(woodProduced);
        }
    }

    void UpdateText()
    {
        if(woodText) woodText.text = currentWoodStored.ToString();
    }
}