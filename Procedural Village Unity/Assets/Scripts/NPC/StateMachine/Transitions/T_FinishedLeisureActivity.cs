using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_FinishedLeisureActivity : Transition
{
    NPCInfo info;
    public T_FinishedLeisureActivity(State nextState) : base(nextState)
    {
    }

    public override bool Check()
    {
        return info.GetLeisurePlace() == null;
    }

    public override void Enter()
    {
        info = gameObject.GetComponent<NPCInfo>();
    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Activity Finished!";
    }
}
