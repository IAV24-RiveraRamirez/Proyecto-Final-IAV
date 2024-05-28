using System.Collections;
using System.Collections.Generic;
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

    public void ChangeMoney(float amount) { money += amount; }

    // Getters
    public float GetMoney() { return money; }
    public House GetHouse() { return house; }
    public NPCBuilding GetWorkPlace() { return workingPlace; }
    public NPCBuilding GetLeisurePlace() { return leisurePlace; }
    public NPCBuilding GetMarketPlace() { return marketPlace; }

    // Setters
    public void SetHouse(House house)
    {
        this.house = house;
    }

    public void SetWorkPlace(NPCBuilding workingPlace)
    {
        this.workingPlace = workingPlace;
    }

    public void SetLeisurePlace(NPCBuilding leisurePlace)
    {
        this.leisurePlace = leisurePlace;
    }

    public void SetMarketPlace(NPCBuilding marketplace)
    {
        this.marketPlace = marketplace;
    }
}
