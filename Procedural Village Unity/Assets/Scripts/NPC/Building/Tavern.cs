using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Representa una taberna en la simulación
/// </summary>
public class Tavern : LeisurePlace
{
    float timer = 0;
    /// <summary>
    /// Dinero gastado cada X tiempo en la taberna
    /// </summary>
    [SerializeField] int moneySpent = 2;
    /// <summary>
    /// Tiempo que tarda la taberna en cobrar moneySpent a un cliente
    /// </summary>
    [SerializeField] float timeToSpendMoney = 0.5f;

    /// <summary>
    /// Hace pagar al cliente una cierta cantidad de dinero cada X tiempo
    /// </summary>
    /// <param name="info"> Cliente </param>
    public override void WhileHavingFun(NPCInfo info)
    {
        timer += Time.deltaTime;
        if (timer > timeToSpendMoney) {
            timer = 0;
            if (!info.ChangeMoney(-moneySpent))
            {
                info.SetLastBuyResult(Market.BuyRequestOutput.CLIENT_HAS_NO_MONEY);
            }
        }
    }
}
