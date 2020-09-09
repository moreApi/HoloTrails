using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TrialConductor : NetworkBehaviour
{

    public float timeForSelection = 10;
    public float timeBeforeNewTarget = 0.5f;
    public string TrialName = "None";
    public int tasks = 0;

    private int currentTask = 0;
    private bool waitingForRestingPos = false;
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

    TrialLogger logger;
    TargetChooserAndMarker targetCandM;
    TrialLoader loader;

    // Use this for initialization
    void Start()
    {
        targetCandM = GameObject.Find("IsoWheel").GetComponent<TargetChooserAndMarker>();
        logger = gameObject.GetComponent<TrialLogger>();
        loader = GameObject.FindObjectOfType<TrialLoader>();
    }

    //sould be called via button
    public void startNextTrial()
    {
        logger.log(TrialEventType.TrialStart, TrialName);
        SetupNextTask();
    }

    //should be called via button
    public void abordTask()
    {
        localPlayer.CmdAbortTask();
    }


    [ClientRpc]
    public void RpcAbortTask()
    {
        if (isServer)
        {
            logger.log(TrialEventType.AbortTask);
        }
        targetCandM.deactivateTarget();
        SetupNextTask();
    }

    void SetupNextTask()
    {
#if UNITY_EDITOR
        if (currentTask++ < tasks)
        {
            targetCandM.chooseTarget();
            logger.log(TrialEventType.TaskStart,"Run: "+currentTask);
        }
        else
        {
            logger.log(TrialEventType.TrialEnd);
            loader.loadNextTrail();
            currentTask = 0;
        }
#endif
    }

    public void receiveConfirmation()
    {
        if (isServer)
        {
            localPlayer.CmdReceiveConfirmation();
        }
    }

    [ClientRpc]
    public void RpcReceiveConfrimation()
    {
        waitingForRestingPos = true;
    }
    

    public void cursorIsInRestingPos()
    {
        if (waitingForRestingPos)
        {
            Invoke("SetupNextTask", timeBeforeNewTarget);
            waitingForRestingPos = false;
        }
    }



}
