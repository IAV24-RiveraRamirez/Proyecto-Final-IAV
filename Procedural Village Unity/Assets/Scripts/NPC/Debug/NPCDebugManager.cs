using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class NPCDebugManager : MonoBehaviour
{
    private static NPCDebugManager instance = null;
    public static NPCDebugManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else Destroy(this);
    }

    private string lastNPCShowingInfo = "";

    [SerializeField] TextMeshProUGUI shownInfo = null;

    // Start is called before the first frame update
    void Start()
    {
        if(!shownInfo) { shownInfo = GetComponentInChildren<TextMeshProUGUI>(); }
    }

    public void SetInfo(string gO, string text)
    {
        lastNPCShowingInfo = gO;
        if (shownInfo) 
            shownInfo.text = text;
        else 
            Debug.LogError("Text not found.");
    }

    public void UpdateInfo(string gO, string text)
    {
        if (shownInfo)
        {
            if (lastNPCShowingInfo == gO && shownInfo.text != "") { shownInfo.text = text; }
        }
        else Debug.LogError("Text not found.");
    }

    public void HideInfo(string gO)
    {
        if(lastNPCShowingInfo == gO) {
            if (shownInfo) 
                shownInfo.text = "";
            else 
                Debug.LogError("Text not found.");
        }
    }

    public string GetLastNPC() { return lastNPCShowingInfo; }

}
