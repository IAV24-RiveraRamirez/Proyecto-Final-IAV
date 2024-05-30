using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class LeisurePlace : NPCBuilding
{
    // Parameters
    [SerializeField] float timeToEnd = 1.5f;

    // Variables
    Dictionary<NPCInfo, float> timeSpentByNPC = new Dictionary<NPCInfo, float>();

    protected override void Start()
    {
        type = BuildingType.LEISURE;
        base.Start();
    }

    public abstract void WhileHavingFun(NPCInfo info);

    public bool HaveFun(NPCInfo info)
    {
        if(!timeSpentByNPC.ContainsKey(info))
        {
            timeSpentByNPC.Add(info, 0);
        }

        timeSpentByNPC[info] += Time.deltaTime;
        
        WhileHavingFun(info);
        bool ended = timeSpentByNPC[info] >= timeToEnd;
        if(ended) { timeSpentByNPC.Remove(info); }
        return ended;
    }
}
