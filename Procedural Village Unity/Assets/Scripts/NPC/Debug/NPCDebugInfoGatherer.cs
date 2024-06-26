using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDebugInfoGatherer : MonoBehaviour
{
    string debugInfo;
    string id = "";

    NPCBehaviourSM npcSM = null;
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
        if(npcSM.GetStateMachine().HasChangedState() && id == NPCDebugManager.Instance.GetLastNPC())
        {
            SendInfo(true);
        } 
    }

    /// <summary>
    /// Manda la información del NPC apuntado al texto mostrado en pantalla
    /// </summary>
    /// <param name="update"> Decide si debe actualizarse el texto actual </param>
    public void SendInfo(bool update = false)
    {
        if(!npcSM) { Debug.LogError("NPC State Machine is missing on: " + gameObject.name + " GameObject."); return; }
        debugInfo = "";

        sM = npcSM.GetStateMachine();
        debugInfo = "Money: " + gameObject.GetComponent<NPCInfo>().GetMoney().ToString() + '\n' + "----------------\n\n";
        debugInfo += "GameObject: " + gameObject.name + '\n';
        debugInfo += "Father SM: " + sM.ID() + '\n';
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
        if(!update) NPCDebugManager.Instance.SetInfo(gameObject.name, debugInfo);
        else NPCDebugManager.Instance.UpdateInfo(gameObject.name, debugInfo);
    }

    /// <summary>
    /// Quita la información del NPC que se estaba mostrando
    /// </summary>
    public void HideInfo()
    {
        debugInfo = "";
        NPCDebugManager.Instance.HideInfo(gameObject.name);
    }
}
