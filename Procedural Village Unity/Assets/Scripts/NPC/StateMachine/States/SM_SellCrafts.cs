using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Máquina de estado de un carpintero que debe vender sus trabajos de artesanía en su tienda de tarde
/// </summary>
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
        Market market = info.GetWorkPlace() as Market;

        // En caso de que tenga que vender, se pone su horario de trabajo por la tarde y se añaden los items necesarios a la tienda
        if(sell != null && (bool)sell)
        {
            info.SetWorkingPeriod(SimulationManager.TimePeriods.AFTERNOON);
            object obj = fsm.blackboard.Get("Craft_ItemsCrafted", typeof(int));
            if(obj != null && market.GetMaxItemAmount(Market.Item.CRAFTS) <= 0) 
                market.AddItems(Market.Item.CRAFTS, (int)obj);
            SimulationManager.Instance.SetUpTemporalMarket(info.GetWorkPlace());
        }

        
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
