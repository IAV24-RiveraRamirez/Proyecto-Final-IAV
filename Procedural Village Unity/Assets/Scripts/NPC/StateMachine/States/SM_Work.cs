using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SM_Work : StateMachine
{
    public override void Init(GameObject g, StateMachine fsm)
    {
        base.Init(g, fsm);

        fsm.blackboard.Set("WorkDayEnded", typeof(bool), false);
    }
}
