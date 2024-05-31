using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Indica que la tienda donde se ha intentado comprar no tiene ese ítem disponible
/// </summary>
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
        info.SetLastBuyResult(Market.BuyRequestOutput.RESTING_STATE);
    }

    public override string ID()
    {
        return "Shop had no Item";
    }
}
