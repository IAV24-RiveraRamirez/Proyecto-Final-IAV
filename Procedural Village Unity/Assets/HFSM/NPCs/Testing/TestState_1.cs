using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestState_1 : State
{
    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override void Update(float dt)
    {
        Debug.Log("Running State 1!!");
    }

    public override string ID()
    {
        return "Test_State_1";
    }
}
