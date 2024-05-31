using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    #region Singleton
    // Singleton
    private static SimulationManager instance = null;
    public static SimulationManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else Destroy(this);
    }
    #endregion

    // Structures
    [System.Serializable]
    struct Day
    {
        [Range(0, 24)]
        [SerializeField] int morning;
        [Range(0, 24)]
        [SerializeField] int afternoon;
        [Range(0, 24)]
        [SerializeField] int evening;

        [HideInInspector]
        public int[] timePeriods;

        public Day(Day _day)
        {
            morning = _day.morning;
            afternoon = _day.afternoon;
            evening = _day.evening;

            timePeriods = new int[3];
            timePeriods[0] = morning;
            timePeriods[1] = afternoon;
            timePeriods[2] = evening;
        }

        public Day(int m, int a, int e)
        {
            morning = m;
            afternoon = a;
            evening = e;

            timePeriods = new int[3];
            timePeriods[0] = morning;
            timePeriods[1] = afternoon;
            timePeriods[2] = evening;
        }
    }

    // Enums
    public enum TimePeriods { MORNING, AFTERNOON, EVENING }

    // References 
    [SerializeField] GameObject npcPrefab = null;

    // Parameters
    [Tooltip("How fast time goes by")]
    [SerializeField] float timeRate = 0.5f;
    [SerializeField, Range(0, 24)] float timeOfDay;

    [Tooltip("Time when morning, afternoon and evening start (Range 0-23)")]
    [SerializeField] Day squedule;

    private void OnValidate()
    {
        time = timeOfDay;

        squedule = new Day(squedule);
    }

    // Constants
    public const int HOURS_IN_DAY = 24;

    // Variables
    private float time;

    TimePeriods currentPeriod = TimePeriods.MORNING;

    Dictionary<NPCBuilding.BuildingType, List<NPCBuilding>> npcBuildings = new Dictionary<NPCBuilding.BuildingType, List<NPCBuilding>>();

    List<NPCBuilding> avaliableWorkingPlaces = new List<NPCBuilding>();
    List<NPCBuilding> avaliableLeisurePlaces = new List<NPCBuilding>();

    // Getters
    public float GetTime() { return time; }
    public float GetTimePercent() { return time / HOURS_IN_DAY; }
    public TimePeriods GetCurrentPeriod() { return currentPeriod; }
    public GameObject GetNPCPrefab() { return npcPrefab; }

    // Own Methods
    void HandleTime()
    {
        time += Time.deltaTime * timeRate;
        time %= HOURS_IN_DAY;

        timeOfDay = time;
        if(timeOfDay >= squedule.timePeriods[(int)TimePeriods.MORNING] && timeOfDay < squedule.timePeriods[(int)TimePeriods.AFTERNOON])
        {
            currentPeriod = TimePeriods.MORNING;
            UIManager.Instance.SetTimePeriod(currentPeriod);
        }
        else if (timeOfDay >= squedule.timePeriods[(int)TimePeriods.AFTERNOON] && timeOfDay < squedule.timePeriods[(int)TimePeriods.EVENING])
        {
            currentPeriod = TimePeriods.AFTERNOON;
            UIManager.Instance.SetTimePeriod(currentPeriod);
        }
        else
        {
            currentPeriod = TimePeriods.EVENING;
            UIManager.Instance.SetTimePeriod(currentPeriod);
        }
    }

    public void SetUpTemporalMarket(NPCBuilding market)
    {
        avaliableLeisurePlaces.Add(market);
    }

    public void RemoveTemporalMarket(NPCBuilding market) 
    {
        avaliableLeisurePlaces.Remove(market);
    }

    public void AddBuilding(NPCBuilding building)
    {
        NPCBuilding.BuildingType type = building.GetBuildingType();

        if(!npcBuildings.ContainsKey(type))
        {
            npcBuildings.Add(type, new List<NPCBuilding>());
        }

        if (type == NPCBuilding.BuildingType.WORK || type == NPCBuilding.BuildingType.MARKET) avaliableWorkingPlaces.Add(building);
        else if (type == NPCBuilding.BuildingType.LEISURE) avaliableLeisurePlaces.Add(building);

        npcBuildings[type].Add(building);
    }

    public void RemoveBuilding(NPCBuilding building)
    {
        npcBuildings[building.GetBuildingType()].Remove(building);
        if(building.GetBuildingType() == NPCBuilding.BuildingType.WORK) { avaliableWorkingPlaces.Remove(building); }
        else if(building.GetBuildingType() == NPCBuilding.BuildingType.LEISURE) { avaliableLeisurePlaces.Remove(building); }
    }

    private NPCBuilding GetNewBuilding(NPCInfo npc, Vector3 housePosition, NPCBuilding.BuildingType type)
    {
        List<NPCBuilding> buildings;
        if (type == NPCBuilding.BuildingType.WORK) buildings = avaliableWorkingPlaces;
        else if (type == NPCBuilding.BuildingType.MARKET) buildings = npcBuildings[NPCBuilding.BuildingType.MARKET];
        else return null;

        NPCBuilding newBuilding = null;

        List<NPCBuilding> avaliableMarkets = new List<NPCBuilding>();

        float distance = float.PositiveInfinity;
        foreach (NPCBuilding building in buildings)
        {
            if(type == NPCBuilding.BuildingType.WORK && building.GetBuildingType() == NPCBuilding.BuildingType.MARKET)
            {
                avaliableMarkets.Add(building);
            }
            if ((building.transform.position - housePosition).magnitude < distance)
            {
                distance = (building.transform.position - housePosition).magnitude;
                newBuilding = building;
            }
        }
        if(avaliableMarkets.Count == 0)
        {
            if(NPCBuilding.BuildingType.MARKET != type && newBuilding.AddNPC(npc))
            {
                buildings.Remove(newBuilding);
            }
        }
        else
        {
            distance = float.PositiveInfinity;
            foreach (NPCBuilding building in avaliableMarkets)
            {
                if ((building.transform.position - housePosition).magnitude < distance)
                {
                    distance = (building.transform.position - housePosition).magnitude;
                    newBuilding = building;
                }
            }

            newBuilding.AddNPC(npc);
            buildings.Remove(newBuilding);
        }
        return newBuilding;
    }

    public NPCBuilding GetNewWorkingPlace(NPCInfo npc, Vector3 housePosition)
    {
        return GetNewBuilding(npc, housePosition, NPCBuilding.BuildingType.WORK);
    }

    public NPCBuilding GetLeisurePlace()
    {
        return avaliableLeisurePlaces[UnityEngine.Random.Range(0, avaliableLeisurePlaces.Count)];
    }

    public NPCBuilding GetNewMarketPlace(NPCInfo npc, Vector3 wokrplacePosition)
    {
        return GetNewBuilding(npc, wokrplacePosition, NPCBuilding.BuildingType.MARKET);
    }

    public void SpawnNPCs()
    {
        foreach(KeyValuePair<NPCBuilding.BuildingType, List<NPCBuilding>> b in npcBuildings)
        {
            foreach(NPCBuilding building in npcBuildings[b.Key])
            {
                if(building is House)
                {
                    (building as House).SpawnNPCs();
                }
                building.DeactivateCollider();
            }
        }
    }

    // Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        if (!npcPrefab)
        {
            Debug.LogError("Missing 'NPCPrefab' reference on SimulationManager.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleTime();
    }
}
