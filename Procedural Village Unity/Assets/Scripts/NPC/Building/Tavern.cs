using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tavern : LeisurePlace
{
    float timer = 0;
    [SerializeField] int moneySpent = 2;
    public override void WhileHavingFun(NPCInfo info)
    {
        timer += Time.deltaTime;
        if (timer > 0.75f) {
            timer = 0;
            if (!info.ChangeMoney(-moneySpent))
            {
                info.SetLastBuyResult(Market.BuyRequestOutput.CLIENT_HAS_NO_MONEY);
            }
        }
    }
}
