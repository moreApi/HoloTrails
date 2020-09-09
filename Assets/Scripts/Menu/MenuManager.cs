using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MenuManager : NetworkBehaviour
{

    public GameObject[] Menus;

    private int current = 0;
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

    public void enableMenu(int index)
    {
        localPlayer.CmdEnableMenu(index);
    }

    public void resetCurrent()
    {
        enableMenu(current);
    }

    [ClientRpc]
    public void RpcEnableMenu(int index)
    {
        current = index;
        for (int i = 0;i<Menus.Length;i++)
        {
            Menus[i].SetActive(index == i);
            if (index == i)
            {
                MenuController mc = Menus[i].GetComponent<MenuController>();
                mc.setChildenMenuState(MenuController.State.closed);
                mc.setChildenMenuState(MenuController.State.active);
            }
        }
    }
}
