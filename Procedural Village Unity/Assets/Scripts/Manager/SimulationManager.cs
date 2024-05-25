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

    // Getters
    public float GetTime() { return time; }
    public float GetTimePercent() { return time / HOURS_IN_DAY; }
    public TimePeriods GetCurrentPeriod() { return currentPeriod; }

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

    // Unity Methods

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleTime();
    }
}
