using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class TrialLogger : NetworkBehaviour
{

    private TrialConductor trialConductor;
    private StreamWriter writer = null;
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

    public string FolderPath;
    internal string currentMenuTarget = "";

    // Use this for initialization
    void Start()
    {
        trialConductor = GetComponent<TrialConductor>();
#if UNITY_EDITOR 
        writer = new StreamWriter(FolderPath + @"\Log" + System.DateTime.Now.ToString("MM.dd.HH.mm") + ".txt");
#endif
    }

    [ClientRpc]
    public void RpcLog(TrialEvent tEvent)
    {
        print(tEvent.toRow());
#if UNITY_EDITOR
        if (writer != null)
        {
            writer.WriteLine(tEvent.toRow());
        }
#endif
    }

    public void log(TrialEvent tEvent)
    {
        localPlayer.CmdLog(tEvent);
    }

    public void log(TrialEventType type)
    {
        log(new TrialEvent(type, trialConductor.TrialName, Time.time));
    }

    public void log(TrialEventType type, string note)
    {
        log(new TrialEvent(type, trialConductor.TrialName, Time.time, note));
    }

    public void serverLog(TrialEventType type)
    {
        if (isServer)
            log(new TrialEvent(type, trialConductor.TrialName, Time.time));
    }

    public void serverLog(TrialEventType type, string note)
    {
        if (isServer)
            log(new TrialEvent(type, trialConductor.TrialName, Time.time, note));
    }

    public void logMenu(string buttonName)
    {
        if (isServer)
        {
            TrialEvent tmp = new TrialEvent(TrialEventType.Hit, "Menu", Time.time);
            tmp.note = buttonName;
            log(tmp);
        }
    }

    public void logMenuTaskStartet()
    {
        TrialEvent tmp = new TrialEvent(TrialEventType.TaskStart, "Menu", Time.time,currentMenuTarget);
        log(tmp);
    }

    void OnDestroy()
    {
#if UNITY_EDITOR
        if (writer != null)
        {
            writer.Flush();
            writer.Close();
        }
#endif
    }

}

[Serializable]
public enum TrialEventType { TaskStart, Hit, Miss, AbortTask, TrialStart, TrialEnd, IsoAnalysis };


[Serializable]
public class TrialEvent
{

    public TrialEventType type;
    public string trialName;
    public float time;
    public string note;

    public TrialEvent() { }

    public TrialEvent(TrialEventType type, string trialName, float time)
    : this(type, trialName, time, null) { }

    public TrialEvent(TrialEventType type, string trialName, float time, string note)
    {
        this.type = type;
        this.trialName = trialName;
        this.time = time;
        this.note = note;
    }

    public string toRow()
    {
        return time + ";" + trialName + ";" + type.ToString() + ((note != null) ? ";" + note : "");
    }
}
