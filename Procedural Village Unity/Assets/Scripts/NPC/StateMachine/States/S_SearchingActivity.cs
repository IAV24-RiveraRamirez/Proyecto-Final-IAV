using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Busca una actividad aleatoria que hacer en tiempo de ocio
/// </summary>
public class S_SearchingActivity : State
{
    NPCInfo info;
    public override void Enter()
    {
        info = gameObject.GetComponent<NPCInfo>();
    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Searching Leisure Activity";
    }

    public override void Update(float dt)
    {
        NPCBuilding building = SimulationManager.Instance.GetLeisurePlace();
        if(building != null) { info.SetLeisurePlace(building); }
    }
}
