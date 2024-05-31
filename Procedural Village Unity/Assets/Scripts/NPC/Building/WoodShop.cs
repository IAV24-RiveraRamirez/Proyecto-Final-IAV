using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class WoodShop : Market
{
    public class CraftingProgress
    {
        public const float timeToCompletion = 1.0f;
        public float progress;

        public void MakeProgress(float newProgres)
        {
            progress += newProgres;
        }

        public bool IsCompleted()
        {
            return progress >= timeToCompletion;
        }

        public CraftingProgress()
        {
            this.progress = 0;
        }
    }

    // References
    [SerializeField] TextMeshProUGUI woodText = null;

    // Parameters
    [SerializeField] int maxWoodStored = 100;
    [SerializeField] int woodPerCraft = 20;
    
    // Variables
    int currentWood;
    bool npcIsRefillingWood = false;

    Dictionary<NPCInfo, CraftingProgress> npcProgress = new Dictionary<NPCInfo, CraftingProgress>();

    // Getters 
    public int GetMaxWood() { return maxWoodStored; }

    // Own Methods
    public CraftingProgress Work(NPCInfo worker)
    {
        if (!npcProgress.ContainsKey(worker))
        {
            npcProgress.Add(worker, null);
        }
        CraftingProgress progress = null;
        progress = npcProgress[worker];
        if (progress == null)
        {
            progress = StartNewCraft();
            if (progress == null)
            {
                Debug.Log("Need more wood!");
                return null;
            }
        }
        return progress;
    }

    public bool LeaveShopToRefill()
    {
        if (npcIsRefillingWood) return false;
        else npcIsRefillingWood = true;

        return true;
    }

    public bool WoodAvaliable()
    {
        int current = currentWood - woodPerCraft;
        return current >= 0;
    }

    public CraftingProgress StartNewCraft()
    {
        if (WoodAvaliable())
        {
            currentWood -= woodPerCraft;
            if(woodText) 
                woodText.text = currentWood.ToString();
            CraftingProgress progress = new CraftingProgress();
            return progress;
        }
        else return null;
    }

    public bool CraftCompleted(NPCInfo worker, CraftingProgress progress)
    {
        if(!progress.IsCompleted()) { Debug.Log("Progress from "+ worker.gameObject.name+ " was not Completed!"); return false; }

        npcProgress[worker] = null;
        return true;
    }

    public void StopWorking(NPCInfo worker, CraftingProgress progress)
    {
        npcProgress[worker] = progress;
    }

    public void RefillShop(int amount)
    {
        currentWood += amount;
        Mathf.Clamp(currentWood, 0, maxWoodStored);
        npcIsRefillingWood = false;
    }

    protected override void Start()
    {
        type = BuildingType.WORK;
        currentWood = maxWoodStored;
        SetUpAvaliableItems();
        SimulationManager.Instance.AddBuilding(this);
    }

    protected override void Update()
    {
        base.Update();
        if (SimulationManager.Instance.GetCurrentPeriod() == SimulationManager.TimePeriods.EVENING) npcIsRefillingWood = false;
    }

    private void OnDestroy()
    {
        SimulationManager.Instance.RemoveBuilding(this);
    }
}
