using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
