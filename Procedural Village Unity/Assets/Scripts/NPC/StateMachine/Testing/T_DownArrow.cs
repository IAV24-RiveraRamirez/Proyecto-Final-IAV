using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_DownArrow : Transition
{
    public T_DownArrow(StateMachine nextState) : base(nextState)
    {
    }

    public override bool Check()
    {
        return Input.GetKeyUp(KeyCode.DownArrow);
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Pressed Down Arrow";
    }
}
