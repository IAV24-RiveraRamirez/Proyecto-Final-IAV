using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase base para todos los edificios de la simulación
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
    /// Máximo de NPCs trabajando/viviendo en el edificio
    /// </summary>
    [SerializeField] protected int maxNpcs = 4;
    /// <summary>
    /// Tipo de edificio
    /// </summary>
    protected BuildingType type = BuildingType.HOUSE;

    /// <summary>
    /// Indica si el edificio está lleno (True en caso de que el número de NPCs trabajando/viviendo dentro sea igual que maxNpcs)
    /// </summary>
    bool full = false;

    /// <summary>
    /// Lista NPCs viviendo/trabajando aquí
    /// </summary>
    List<NPCInfo> npcs = new List<NPCInfo>();

    // Getters
    public int GetMaxNPCs() { return maxNpcs; }
    public BuildingType GetBuildingType() { return type; }

    // Methods
    /// <summary>
    /// Añade un NPC
    /// </summary>
    /// <param name="npc"> NPC a añadir </param>
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
    /// Desactiva colisión
    /// </summary>
    public void DeactivateCollider()
    {
        GetComponent<Collider>().enabled = false;
    }
    
    /// <summary>
    /// Añade el edificio a la simulación
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
