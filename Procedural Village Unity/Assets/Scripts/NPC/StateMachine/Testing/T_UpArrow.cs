using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_UpArrow : Transition
{
    public T_UpArrow(StateMachine nextState) : base(nextState)
    {

    }

    public override void Enter()
    {

    }

    public override bool Check()
    {
        return Input.GetKeyUp(KeyCode.UpArrow);
    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Pressed Up Arrow";
    }
}
