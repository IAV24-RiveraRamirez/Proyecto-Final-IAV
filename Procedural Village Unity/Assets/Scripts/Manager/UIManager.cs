using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    #region Singleton
    // Singleton
    private static UIManager instance = null;
    public static UIManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else Destroy(this);
    }
    #endregion

    // References
    [SerializeField] TextMeshProUGUI currentPeriodText = null;

    // Start is called before the first frame update
    void Start()
    {
        if(currentPeriodText == null)
        {
            Debug.LogError("No 'Current Period Text' was set on the inspector of UIManager");
        }
    }

    public void SetTimePeriod(SimulationManager.TimePeriods period)
    {
        if (currentPeriodText == null) { Debug.LogError("No 'Current Period Text' was set on the inspector of UIManager"); return; }
        TextInfo info = new CultureInfo("en-US", false).TextInfo;
        currentPeriodText.text = info.ToTitleCase(period.ToString());
    }
}
