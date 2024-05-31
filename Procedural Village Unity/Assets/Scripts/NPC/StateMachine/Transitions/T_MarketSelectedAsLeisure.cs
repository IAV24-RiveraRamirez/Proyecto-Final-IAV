using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Indica cuándo se ha encontrado un mercado como actividad de ocio
/// </summary>
public class T_MarketSelectedAsLeisure : Transition
{
    NPCInfo info;
    public T_MarketSelectedAsLeisure(State nextState) : base(nextState)
    {
    }

    public override bool Check()
    {
        return info.GetLeisurePlace() != null && info.GetLeisurePlace() is Market;
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
        return "Market found as Leisure activity!";
    }
}
