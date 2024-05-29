using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_SellCrafts : StateMachine
{
    public override string ID()
    {
        return "Selling Crafts";
    }

    public override void Init(GameObject g, StateMachine fsm)
    {
        base.Init(g, fsm);

        object sell = fsm.blackboard.Get("Craft_SellCrafts", typeof(bool));

        NPCInfo info = g.GetComponent<NPCInfo>();
        if(sell != null && (bool)sell) 
            info.SetWorkingPeriod(SimulationManager.TimePeriods.AFTERNOON);

        SimulationManager.Instance.SetUpTemporalMarket(info.GetWorkPlace());

        Market market = info.GetWorkPlace() as Market;
        object obj = fsm.blackboard.Get("Craft_ItemsCrafted", typeof(int));
        if(obj != null) market.AddItems(Market.Item.CRAFTS, (int)obj);
        
        State state1 = new S_ToFixedDestination(info.GetWorkPlace().gameObject);
        State state2 = new S_AttendShop();

        state1.AddTransition(new T_ReachDestination(state2, 1.0f));

        AddState(state1).AddState(state2);

        StartMachine(state1);
    }

    public override void Exit()
    {
        base.Exit();
        SimulationManager.Instance.RemoveTemporalMarket(gameObject.GetComponent<NPCInfo>().GetWorkPlace());
    }
}
