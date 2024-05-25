using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class NPCBaseSM : StateMachine
{
    public override string ID()
    {
        return "FatherNPC_SM";
    }
    public override void Init(GameObject g, StateMachine fsm)
    {
        base.Init(g, fsm);

        TestState_1 state1 = new TestState_1();
        TestState_2 state2 = new TestState_2();
        TestStateMachine_1 state3 = new TestStateMachine_1();

        state1.AddTransition(new T_Morning(state2));
        state2.AddTransition(new T_Afternoon(state3));
        state3.AddTransition(new T_Evening(state1));

        AddState(state1).AddState(state2).AddState(state3);

        StartMachine(state1);
        
    }
}

public class NPCBehaviourSM : MonoBehaviour
{
    StateMachine sM = null;
    // Start is called before the first frame update
    void Start()
    {
        InitializeStateMachine();
    }

    void InitializeStateMachine()
    {
        sM = new NPCBaseSM();
        sM.Init(gameObject, sM);
    }

    // Update is called once per frame
    void Update()
    {
        sM.Update(Time.deltaTime);
    }

    public StateMachine GetStateMachine() { return sM; }
}
