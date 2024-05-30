using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class NPCInfo : MonoBehaviour
{
    // Variables
    House house = null;
    NPCBuilding workingPlace = null;
    NPCBuilding leisurePlace = null;
    NPCBuilding marketPlace = null;

    float money;
    float savings = 0;
    float lastMoneyNeeded = 0;

    Market.BuyRequestOutput lastBuyingTry = Market.BuyRequestOutput.RESTING_STATE;

    SimulationManager.TimePeriods workingPeriod;

    public bool ChangeMoney(float amount) { 
        money += amount;
        if (money < savings)
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
    public void AddToSavings(float savings) { this.savings += savings; if (this.savings < 0) this.savings = 0; }
    public void SetHouse(House house) { this.house = house; }
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
