using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_ZeroUnitsOfItem : Transition
{
    string itemNameOnBlackboard;
    public T_ZeroUnitsOfItem(State nextState, string itemNameOnBlackboard) : base(nextState)
    {
        this.itemNameOnBlackboard = itemNameOnBlackboard;
    }

    public override bool Check()
    {
        return (int)fsm.blackboard.Get(itemNameOnBlackboard, typeof(int)) == 0;
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override string ID()
    {
        return itemNameOnBlackboard + " is zero";
    }
}
