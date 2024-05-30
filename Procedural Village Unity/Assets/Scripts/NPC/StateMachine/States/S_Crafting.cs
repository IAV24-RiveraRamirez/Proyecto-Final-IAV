using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Crafting : State
{
    NPCInfo info;
    WoodShop.CraftingProgress progress;
    WoodShop woodShop;
    public override void Enter()
    {
        object obj = fsm.blackboard.Get("Craft_ItemsCrafted", typeof(int));
        if(obj == null)
        {
            fsm.blackboard.Set("Craft_ItemsCrafted", typeof(int), 0);
        }
        info = gameObject.GetComponent<NPCInfo>();
        woodShop = info.GetWorkPlace() as WoodShop;
        GetWork();
    }

    void GetWork()
    {
        progress = woodShop.Work(info);
        if(progress == null && !(bool)fsm.blackboard.Get("Craft_GoRefill", typeof(bool)))
        {
            bool goToRefill = woodShop.LeaveShopToRefill();
            if(goToRefill)
            {
                fsm.blackboard.Set("Craft_GoRefill", typeof(bool), true);
                fsm.blackboard.Set("Craft_WoodAmount", typeof(int), woodShop.GetMaxWood());
            }
            else
            {
                fsm.blackboard.Set("WorkDayEnded", typeof(bool), true);
            }
        }
    }

    public override void Exit()
    {
        woodShop.StopWorking(info, progress);
    }

    public override string ID()
    {
        return "Crafting";
    }
    
    public override void Update(float dt)
    {
        if (progress == null) return;

        if (!progress.IsCompleted())
        {
            progress.MakeProgress(dt);
        }
        else if (woodShop.CraftCompleted(info, progress))
        {
            object obj = fsm.blackboard.Get("Craft_ItemsCrafted", typeof(int));
            if(obj == null)
            {
                fsm.blackboard.Set("Craft_ItemsCrafted", typeof(int), 1);
            }
            else
            {
                int numCrafts = (int)obj;
                fsm.blackboard.Set("Craft_ItemsCrafted", typeof(int), numCrafts + 1);
            }
            GetWork();
        }
        else
        {
            Debug.Log("A fatal error ocurred while completing craft on NPC '" + gameObject.name + "'");
        }
    }
}
