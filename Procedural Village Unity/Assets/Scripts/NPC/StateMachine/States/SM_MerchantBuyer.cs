using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_MerchantBuyer : StateMachine
{
    public override string ID()
    {
        return "On Shop to Buy";
    }

    public override void Init(GameObject g, StateMachine fsm)
    {
        base.Init(g, fsm);

        NPCInfo info = gameObject.GetComponent<NPCInfo>();
        NPCBuilding market = info.GetMarketPlace();

        State state1 = new S_ToFixedDestination(market.gameObject);
        State state2 = new S_BuyGoods();

        state1.AddTransition(new T_ReachDestination(state2, 0.2f));

        AddState(state1).AddState(state2);

        StartMachine(state1);
    }
}
