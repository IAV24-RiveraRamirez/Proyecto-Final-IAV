using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStateMachine_1 : StateMachine
{
    public override string ID()
    {
        return "Test_SM";
    }

    public override void Init(GameObject g, StateMachine fsm)
    {
        base.Init(g, fsm);

        TestState_1 state1 = new TestState_1();
        TestState_2 state2 = new TestState_2();

        Transition transition1 = new T_RightArrow(state2);
        Transition transition2 = new T_LeftArrow(state1);

        state1.AddTransition(transition1);
        state2.AddTransition(transition2);

        AddState(state1).AddState(state2);

        StartMachine(state1);
    }
}
