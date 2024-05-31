using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Indica cu�ndo una carpinter�a tiene madera disponible
/// </summary>
public class T_WoodShopRefilled : Transition
{
    WoodShop shop;
    public T_WoodShopRefilled(State nextState) : base(nextState)
    {
    }

    public override bool Check()
    {
        return shop.WoodAvaliable();
    }

    public override void Enter()
    {
        shop = gameObject.GetComponent<NPCInfo>().GetWorkPlace() as WoodShop;
    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Shop is refilled";
    }
}
