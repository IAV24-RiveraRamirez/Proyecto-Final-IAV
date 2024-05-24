using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDebugInfoGatherer : MonoBehaviour
{
    string debugInfo;
    string id = "";

    [SerializeField] NPCBehaviourSM npcSM = null;
    StateMachine sM;
    // Start is called before the first frame update
    void Start()
    {
        npcSM = GetComponent<NPCBehaviourSM>();
        if(!npcSM) { Debug.LogError("NPC State Machine is missing on: " + gameObject.name + " GameObject."); return; }
        sM = npcSM.GetStateMachine();
        id = gameObject.name;
    }

    private void Update()
    {
        if(sM.HasChangedState() && id == NPCDebugManager.Instance.GetLastNPC())
        {
            SendInfo();
        } 
    }

    public void SendInfo()
    {
        if(!npcSM) { Debug.LogError("NPC State Machine is missing on: " + gameObject.name + " GameObject."); return; }
        debugInfo = "";

        sM = npcSM.GetStateMachine();
        debugInfo = "Father SM: " + sM.ID() + '\n';
        debugInfo += "Last Transition: " + sM.GetLastTransitionID() + '\n';

        State current = sM.GetActiveState();
        
        bool isSM = current is StateMachine;
        
        while(isSM)
        {
            StateMachine currentSM = current as StateMachine;

            debugInfo += "Active SM: " + current.ID() + '\n';
            debugInfo += "Last Transition: " + currentSM.GetLastTransitionID() + '\n';

            current = currentSM.GetActiveState();

            isSM = current is StateMachine;
        }
        debugInfo += "Active State: " + current.ID();
        NPCDebugManager.Instance.SetInfo(gameObject.name, debugInfo);
    }

    public void HideInfo()
    {
        debugInfo = "";
        NPCDebugManager.Instance.HideInfo(gameObject.name);
    }
}
