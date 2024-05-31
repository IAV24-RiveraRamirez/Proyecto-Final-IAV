using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Máquina de estados para un mercader de mañana, que compra recursos a los clientes
/// </summary>
public class SM_MerchantBuyer : SM_Work
{
    public override string ID()
    {
        return "On Shop to Buy";
    }

    public override void Init(GameObject g, StateMachine fsm)
    {
        base.Init(g, fsm);

        NPCInfo info = gameObject.GetComponent<NPCInfo>();
        info.SetWorkingPeriod(SimulationManager.TimePeriods.MORNING);

        NPCBuilding market = info.GetMarketPlace();

        State state1 = new S_ToFixedDestination(market.gameObject);
        State state2 = new S_AttendShop();

        state1.AddTransition(new T_ReachDestination(state2, 0.2f));

        AddState(state1).AddState(state2);

        StartMachine(state1);
    }
}
