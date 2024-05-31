using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Clase abstracta que funciona como un SmartObject, donde los NPCs llegan para hacer
/// una actividad que será definida por los hijos de esta clase.
/// </summary>
public abstract class LeisurePlace : NPCBuilding
{
    // Parameters
    /// <summary>
    /// Tiempo que dura la actividad
    /// </summary>
    [SerializeField] float timeToEnd = 1.5f;

    // Variables
    /// <summary>
    /// Tiempo que ha pasado un NPC haciendo la activdad
    /// </summary>
    Dictionary<NPCInfo, float> timeSpentByNPC = new Dictionary<NPCInfo, float>();

    /// <summary>
    /// Inicialización del edificio
    /// </summary>
    protected override void Start()
    {
        type = BuildingType.LEISURE;
        base.Start();
    }

    /// <summary>
    /// Método abstracto que define la actividad que realizarán los NPCs que 
    /// estén en este edificio.
    /// </summary>
    /// <param name="info"> NPC realizando la actividad </param>
    public abstract void WhileHavingFun(NPCInfo info);

    /// <summary>
    /// Método al que llamarán los NPCs para realizar la actividad
    /// </summary>
    /// <param name="info"> NPC que realiza la actividad </param>
    /// <returns> Si ha acabado la actividad </returns>
    public bool HaveFun(NPCInfo info)
    {
        if(!timeSpentByNPC.ContainsKey(info))
        {
            timeSpentByNPC.Add(info, 0);
        }

        timeSpentByNPC[info] += Time.deltaTime;
        
        WhileHavingFun(info);
        bool ended = timeSpentByNPC[info] >= timeToEnd;
        if(ended) { timeSpentByNPC.Remove(info); }
        return ended;
    }
}
