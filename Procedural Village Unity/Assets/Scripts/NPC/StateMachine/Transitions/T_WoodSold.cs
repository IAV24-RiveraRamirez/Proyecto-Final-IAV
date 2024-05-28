using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_WoodSold : Transition
{
    public T_WoodSold(State nextState) : base(nextState)
    {
    }

    public override bool Check()
    {
        return fsm.blackboard.Get("Wood", typeof(int)) == null;
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Wood was sold";
    }
}
