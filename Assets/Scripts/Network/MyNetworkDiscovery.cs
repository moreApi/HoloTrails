using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkDiscovery :  NetworkDiscovery{

    private bool actOnBroadcast = true;

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        print("Received broadcast from " + fromAddress);
        if (actOnBroadcast)
        {
            this.StopBroadcast();

            string[] split = fromAddress.Split(':');
            string ipv4Address = split[split.Length - 1];

            NetworkManager.singleton.StopHost();
            NetworkManager.singleton.networkAddress = ipv4Address;
            NetworkManager.singleton.StartClient();
            print("automatic connect to " + ipv4Address);
            actOnBroadcast = false;
        }
    }
}
