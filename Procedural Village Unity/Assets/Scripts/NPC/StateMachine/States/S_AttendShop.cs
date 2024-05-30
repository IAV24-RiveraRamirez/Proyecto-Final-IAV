using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class S_AttendShop : State
{
    Market market = null;

    float timer;
    float timeToProcessRequest = 1.5f;
    public override void Enter()
    {
        if(fsm.blackboard.Get("Sell_Request", typeof(bool)) == null)
        {
            fsm.blackboard.Set("Sell_Request", typeof(bool), false);
        }
        if (fsm.blackboard.Get("Buy_Request", typeof(bool)) == null)
        {
            fsm.blackboard.Set("Buy_Request", typeof(bool), false);
        }
        NPCInfo info = gameObject.GetComponent<NPCInfo>();

        market = info.GetWorkPlace() as Market;
        market.SetNPCAttending(info);
    }

    public override void Exit()
    {
        market.SetNPCAttending(null);
        SimulationManager.Instance.RemoveTemporalMarket(market);
    }

    public override string ID()
    {
        return "Attending Shop!";
    }

    public override void Update(float dt)
    {
        bool sellRequest = (bool)fsm.blackboard.Get("Sell_Request", typeof(bool));
        bool buyRequest = (bool)fsm.blackboard.Get("Buy_Request", typeof(bool));
        if(sellRequest)
        {
            timer += dt;
            if(timer > timeToProcessRequest)
            {
                Market.Item item = (Market.Item)fsm.blackboard.Get("Sell_Item", typeof(Market.Item));
                int amount = (int)fsm.blackboard.Get("Sell_ItemAmout", typeof(int));
                if(market.GiveMoneyToClient(item, amount))
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
        else if (buyRequest)
        {
            timer += dt;
            if (timer > timeToProcessRequest)
            {
                Market.Item item = (Market.Item)fsm.blackboard.Get("Buy_Item", typeof(Market.Item));
                int amount = (int)fsm.blackboard.Get("Buy_ItemAmout", typeof(int));
                Market.BuyRequestOutput result = market.GetMoneyFromClient(item, amount);
                if (result == Market.BuyRequestOutput.ITEM_BOUGHT)
                {
                    Debug.Log("Se ha comprado: " + amount.ToString() + " de " + item.ToString());
                    timer = 0;
                }
                else
                {
                    timer = 0;
                    Debug.Log(result.ToString());
                }
                fsm.blackboard.Set("Buy_Request", typeof(bool), false);
            }
        }
    }
}
