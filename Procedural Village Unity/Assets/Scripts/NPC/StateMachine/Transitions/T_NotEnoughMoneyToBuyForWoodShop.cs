using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_NotEnoughMoneyToBuyForWoodShop : Transition
{
    NPCInfo info;
    public T_NotEnoughMoneyToBuyForWoodShop(State nextState) : base(nextState)
    {
    }

    public override bool Check()
    {
        return info.GetLastBuyResult() == Market.BuyRequestOutput.CLIENT_HAS_NO_MONEY;
    }

    public override void Enter()
    {
        info = gameObject.GetComponent<NPCInfo>();
    }

    public override void Exit()
    {
        fsm.blackboard.Set("Craft_SellCrafts", typeof(bool), true);
        info.SetLastBuyResult(Market.BuyRequestOutput.ALL_OK);
    }

    public override string ID()
    {
        return "Not enough money to buy";
    }
}
