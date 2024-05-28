using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_SawmillWorker : StateMachine
{
    public override string ID()
    {
        return "Working on Sawmill";
    }

    public override void Init(GameObject g, StateMachine fsm)
    {
        base.Init(g, fsm);

        NPCInfo info = gameObject.GetComponent<NPCInfo>();
        State state1 = new S_ToFixedDestination(info.GetWorkPlace().gameObject);
        State state2 = new S_ChoppingWood(0.5f, 3);
        State state3 = new S_ToFixedDestination(info.GetMarketPlace().gameObject);
        State state4 = new S_SellWood();

        state1.AddTransition(new T_ReachDestination(state2, 1.0f));
        state2.AddTransition(new T_WoodStorageFull(state3));
        state3.AddTransition(new T_ReachDestination(state4, 1.0f));
        state4.AddTransition(new T_WoodSold(state1));

        AddState(state1).AddState(state2).AddState(state3).AddState(state4);

        if(blackboard.Get("Wood", typeof(int)) != null)
        {
            StartMachine(state3);
        }
        else StartMachine(state1);
    }

}
