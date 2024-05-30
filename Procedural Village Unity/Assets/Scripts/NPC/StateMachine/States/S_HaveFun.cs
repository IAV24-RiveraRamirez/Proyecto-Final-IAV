using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_HaveFun : State
{
    LeisurePlace place;
    NPCInfo info;

    public override void Enter()
    {
        info = gameObject.GetComponent<NPCInfo>();
        place = info.GetLeisurePlace() as LeisurePlace;
    }

    public override void Exit()
    {
        place = null;
        info.SetLeisurePlace(null);
    }

    public override string ID()
    {
        return "Having fun at " + place.gameObject.name + "!!";
    }

    public override void Update(float dt)
    {
        if(place != null && place.HaveFun(info))
        {
            place = null;
            info.SetLeisurePlace(null);
        }
    }
}
