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
        return (bool)fsm.blackboard.Get("Craft_GoRefill", typeof(bool));
    }

    public override void Enter()
    {
        if((bool)fsm.blackboard.Get("Craft_GoRefill", typeof(bool))) {
            fsm.blackboard.Set("Craft_GoRefill", typeof(bool), false);
        }
    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return "Wood shop out of wood";
    }
}
