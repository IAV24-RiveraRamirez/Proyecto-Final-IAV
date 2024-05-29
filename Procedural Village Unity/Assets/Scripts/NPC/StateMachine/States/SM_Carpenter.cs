using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_Carpenter : SM_Work
{
    public override string ID()
    {
        return "Working on Wood Shop";
    }

    public override void Init(GameObject g, StateMachine fsm)
    {
        base.Init(g, fsm);

        object sell = fsm.blackboard.Get("Craft_SellCrafts", typeof(bool));

        NPCInfo info = g.GetComponent<NPCInfo>();
        if (sell == null || !(bool)sell)
            info.SetWorkingPeriod(SimulationManager.TimePeriods.MORNING);

        State state1 = new S_ToFixedDestination(info.GetWorkPlace().gameObject);
        State state2 = new S_Crafting();
        State state3 = new S_ToFixedDestination(info.GetMarketPlace().gameObject);
        State state4 = new S_BuyItem(Market.Item.WOOD, "Craft_WoodAmount");
        State state5 = new SM_SellCrafts();
        State state6 = new S_RefillWoodShop();

        State stopWorking = new S_StopWorking();

        state1.AddTransition(new T_ReachDestination(state2, 3.0f));

        state2.AddTransition(new T_HasToRefillWoodShop(state6));
        state2.AddTransition(new T_WoodShopOutOfWood(state3));

        state3.AddTransition(new T_ReachDestination(state4, 3.0f));

        state4.AddTransition(new T_ZeroUnitsOfItem(state1, "Craft_WoodAmount"));
        state4.AddTransition(new T_NotEnoughMoneyToBuyForWoodShop(state5));
        state4.AddTransition(new T_ShopHasNoItem(stopWorking));

        state5.AddTransition(new T_RunOutOfCraftsToSell(stopWorking));

        state6.AddTransition(new T_WoodShopRefilled(state2));

        AddState(state1).AddState(state2).AddState(state3).AddState(state4).AddState(state5).AddState(state6);

        object obj = fsm.blackboard.Get("Craft_SellCrafts", typeof(bool));
        if (obj != null && (bool)obj == true) StartMachine(state5);
        else StartMachine(state1);

    }
}
