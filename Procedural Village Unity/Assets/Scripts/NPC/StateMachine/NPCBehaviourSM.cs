using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.VisualScripting;
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

        State state1 = new SM_Sleep();

        State state2 = null;

        NPCBuilding building = gameObject.GetComponent<NPCInfo>().GetWorkPlace();
        if (building is Sawmill) state2 = new SM_SawmillWorker();
        else if(building is WoodShop) state2 = new SM_Carpenter();
        else if (building is Market) state2 = new SM_MerchantBuyer();

        State state3 = new SM_Leisure();

        state1.AddTransition(new T_HasToWork(state2));
        state1.AddTransition(new T_LeisureTimeArrived(state3));

        state2.AddTransition(new T_LeisureTimeArrived(state3));
        state2.AddTransition(new T_WorkDayEnded(state3));
        state2.AddTransition(new T_Evening(state1));

        state3.AddTransition(new T_HasToWork(state2));
        state3.AddTransition(new T_Evening(state1));

        AddState(state1).AddState(state2).AddState(state3);

        SimulationManager mngr = SimulationManager.Instance;
        switch (mngr.GetCurrentPeriod())
        {
            case SimulationManager.TimePeriods.MORNING:
            {
                StartMachine(state1);
                break;
            }
            case SimulationManager.TimePeriods.AFTERNOON:
            {
                StartMachine(state2);
                break;
            }
            case SimulationManager.TimePeriods.EVENING:
            {
                StartMachine(state3);
                break;
            }
        }
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
