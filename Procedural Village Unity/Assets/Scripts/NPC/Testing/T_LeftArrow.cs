using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_LeftArrow : Transition
{
    public T_LeftArrow(State nextState) : base(nextState)
    {
    }

    public override void Enter()
    {

    }

    public override bool Check()
    {
        return Input.GetKeyUp(KeyCode.LeftArrow);
    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Pressed Left Arrow";
    }
}
