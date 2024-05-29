using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_RefillWoodShop : State
{
    public override void Enter()
    {
        WoodShop shop = gameObject.GetComponent<NPCInfo>().GetWorkPlace() as WoodShop;
        shop.RefillShop((int)fsm.blackboard.Get(Market.Item.WOOD.ToString(), typeof(int)));
        fsm.blackboard.Set(Market.Item.WOOD.ToString(), typeof(int), 0);
    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Refilling Shop";
    }

    public override void Update(float dt)
    {

    }
}
