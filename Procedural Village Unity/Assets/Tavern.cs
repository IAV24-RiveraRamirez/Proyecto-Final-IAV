using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tavern : LeisurePlace
{
    float timer = 0;
    public override void WhileHavingFun(NPCInfo info)
    {
        timer += Time.deltaTime;
        if (timer > 0.75f) {
            timer = 0;
            if (!info.ChangeMoney(-10))
            {
                info.SetLastBuyResult(Market.BuyRequestOutput.CLIENT_HAS_NO_MONEY);
            }
        }
    }
}
