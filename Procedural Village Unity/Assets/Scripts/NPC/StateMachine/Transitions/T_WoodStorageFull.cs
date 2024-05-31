using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Indica cuándo se ha llenado el almacén de madera de un aserradero
/// </summary>
public class T_WoodStorageFull : Transition
{
    // References 
    Sawmill sawmill = null;
    public T_WoodStorageFull(State nextState) : base(nextState)
    {
    }

    public override bool Check()
    {
        return sawmill.IsStorageFull();
    }

    public override void Enter()
    {
        sawmill = gameObject.GetComponent<NPCInfo>().GetWorkPlace() as Sawmill;
    }

    public override void Exit()
    {
        NPCInfo info = gameObject.GetComponent<NPCInfo>();
        Sawmill sawmill = info.GetWorkPlace() as Sawmill;
        UnityEngine.AI.NavMeshAgent navMeshAgent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        navMeshAgent.SetDestination(info.GetMarketPlace().transform.position);

        fsm.blackboard.Set("Wood", typeof(int), sawmill.GetWoodStored());
    }

    public override string ID()
    {
        return "Wood Storage is Full";
    }
}
