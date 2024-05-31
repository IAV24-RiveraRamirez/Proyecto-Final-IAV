using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Clase encargada de guardar toda la información relativa a un NPC
/// </summary>
public class NPCInfo : MonoBehaviour
{
    // Variables
    /// <summary>
    /// Referencia a su casa
    /// </summary>
    House house = null;
    /// <summary>
    /// Referencia a su lugar de trabajo
    /// </summary>
    NPCBuilding workingPlace = null;
    /// <summary>
    /// Referencia a un lugar donde hacer Ocio. Si no se está en estado de Ocio, esta referencia es nula
    /// </summary>
    NPCBuilding leisurePlace = null;
    /// <summary>
    /// Referencia a un mercado donde comprar/vender los materiales de su trabajo
    /// </summary>
    NPCBuilding marketPlace = null;

    /// <summary>
    /// Dinero disponible
    /// </summary>
    float money;
    /// <summary>
    /// Mínimo de dinero que debe tener un NPC. 
    /// </summary>
    float savings = 0;
    /// <summary>
    /// La última cantidad de dinero que era necesaria para comprar algo.
    /// </summary>
    float lastMoneyNeeded = 0;

    /// <summary>
    /// Output de la última operación de compra en un mercado
    /// </summary>
    Market.BuyRequestOutput lastBuyingTry = Market.BuyRequestOutput.RESTING_STATE;

    /// <summary>
    /// Periodo del día en el que trabaja el NPC
    /// </summary>
    SimulationManager.TimePeriods workingPeriod;

    /// <summary>
    /// Método que cambia el dinero del NPC según una cantidad.
    /// En caso de que el dinero está por debajo de los ahorros (savings), no se realiza ningún cambio 
    /// y se devuelve false.
    /// </summary>
    /// <param name="amount"> Cantidad a añadir </param>
    /// <param name="useSavings"> Marca si se usarán los ahorros para esta compra </param>
    /// <returns> False si no se ha podido añadir la cantidad | True si se ha podido añadir la cantidad </returns>
    public bool ChangeMoney(float amount, bool useSavings = false) {
        if (useSavings) savings = 0;
        money += amount;
        if (amount < 0 && money < savings)
        {
            money -= amount;
            return false;
        }
        else { return true; } 
    }

    // Getters
    public Market.BuyRequestOutput GetLastBuyResult() { return lastBuyingTry; }
    public float GetMoney() { return money; }
    public float GetLastMoneyNeeded() { return lastMoneyNeeded; }
    public House GetHouse() { return house; }
    public NPCBuilding GetWorkPlace() { return workingPlace; }
    public NPCBuilding GetLeisurePlace() { return leisurePlace; }
    public NPCBuilding GetMarketPlace() { return marketPlace; }
    public SimulationManager.TimePeriods GetWorkingPeriod() {  return workingPeriod; }

    // Setters
    /// <summary>
    /// Añadir una cantidad concreta a los ahorros necesarios
    /// </summary>
    /// <param name="savings"> Cantidad a añadir </param>
    public void AddToSavings(float savings) { this.savings += savings; if (this.savings < 0) this.savings = 0; }
    public void SetHouse(House house) { this.house = house; }
    /// <summary>
    /// Da valor a la variable lastBuyingTry.
    /// </summary>
    /// <param name="lastBuyingTry"> Valor nuevo </param>
    /// <param name="moneyNeeded"> En caso de que el nuevo valor sea "CLIENT_HAS_NO_MONEY" se le puede pasar 
    /// el dinero necesitado para que la transacción fuera posible. De esta forma lastMoneyNeeded se iguala a moneyNeeded</param>
    public void SetLastBuyResult(Market.BuyRequestOutput lastBuyingTry, float moneyNeeded = 0) { 
        this.lastBuyingTry = lastBuyingTry; 
        
        if(lastBuyingTry == Market.BuyRequestOutput.CLIENT_HAS_NO_MONEY)
        {
            lastMoneyNeeded = moneyNeeded;
        }

    }
    public void SetWorkPlace(NPCBuilding workingPlace) { this.workingPlace = workingPlace; }
    public void SetLeisurePlace(NPCBuilding leisurePlace) { this.leisurePlace = leisurePlace; }
    public void SetMarketPlace(NPCBuilding marketplace) { this.marketPlace = marketplace; }
    public void SetWorkingPeriod(SimulationManager.TimePeriods workingPeriod) {  this.workingPeriod = workingPeriod; }
}
