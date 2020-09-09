using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TargetChooserAndMarker : NetworkBehaviour {

    public Material markMat;
    public System.Random rnd = new System.Random();

    private GameObject[] targets;
    private GameObject lastTarget = null;

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

    // Use this for initialization
    void Start () {
        targets = gameObject.GetComponent<IsoMorph>().targets;
	}
	
    public void chooseTarget()
    {
        int newTarget = rnd.Next(targets.Length);
        localPlayer.CmdChooseTarget(newTarget);
    }

    [ClientRpc]
    public void RpcChooseTarget(int newTarget)
    {
        //12 Targets
        activateTarget(targets[newTarget]);
    }

    private void activateTarget(GameObject target)
    {
        target.GetComponent<SelectionConfirmer>().targetIsActive = true;
        target.GetComponent<SelectionConfirmer>().CancelMaterialChange();
        target.GetComponent<Renderer>().material = markMat;
        lastTarget = target;
    }

    public void deactivateTarget()
    {
        if (lastTarget)
        {
            lastTarget.GetComponent<SelectionConfirmer>().targetIsActive = false;
            lastTarget.GetComponent<SelectionConfirmer>().ResetMaterial();
            lastTarget.GetComponent<SelectionConfirmer>().CancelMaterialChange();

            lastTarget = null;
        }

    }


}
