using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_LeisureTimeArrived : Transition
{
    NPCInfo info;
    public T_LeisureTimeArrived(State nextState) : base(nextState)
    {
    }

    public override bool Check()
    {
        SimulationManager.TimePeriods time = SimulationManager.Instance.GetCurrentPeriod();
        return time != info.GetWorkingPeriod() && time != SimulationManager.TimePeriods.EVENING;
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
        return "Time for Leisure";
    }
}
