using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_SawmillWorker : SM_Work
{
    public override string ID()
    {
        return "Working on Sawmill";
    }

    public override void Init(GameObject g, StateMachine fsm)
    {
        base.Init(g, fsm);

        NPCInfo info = gameObject.GetComponent<NPCInfo>();
        info.SetWorkingPeriod(SimulationManager.TimePeriods.MORNING);

        State state1 = new S_ToFixedDestination(info.GetWorkPlace().gameObject);
        State state2 = new S_ChoppingWood(0.5f, 30);
        State state3 = new S_ToFixedDestination(info.GetMarketPlace().gameObject);
        State state4 = new S_SellItem(Market.Item.WOOD, "Wood");

        state1.AddTransition(new T_ReachDestination(state2, 1.0f));
        state2.AddTransition(new T_WoodStorageFull(state3));
        state3.AddTransition(new T_ReachDestination(state4, 3.0f));
        state4.AddTransition(new T_ZeroUnitsOfItem(state1, "Wood"));

        AddState(state1).AddState(state2).AddState(state3).AddState(state4);

        object obj = blackboard.Get("Wood", typeof(int));
        if (obj != null && (int)obj > 0)
        {
            StartMachine(state3);
        }
        else StartMachine(state1);
    }
}