using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkSelectionController : NetworkBehaviour
{

    public AConfirmer currentGazedAt = null;

    public void ConfirmGazedAt()
    {
        if (isServer)
            RpcConfimredGazedAt();
    }

    [ClientRpc]
    private void RpcConfimredGazedAt()
    {
        if (currentGazedAt != null)
        {
            currentGazedAt.ConfirmationAction();
        }
    }
}
