using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Representa un aserradero en la simulación
/// </summary>
public class Sawmill : NPCBuilding
{
    // References
    /// <summary>
    /// Texto que indica la madera actual dentro del aserradero
    /// </summary>
    [SerializeField] TextMeshProUGUI woodText = null;

    // Parameters
    /// <summary>
    /// Tiempo para producir madera
    /// </summary>
    [SerializeField] float timeToProduce = 0.5f;
    /// <summary>
    /// Madera producida
    /// </summary>
    [SerializeField] int woodProduced = 3;
    /// <summary>
    /// Máximo de madera almacenada
    /// </summary>
    [SerializeField] int maxWoodStored = 100;

    // Variables
    /// <summary>
    /// Progreso de producción de madera por cada NPC trabajando
    /// </summary>
    Dictionary<NPCInfo, float> timePerWorker = new Dictionary<NPCInfo, float>();

    /// <summary>
    /// Madera actualmente almacenada
    /// </summary>
    int currentWoodStored = 0;
    /// <summary>
    /// Indica si el almacén de madera está lleno
    /// </summary>
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
    /// <summary>
    /// Añade madera al almacenamiento
    /// </summary>
    public void AddWood(int wood)
    {
        currentWoodStored += wood;
        if (currentWoodStored > maxWoodStored) currentWoodStored = maxWoodStored;
        isStorageFull = currentWoodStored == maxWoodStored;
        UpdateText();
    }

    /// <summary>
    /// Vacía la madera del almacén
    /// </summary>
    /// <returns> Madera almacenada </returns>
    public int GetWoodStored() 
    {
        int returnValue = currentWoodStored;
        currentWoodStored = 0;
        isStorageFull = false;
        UpdateText();
        return returnValue; 
    }

    public bool IsStorageFull() { return isStorageFull; }

    /// <summary>
    /// Método que llama un NPC para trabajar en un aserradero
    /// </summary>
    /// <param name="info"> NPC trabajador </param>
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