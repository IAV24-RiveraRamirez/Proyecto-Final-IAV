using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Representa una carpintería en la simulación
/// </summary>
public class WoodShop : Market
{
    /// <summary>
    /// Define el progreso conseguido para crear una pieza de artesanía
    /// </summary>
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
    /// <summary>
    /// Muetra la madera restante en el almacén
    /// </summary>
    [SerializeField] TextMeshProUGUI woodText = null;

    // Parameters
    /// <summary>
    /// Máximo de madera disponible
    /// </summary>
    [SerializeField] int maxWoodStored = 100;
    /// <summary>
    /// Cuánta madera cuesta empezar un trabajo de artesanía
    /// </summary>
    [SerializeField] int woodPerCraft = 20;
    
    // Variables
    /// <summary>
    /// Madera en el almacén actualmente
    /// </summary>
    int currentWood;
    /// <summary>
    /// Indica su hay un NPC rellenando el almacén actualmente
    /// </summary>
    bool npcIsRefillingWood = false;

    /// <summary>
    /// Progreso de un NPC en su trabajo
    /// </summary>
    Dictionary<NPCInfo, CraftingProgress> npcProgress = new Dictionary<NPCInfo, CraftingProgress>();

    // Getters 
    public int GetMaxWood() { return maxWoodStored; }

    // Own Methods
    /// <summary>
    /// Llamado por un carpintero para progesar en su trabajo. 
    /// Tambien decide cuándo debe ir o no a por madera
    /// </summary>
    /// <param name="worker"> Carpintero trabajando </param>
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
                    if (LeaveShopToRefill())
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
    /// <summary>
    /// Indicar si un NPC puede ir a rellenar el almacén de Madera o si hay alguien haciéndolo actualmente
    /// </summary>
    public bool LeaveShopToRefill()
    {
        if (npcIsRefillingWood) return false;
        else npcIsRefillingWood = true;

        return true;
    }

    /// <summary>
    /// Indica si hay madera para hacer comenzar un trabajo de artesanía
    /// </summary>
    public bool WoodAvaliable()
    {
        int current = currentWood - woodPerCraft;
        return current >= 0;
    }

    /// <summary>
    /// Comienza un nuevo trabajo de artesanía
    /// </summary>
    /// <returns> Indica si puede comenzarse o no dicho trabajo </returns>
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

    /// <summary>
    /// Comprueba si se ha acabado un trabajo, y en ese caso, añade el Item crafteado al inventario del trabajador
    /// </summary>
    /// <param name="worker"></param>
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

    /// <summary>
    /// Rellena la carpintería
    /// </summary>
    /// <param name="amount"> Cantidad a rellenar </param>
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
