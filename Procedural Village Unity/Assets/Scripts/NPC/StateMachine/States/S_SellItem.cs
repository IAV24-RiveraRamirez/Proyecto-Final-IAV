using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Un NPC intenta vender un Item a un mercader
/// Solo lo consigue si ve aumentado su dinero durante este estado
/// </summary>
public class S_SellItem : State
{
    float lastMoney = 0;
    NPCInfo info;
    Market.Item item;
    string itemNameOnBlacboard;

    public S_SellItem(Market.Item item, string itemNameOnBlacboard)
    {
        this.item = item;
        this.itemNameOnBlacboard = itemNameOnBlacboard;
    }

    public override void Enter()
    {
        info = gameObject.GetComponent<NPCInfo>();
        gameObject.GetComponent<NavMeshAgent>().isStopped = true;

        Market market = info.GetMarketPlace() as Market;
        market.MakeRequest(info, item, (int)fsm.blackboard.Get(itemNameOnBlacboard, typeof(int)), Market.Request.RequestType.SELL);
        lastMoney = info.GetMoney();
    }

    public override void Exit()
    {
        gameObject.GetComponent<NavMeshAgent>().isStopped = false;
    }

    public override string ID()
    {
        return "Selling " + item.ToString().ToLower();
    }

    public override void Update(float dt)
    {
        if(lastMoney < info.GetMoney())
        {
            fsm.blackboard.Set(itemNameOnBlacboard, typeof(int), 0);
        }
    }
}
