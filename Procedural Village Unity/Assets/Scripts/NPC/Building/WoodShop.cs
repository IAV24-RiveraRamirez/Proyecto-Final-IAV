using System.Collections.Generic;
using TMPro;
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
    public void Work(NPCInfo worker)
    {
        if (!npcProgress.ContainsKey(worker))
        {
            npcProgress.Add(worker, null);
        }
        if (npcProgress[worker] == null)
        {
            CraftingProgress newProgress = StartNewCraft();
            if(newProgress == null)
            {
                Debug.Log("Need More Wood");

                Blackboard blackboard = worker.GetComponent<NPCBehaviourSM>().GetStateMachine().blackboard;

                if (!(bool)blackboard.Get("Craft_GoRefill", typeof(bool)))
                {
                    bool goToRefill = LeaveShopToRefill();
                    if (goToRefill)
                    {
                        blackboard.Set("Craft_GoRefill", typeof(bool), true);
                        blackboard.Set("Craft_WoodAmount", typeof(int), GetMaxWood());
                    }
                    else
                    {
                        blackboard.Set("WorkDayEnded", typeof(bool), true);
                    }
                }
            } 
            else npcProgress[worker] = newProgress;
        }
        else
        {
            npcProgress[worker].MakeProgress(Time.deltaTime);
            CraftCompleted(worker);
        }
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

    public bool CraftCompleted(NPCInfo worker)
    {
        if(!npcProgress[worker].IsCompleted()) { return false; }

        Blackboard blackboard = worker.GetComponent<NPCBehaviourSM>().GetStateMachine().blackboard;
        object obj = blackboard.Get("Craft_ItemsCrafted", typeof(int));
        if (obj == null)
        {
            blackboard.Set("Craft_ItemsCrafted", typeof(int), 1);
        }
        else
        {
            int numCrafts = (int)obj;
            blackboard.Set("Craft_ItemsCrafted", typeof(int), numCrafts + 1);
        }

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

    protected void Update()
    {
        if (SimulationManager.Instance.GetCurrentPeriod() == SimulationManager.TimePeriods.EVENING) npcIsRefillingWood = false;
    }

    private void OnDestroy()
    {
        SimulationManager.Instance.RemoveBuilding(this);
    }
}
