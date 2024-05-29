using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_ShopHasNoItem : Transition
{
    NPCInfo info;
    public T_ShopHasNoItem(State nextState) : base(nextState)
    {
    }

    public override bool Check()
    {
        return info.GetLastBuyResult() == Market.BuyRequestOutput.SHOP_HAS_NO_ITEM;
    }

    public override void Enter()
    {
        info = gameObject.GetComponent<NPCInfo>();
    }

    public override void Exit()
    {
        info.SetLastBuyResult(Market.BuyRequestOutput.ALL_OK);
    }

    public override string ID()
    {
        return "Shop had no Item";
    }
}
