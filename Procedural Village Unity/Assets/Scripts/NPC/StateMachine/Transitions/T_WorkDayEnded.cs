using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Indica que el día de trabajo se ha acabado abruptamente
/// </summary>
public class T_WorkDayEnded : Transition
{
    public T_WorkDayEnded(State nextState) : base(nextState)
    {
    }

    public override bool Check()
    {
        return (bool)fsm.blackboard.Get("WorkDayEnded", typeof(bool));
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Work Day Ended";
    }
}
