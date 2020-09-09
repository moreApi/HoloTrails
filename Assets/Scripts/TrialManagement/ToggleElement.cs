using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ToggleElement : NetworkBehaviour {

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

    private void Toggle(string name)
    {
        foreach (GameObject go in deactivated)
        {
            if (go.name == name)
            {
                go.SetActive(true);
                deactivated.Remove(go);
                return;
            }
        }
        GameObject gob = GameObject.Find(name);
        gob.SetActive(false);
        deactivated.Add(gob);
    }


    [ClientRpc]
	public void RpcToggleElement(string name)
    {
        Toggle(name);
    }

    public void  ToggleCursor()
    {
        localPlayer.CmdToggleElement("GravityCursor");
    }
}
