using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;


public class GameAnalytic : MonoBehaviour
{


    [Header("Games Events Names")]
    [SerializeField] private string _GameAnalytics_ID = "";
    [SerializeField] private string startTheGameEvent = "startTheGame";
    [SerializeField] private string CompletedEvent = "Completed";
    [SerializeField] private string FaildEvent = "Faild";
    [SerializeField] private string PlayeLevelEvent = "finishedLevel";
    [Header("Sync Settings")]
    [SerializeField] private float Check_UnSended_Events_rateTime = 0.5f;


    static List<LevelProgression> v_LevelsProgressions = new List<LevelProgression>();

    float time;

    private void Awake()
    {
        GameAnalytics.Initialize();
        GameAnalytics.SetCustomId(_GameAnalytics_ID);

    }

    private void Update()
    {
        time += Time.unscaledDeltaTime;
        if (time > Check_UnSended_Events_rateTime)
        {
            FireAllEvents();
            time = 0;
        }
    }


    private void Start()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, startTheGameEvent);

    }

    public void FireAllEvents()
    {
        foreach (var lp in v_LevelsProgressions)
        {

            //GameAnalytics.NewProgressionEvent(lp.state,lp.v_LevelName,lp.v_Diffeculty,lp.v_LevelTime,lp.v_ClearedArea);
            GameAnalytics.NewProgressionEvent(lp.state, lp.v_LevelName);


        }
        v_LevelsProgressions.Clear();
    }

    public static void AddProgression(LevelProgression l)
    {
        v_LevelsProgressions.Add(l);
    }


}

public class LevelProgression
{
    public GAProgressionStatus state;
    public string v_LevelName;
    //public int v_ClearedArea;
    //public string v_Diffeculty;
    //public string v_LevelTime;

    public Dictionary<string, object> getDictionaty()
    {
        var levelProgressionDictionary = new Dictionary<string, object>();
        levelProgressionDictionary.Add("LevelName", v_LevelName);
        //  levelProgressionDictionary.Add("ClearedArea",v_ClearedArea);
        //  levelProgressionDictionary.Add("Diffeculty",v_Diffeculty);
        //  levelProgressionDictionary.Add("LevelTime",v_LevelTime);
        return levelProgressionDictionary;
    }
}
