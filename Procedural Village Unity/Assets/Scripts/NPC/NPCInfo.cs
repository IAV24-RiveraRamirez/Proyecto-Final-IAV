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

    Market.BuyRequestOutput lastBuyingTry = Market.BuyRequestOutput.ALL_OK;

    SimulationManager.TimePeriods workingPeriod;

    public bool ChangeMoney(float amount) { 
        money += amount;
        if (money < 0)
        {
            money -= amount;
            return false;
        }
        else { return true; } 
    }

    private void Update()
    {
        Debug.Log(gameObject.name + " " + workingPeriod.ToString());
    }

    // Getters
    public Market.BuyRequestOutput GetLastBuyResult() { return lastBuyingTry; }
    public float GetMoney() { return money; }
    public House GetHouse() { return house; }
    public NPCBuilding GetWorkPlace() { return workingPlace; }
    public NPCBuilding GetLeisurePlace() { return leisurePlace; }
    public NPCBuilding GetMarketPlace() { return marketPlace; }
    public SimulationManager.TimePeriods GetWorkingPeriod() {  return workingPeriod; }

    // Setters
    public void SetHouse(House house) { this.house = house; }
    public void SetLastBuyResult(Market.BuyRequestOutput lastBuyingTry) { this.lastBuyingTry = lastBuyingTry; }
    public void SetWorkPlace(NPCBuilding workingPlace) { this.workingPlace = workingPlace; }
    public void SetLeisurePlace(NPCBuilding leisurePlace) { this.leisurePlace = leisurePlace; }
    public void SetMarketPlace(NPCBuilding marketplace) { this.marketPlace = marketplace; }
    public void SetWorkingPeriod(SimulationManager.TimePeriods workingPeriod) {  this.workingPeriod = workingPeriod; }
}
