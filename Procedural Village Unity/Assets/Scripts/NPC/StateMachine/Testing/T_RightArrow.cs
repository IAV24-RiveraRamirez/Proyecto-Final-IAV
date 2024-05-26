using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_RightArrow : Transition
{
    public T_RightArrow(State nextState) : base(nextState)
    {
    }

    public override void Enter()
    {

    }

    public override bool Check()
    {
        return Input.GetKeyUp(KeyCode.RightArrow);
    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Pressed Right Arrow";
    }
}
