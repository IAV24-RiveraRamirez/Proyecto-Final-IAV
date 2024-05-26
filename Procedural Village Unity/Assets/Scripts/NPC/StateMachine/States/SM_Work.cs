using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_Work : StateMachine
{
    public override string ID()
    {
        return "Working";
    }

    public override void Init(GameObject g, StateMachine fsm)
    {
        base.Init(g, fsm);

        NPCInfo info = gameObject.GetComponent<NPCInfo>();
        S_ToFixedDestination state1 = new S_ToFixedDestination(info.GetWorkPlace().gameObject);
        State state2 = new S_Working();

        state1.AddTransition(new T_ReachDestination(state2, 1.0f));

        AddState(state1).AddState(state2);

        StartMachine(state1);
    }
}
