using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class NetworkCommandHelper : NetworkBehaviour
{

    TrialLoader loader;
    TrialConductor conductor;
    ToggleElement toggler;
    TrialLogger logger;
    TargetChooserAndMarker targetCandM;
    TargetSuggester suggester;
    MenuManager menuManager;

    void Start()
    {
        loader = FindObjectOfType<TrialLoader>();
        conductor = FindObjectOfType<TrialConductor>();
        toggler = FindObjectOfType<ToggleElement>();
        logger = FindObjectOfType<TrialLogger>();
        targetCandM = FindObjectOfType<TargetChooserAndMarker>();
        suggester = FindObjectOfType<TargetSuggester>();
        menuManager = FindObjectOfType<MenuManager>();
    }



    [Command]
    public void CmdLoadLevel(TrialConfig config)
    {
        loader.RpcSetTrial(config);
    }


    [Command]
    public void CmdLoadMenuScene()
    {
        SceneManager.LoadScene("menu");
    }

    [Command]
    public void CmdLoadIsoScene()
    {
        SceneManager.LoadScene("iso");
    }

    [Command]
    public void CmdAbortTask()
    {
        conductor.RpcAbortTask();
    }

    [Command]
    public void CmdToggleElement(String name)
    {
        toggler.RpcToggleElement(name);
    }

    [Command]
    public void CmdLogTaskStarted()
    {
        logger.logMenuTaskStartet();
    }

    [Command]
    public void CmdChooseTarget(int newTarget)
    {
        targetCandM.RpcChooseTarget(newTarget);
    }

    [Command]
    internal void CmdLog(TrialEvent tEvent)
    {
        tEvent.time = Time.time; //set time to server time
        logger.RpcLog(tEvent);
    }

    [Command]
    public void CmdReceiveConfirmation()
    {
        conductor.RpcReceiveConfrimation();
    }

    [Command]
    public void CmdSuggestMenuTarget(string target)
    {
        suggester.RpcSuggestMenuTarget(target);
    }

    [Command]
    public void CmdEnableMenu(int index)
    {
        menuManager.RpcEnableMenu(index);
    }
}
