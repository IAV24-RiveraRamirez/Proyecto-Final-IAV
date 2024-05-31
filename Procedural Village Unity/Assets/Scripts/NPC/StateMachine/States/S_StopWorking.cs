using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// El NPC entra en este estado cuando debe dejar de trabajar por cuestiones externas a la hora del día
/// </summary>
public class S_StopWorking : State
{
    public override void Enter()
    {
        fsm.blackboard.Set("WorkDayEnded", typeof(bool), true);
    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Work ended";
    }

    public override void Update(float dt)
    {

    }
}
