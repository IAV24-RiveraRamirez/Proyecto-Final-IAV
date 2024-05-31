using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase base para todos los edificios de la simulaci�n
/// </summary>
public class NPCBuilding : MonoBehaviour
{
    // Enums
    /// <summary>
    /// Tipo de edificio
    /// </summary>
    public enum BuildingType { HOUSE, WORK, LEISURE, MARKET }

    // Parameters
    /// <summary>
    /// M�ximo de NPCs trabajando/viviendo en el edificio
    /// </summary>
    [SerializeField] protected int maxNpcs = 4;
    /// <summary>
    /// Tipo de edificio
    /// </summary>
    protected BuildingType type = BuildingType.HOUSE;

    /// <summary>
    /// Indica si el edificio est� lleno (True en caso de que el n�mero de NPCs trabajando/viviendo dentro sea igual que maxNpcs)
    /// </summary>
    bool full = false;

    /// <summary>
    /// Lista NPCs viviendo/trabajando aqu�
    /// </summary>
    List<NPCInfo> npcs = new List<NPCInfo>();

    // Getters
    public int GetMaxNPCs() { return maxNpcs; }
    public BuildingType GetBuildingType() { return type; }

    // Methods
    /// <summary>
    /// A�ade un NPC
    /// </summary>
    /// <param name="npc"> NPC a a�adir </param>
    /// <returns> Si se ha llenado el edificio </returns>
    public virtual bool AddNPC(NPCInfo npc)
    {
        if (npcs.Count < maxNpcs)
        {
            npcs.Add(npc);
        }

        full = npcs.Count >= maxNpcs;

        return full;
    }

    /// <summary>
    /// Desactiva colisi�n
    /// </summary>
    public void DeactivateCollider()
    {
        GetComponent<Collider>().enabled = false;
    }
    
    /// <summary>
    /// A�ade el edificio a la simulaci�n
    /// </summary>
    protected virtual void Start()
    {
        SimulationManager.Instance.AddBuilding(this);
    }

    private void OnDestroy()
    {
        SimulationManager.Instance.RemoveBuilding(this);
    }
}
