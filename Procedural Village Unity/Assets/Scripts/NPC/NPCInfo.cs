using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInfo : MonoBehaviour
{
    // Variables
    House house = null;
    NPCBuilding workingPlace = null;
    NPCBuilding leisurePlace = null;

    // Getters
    public House GetHouse() { return house; }
    public NPCBuilding GetWorkPlace() { return workingPlace; }
    public NPCBuilding GetLeisurePlace() { return leisurePlace; }

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
}
