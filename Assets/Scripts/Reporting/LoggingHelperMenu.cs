using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoggingHelperMenu : NetworkBehaviour
{

    public List<GameObject> deactivated = new List<GameObject>();

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

    public void logTaskStarted()
    {
        localPlayer.CmdLogTaskStarted();    
    }
    
}
