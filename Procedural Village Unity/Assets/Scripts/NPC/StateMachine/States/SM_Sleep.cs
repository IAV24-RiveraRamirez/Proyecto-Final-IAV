using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// M�quina de estados para Dormir
/// </summary>
public class SM_Sleep : StateMachine
{
    public override void Init(GameObject g, StateMachine fsm)
    {
        base.Init(g, fsm);

        NPCInfo info = gameObject.GetComponent<NPCInfo>();

        // Viajar hasta cama y dormir
        State state1 = new S_ToFixedDestination(info.GetHouse().gameObject);
        State state2 = new S_OnBed();

        state1.AddTransition(new T_ReachDestination(state2, 1.0f));

        AddState(state1).AddState(state2);

        StartMachine(state1);
    }

    public override void Enter()
    {
        Debug.Log("Now Going to Sleep");
    }

    public override void Exit()
    {
        Debug.Log("Stop Slepping");
        fsm.blackboard.Set("WorkDayEnded", typeof(bool), false);
    }

    public override string ID()
    {
        return "Sleep";
    }
}
