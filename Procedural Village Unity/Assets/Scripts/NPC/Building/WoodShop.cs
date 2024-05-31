using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Representa una carpinter�a en la simulaci�n
/// </summary>
public class WoodShop : Market
{
    /// <summary>
    /// Define el progreso conseguido para crear una pieza de artesan�a
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
    /// Muetra la madera restante en el almac�n
    /// </summary>
    [SerializeField] TextMeshProUGUI woodText = null;

    // Parameters
    /// <summary>
    /// M�ximo de madera disponible
    /// </summary>
    [SerializeField] int maxWoodStored = 100;
    /// <summary>
    /// Cu�nta madera cuesta empezar un trabajo de artesan�a
    /// </summary>
    [SerializeField] int woodPerCraft = 20;
    
    // Variables
    /// <summary>
    /// Madera en el almac�n actualmente
    /// </summary>
    int currentWood;
    /// <summary>
    /// Indica su hay un NPC rellenando el almac�n actualmente
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
    /// Tambien decide cu�ndo debe ir o no a por madera
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
    /// Indicar si un NPC puede ir a rellenar el almac�n de Madera o si hay alguien haci�ndolo actualmente
    /// </summary>
    public bool LeaveShopToRefill()
    {
        if (npcIsRefillingWood) return false;
        else npcIsRefillingWood = true;

        return true;
    }

    /// <summary>
    /// Indica si hay madera para hacer comenzar un trabajo de artesan�a
    /// </summary>
    public bool WoodAvaliable()
    {
        int current = currentWood - woodPerCraft;
        return current >= 0;
    }

    /// <summary>
    /// Comienza un nuevo trabajo de artesan�a
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
    /// Comprueba si se ha acabado un trabajo, y en ese caso, a�ade el Item crafteado al inventario del trabajador
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
    /// Rellena la carpinter�a
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
