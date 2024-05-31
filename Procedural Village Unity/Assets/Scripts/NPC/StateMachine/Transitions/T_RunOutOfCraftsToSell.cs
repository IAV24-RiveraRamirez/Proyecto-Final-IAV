using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Indica cuándo se han acabado los trabajos de artesanía disponibles para vender
/// </summary>
public class T_RunOutOfCraftsToSell : Transition
{
    Market market;
    public T_RunOutOfCraftsToSell(State nextState) : base(nextState)
    {
    }

    public override bool Check()
    {
        return !market.IsItemAvaliable(Market.Item.CRAFTS, 1);
    }

    public override void Enter()
    {
        market = gameObject.GetComponent<NPCInfo>().GetWorkPlace() as Market;
    }

    public override void Exit()
    {
        fsm.blackboard.Set("Craft_SellCrafts", typeof(bool), false);
    }

    public override string ID()
    {
        return "Runned out of crafts to sell";
    }
}
