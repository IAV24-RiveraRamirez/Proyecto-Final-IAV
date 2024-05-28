using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class S_SellWood : State
{
    public override void Enter()
    {
        NPCInfo info = gameObject.GetComponent<NPCInfo>();
        Market market = info.GetMarketPlace() as Market;
        info.ChangeMoney(market.Sell(Market.Item.WOOD, (int)fsm.blackboard.Get("Wood", typeof(int))));
        fsm.blackboard.Set("Wood", typeof(int), null);
    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Selling Wood";
    }

    public override void Update(float dt)
    {

    }
}
