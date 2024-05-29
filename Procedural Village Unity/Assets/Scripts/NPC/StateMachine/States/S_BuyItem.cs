using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class S_BuyItem : State
{
    float lastMoney = 0;
    NPCInfo info;
    Market.Item item;
    string itemNameOnBlackboard;

    public S_BuyItem(Market.Item item, string itemNameOnBlackboard)
    {
        this.item = item;
        this.itemNameOnBlackboard = itemNameOnBlackboard;
    }

    public override void Enter()
    {
        info = gameObject.GetComponent<NPCInfo>();
        gameObject.GetComponent<NavMeshAgent>().isStopped = true;

        Market market = info.GetMarketPlace() as Market;
        market.AddNPCToQueue(info, item, (int)fsm.blackboard.Get(itemNameOnBlackboard, typeof(int)), Market.Request.RequestType.BUY);
        lastMoney = info.GetMoney();
    }

    public override void Exit()
    {
        gameObject.GetComponent<NavMeshAgent>().isStopped = false;
    }

    public override string ID()
    {
        return "Buying " + item.ToString().ToLower();
    }

    public override void Update(float dt)
    {
        if (lastMoney > info.GetMoney())
        {
            fsm.blackboard.Set(item.ToString(), typeof(int), (int)fsm.blackboard.Get(itemNameOnBlackboard, typeof(int)));
            fsm.blackboard.Set(itemNameOnBlackboard, typeof(int), 0);
        }
    }
}
