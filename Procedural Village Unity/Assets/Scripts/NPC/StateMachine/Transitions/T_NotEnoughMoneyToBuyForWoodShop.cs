using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Indica cuándo un carpintero no ha tenido suficiente dinero para comprar madera
/// </summary>
public class T_NotEnoughMoneyToBuyForWoodShop : T_NotEnoughMoney
{
    public T_NotEnoughMoneyToBuyForWoodShop(State nextState) : base(nextState)
    {
    }

    public override void Exit()
    {
        base.Exit();
        info.AddToSavings(info.GetLastMoneyNeeded());
        fsm.blackboard.Set("Craft_SellCrafts", typeof(bool), true);
    }

    public override string ID()
    {
        return "Not enough money to buy wood";
    }
}
