using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetNPCInfo : MonoBehaviour
{
    [SerializeField] LayerMask debugLayerMask;

    GameObject lastNPC = null;

    // Update is called once per frame
    void Update()
    {
        RaycastHit info;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out info, 10000.0f, debugLayerMask))
        {
            lastNPC = info.transform.gameObject;
            lastNPC.GetComponent<NPCDebugInfoGatherer>().SendInfo();
        }
        else if(lastNPC != null) NPCDebugManager.Instance.HideInfo(lastNPC.name);
    }
}
