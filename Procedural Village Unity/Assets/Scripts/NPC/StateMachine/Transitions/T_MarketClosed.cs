using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_MarketClosed : Transition
{
    protected NPCInfo info;
    public T_MarketClosed(State nextState) : base(nextState)
    {
    }

    public override bool Check()
    {
        return info.GetLastBuyResult() == Market.BuyRequestOutput.MARKET_CLOSED;
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
        return "Not enough money";
    }
}
