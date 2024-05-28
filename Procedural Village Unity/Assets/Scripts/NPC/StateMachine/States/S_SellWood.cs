using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class S_SellWood : State
{
    float lastMoney = 0;
    NPCInfo info;
    
    public override void Enter()
    {
        info = gameObject.GetComponent<NPCInfo>();
        gameObject.GetComponent<NavMeshAgent>().isStopped = true;

        Market market = info.GetMarketPlace() as Market;
        market.AddNPCToQueue(info, Market.Item.WOOD, (int)fsm.blackboard.Get("Wood", typeof(int)));
        lastMoney = info.GetMoney();
    }

    public override void Exit()
    {
        gameObject.GetComponent<NavMeshAgent>().isStopped = false;
    }

    public override string ID()
    {
        return "Selling Wood";
    }

    public override void Update(float dt)
    {
        if(lastMoney < info.GetMoney())
        {
            fsm.blackboard.Set("Wood", typeof(int), null);
        }
    }
}
