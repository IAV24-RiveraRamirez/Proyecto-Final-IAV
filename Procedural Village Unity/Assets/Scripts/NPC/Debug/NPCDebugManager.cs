using TMPro;
using UnityEngine;

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

    /// <summary>
    /// Nuevo texto a mostrar y quién quiere mostrarlo
    /// </summary>
    public void SetInfo(string gO, string text)
    {
        lastNPCShowingInfo = gO;
        if (shownInfo) 
            shownInfo.text = text;
        else 
            Debug.LogError("Text not found.");
    }

    /// <summary>
    /// Actualiza el texto actual si el objeto que quiere actualizar es el que está mostrando actualmente su información
    /// </summary>
    public void UpdateInfo(string gO, string text)
    {
        if (shownInfo)
        {
            if (lastNPCShowingInfo == gO && shownInfo.text != "") { shownInfo.text = text; }
        }
        else Debug.LogError("Text not found.");
    }

    /// <summary>
    /// Esconde el texto actual si el objeto que quiere actualizar es el que está mostrando actualmente su información
    /// </summary>
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
