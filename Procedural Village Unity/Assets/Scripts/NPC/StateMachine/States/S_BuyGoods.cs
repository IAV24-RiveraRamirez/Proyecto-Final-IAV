using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class S_BuyGoods : State
{
    Market market = null;

    float timer;
    float timeToSell = 1.5f;
    public override void Enter()
    {
        if(fsm.blackboard.Get("Sell_Request", typeof(bool)) == null)
        {
            fsm.blackboard.Set("Sell_Request", typeof(bool), false);
        }
        NPCInfo info = gameObject.GetComponent<NPCInfo>();

        market = info.GetMarketPlace() as Market;
        market.SetNPCAttending(info);
    }

    public override void Exit()
    {
        market.SetNPCAttending(null);   
    }

    public override string ID()
    {
        return "Buying Goods from Clients";
    }

    public override void Update(float dt)
    {
        bool clientRequest = (bool)fsm.blackboard.Get("Sell_Request", typeof(bool));
        if(clientRequest)
        {
            timer += dt;
            if(timer > timeToSell)
            {
                Market.Item item = (Market.Item)fsm.blackboard.Get("Sell_Item", typeof(Market.Item));
                int amount = (int)fsm.blackboard.Get("Sell_ItemAmout", typeof(int));
                if(market.Sell(item, amount))
                {
                    fsm.blackboard.Set("Sell_Request", typeof(bool), false);
                    timer = 0;
                }
                else
                {
                    Debug.LogError("No client found!");
                }
            }
        }
    }
}
