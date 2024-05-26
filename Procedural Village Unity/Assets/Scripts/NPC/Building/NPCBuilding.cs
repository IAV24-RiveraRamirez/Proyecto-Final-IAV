using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBuilding : MonoBehaviour
{
    // Enums
    public enum BuildingType { HOUSE, WORK, LEISURE }

    // Parameters
    [SerializeField] protected int maxNpcs = 4;
    [SerializeField] protected BuildingType type = BuildingType.HOUSE;

    // Variables
    protected int NpcsSettled = 0; // Number of NPCs which are assigned to this building. Does not say how many are actually inside
    protected int NpcsInside = 0; // NPCs currently inside the building

    bool full = false;

    List<NPCInfo> npcs = new List<NPCInfo>();

    // Getters
    public int GetMaxNPCs() { return maxNpcs; }
    public int GetSettledNPCs() { return NpcsSettled; }
    public int GetNPCsInside() { return NpcsInside; }
    public BuildingType GetBuildingType() { return type; }
    public bool IsFull() { return full; }

    // Methods
    public bool AddNPC(NPCInfo npc)
    {
        if (npcs.Count < maxNpcs)
        {
            npcs.Add(npc);
        }

        full = npcs.Count >= maxNpcs;

        return full;
    }
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        SimulationManager.Instance.AddBuilding(this);
    }
}
