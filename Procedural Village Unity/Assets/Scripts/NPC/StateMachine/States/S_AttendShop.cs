using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Atienda la tienda que tiene asignado el NPC como lugar de trabajo
/// </summary>
public class S_AttendShop : State
{
    Market market = null;
    NPCInfo info = null;

    public override void Enter()
    {
        info = gameObject.GetComponent<NPCInfo>();

        market = info.GetWorkPlace() as Market;
        market.SetNPCAttending(info);
    }

    public override void Exit()
    {
        market.SetNPCAttending(null);
        SimulationManager.Instance.RemoveTemporalMarket(market);
    }

    public override string ID()
    {
        return "Attending Shop!";
    }

    public override void Update(float dt)
    {
        market.Attend();
    }
}
