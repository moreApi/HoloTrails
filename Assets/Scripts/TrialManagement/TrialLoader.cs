using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class TrialLoader : NetworkBehaviour
{

    public string ConfigFile = "Assets\\TrialConfigs.json";
    public TrialConfigs trialConfigs;
    public TrialConfig current;
    public bool shuffleTrials = true;

    public GameObject cursor;
    public GameObject isoWheel;

    private Vector3 origCursorScale;
    private float origZDistance;
    private int currentTrial = -1;
    private static System.Random random = new System.Random();

    private NetworkCommandHelper _localPlayer;
    private NetworkCommandHelper localPlayer
    {
        get
        {
            if (_localPlayer == null)
            {
                foreach (NetworkCommandHelper tmp in GameObject.FindObjectsOfType<NetworkCommandHelper>())
                {
                    if (tmp.isLocalPlayer)
                    {
                        _localPlayer = tmp;
                    }
                }
            }
            return _localPlayer;
        }
        set { _localPlayer = value; }
    }
    private FeedbackConditionManager fbCondManager;
    private SelectionConditionManager selCondManager;
    private TrialConductor trialConductor;




    // Use this for initialization
    void Start()
    {

#if UNITY_EDITOR
        StreamReader reader = new StreamReader(ConfigFile);
        String text = reader.ReadToEnd();
        trialConfigs = JsonUtility.FromJson<TrialConfigs>(text);
        reader.Close();

        if (shuffleTrials)
            ShuffleArray<TrialConfig>(trialConfigs.configs);
#endif

        current = new TrialConfig();
        current.name = "DebugTrial";
        current.targetDistance = 0.2f;
        current.targetSize = 0.05f;
        current.zDistance = 2;
        current.feedbackConditions = new string[] { "Glow", "Cursor" };
        current.selectionCondition = new string[] { "Dwell", "0.500" };

        origCursorScale = cursor.transform.Find("CursorVisual").localScale;
        origZDistance = isoWheel.transform.position.z;

        fbCondManager = GetComponent<FeedbackConditionManager>();
        selCondManager = GetComponent<SelectionConditionManager>();
        trialConductor = GetComponent<TrialConductor>();


    }

    public void loadNextTrail()
    {
        if (++currentTrial < trialConfigs.configs.Length)
        {
            setTrial(trialConfigs.configs[currentTrial]);
            print("Trial " + currentTrial+1 + " of " + trialConfigs.configs.Length);
        }
        else
        {
            print("No More Trials");
        }
    }

    public void setTrial(TrialConfig config)
    {
        localPlayer.CmdLoadLevel(config);
        print(trialConfigToString(config));
    }

    [ClientRpc]
    public void RpcSetTrial(TrialConfig config)
    {
        current = config;

        trialConductor.TrialName = config.name;
        trialConductor.tasks = config.runs;

        GravityCursor gc = cursor.GetComponent<GravityCursor>();
        gc.DefaultCursorDistance = config.zDistance;
        cursor.transform.Find("CursorVisual").localScale = origCursorScale * (config.zDistance / origZDistance);
        isoWheel.transform.Translate(new Vector3(0, 0, config.zDistance - isoWheel.transform.position.z)); //set new z value

        IsoMorph morph = isoWheel.GetComponent<IsoMorph>();
        morph.distance = config.targetDistance;
        morph.size = config.targetSize;

        List<string> fbConds = new List<string>(config.feedbackConditions);
        fbCondManager.glow = fbConds.Contains("Glow");
        fbCondManager.gravity = fbConds.Contains("Gravity");
        fbCondManager.cursor = fbConds.Contains("Cursor");
        fbCondManager.sound = fbConds.Contains("Sound");

        switch (config.selectionCondition[0])
        {
            case "Dwell":
                float seconds = float.Parse(config.selectionCondition[1], System.Globalization.CultureInfo.InvariantCulture);
                selCondManager.setDwellSelection(seconds);
                break;

            case "Clicker":
                selCondManager.setClickerSelection();
                break;

            case "AirTap":
                selCondManager.setAirTapSelection();
                break;
        }

        fbCondManager.Apply();

        //isoWheel.GetComponent<IsoAnalyser>().Analyze();
    }

    public static void ShuffleArray<T>(T[] arr)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int r = random.Next(0, i + 1);
            T tmp = arr[i];
            arr[i] = arr[r];
            arr[r] = tmp;
        }
    }

    public static string trialConfigToString(TrialConfig config)
    {
        string output = config.name + "   Runs: " + config.runs+"  Feedback:";
        foreach (string s in config.feedbackConditions)
        {
            output += " "+s+",";
        }
        output += "\nSelection:";
        foreach (string s in config.selectionCondition)
        {
            output += " "+s+",";
        }
        return output;
    }
}


[System.Serializable]
public struct TrialConfig
{
    public string name;
    public float zDistance;
    public float targetDistance;
    public float targetSize;
    public string[] feedbackConditions;
    public string[] selectionCondition;
    public int runs;
}

[System.Serializable]
public struct TrialConfigs
{
    public TrialConfig[] configs;
}