using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_WoodShopOutOfWood : Transition
{
    public T_WoodShopOutOfWood(State nextState) : base(nextState)
    {
    }

    public override bool Check()
    {
        return (bool)fsm.blackboard.Get("Craft_GoRefill", typeof(bool)) &&
            !((int)fsm.blackboard.Get(Market.Item.WOOD.ToString(), typeof(int)) > 0);
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Wood shop out of wood";
    }
}
