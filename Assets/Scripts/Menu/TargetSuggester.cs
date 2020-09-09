using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TargetSuggester : NetworkBehaviour
{


    [TextArea(3, 10)]
    public string items = "Entry 1\n.Subentry1\n.Subentry2\nEntry2";
    public Text text;

    int counter = 0;
    private string[] itemsSplit;
    private TrialLogger logger;
    private NetworkCommandHelper _localPlayer;
    private NetworkCommandHelper localPlayer
    {
        get
        {
            if (_localPlayer == null)
                foreach (NetworkCommandHelper tmp in GameObject.FindObjectsOfType<NetworkCommandHelper>())
                    if (tmp.isLocalPlayer)
                        _localPlayer = tmp;

            return _localPlayer;
        }
        set { _localPlayer = value; }
    }
    
    
    // Use this for initialization
    void Start()
    {
        logger = FindObjectOfType<TrialLogger>();
        itemsSplit = items.Split('\n');
        suggestTarget();
    }

    public void suggestTarget()
    {
#if UNITY_EDITOR
        if (localPlayer == null)
        {
            Invoke("suggestTarget", 0.5f);
            return;
        }
        string target = "";
        while (!target.StartsWith("..") && text.text != target)
        {
            target = itemsSplit[UnityEngine.Random.Range(0,itemsSplit.Length - 1)];
        }
        target = counter++ + ". "+ target.Substring(2);
        localPlayer.CmdSuggestMenuTarget(target);
#endif
    }

    [ClientRpc]
    internal void RpcSuggestMenuTarget(string target)
    {
        text.text = target;
        logger.currentMenuTarget = target;
    }
}
