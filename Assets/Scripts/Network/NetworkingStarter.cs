using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkingStarter : MonoBehaviour {
    public bool done = false;
    public bool server = false;

	void Update() {
        if (!done)
        {
            print("starter started");
            MyNetManager mnm = GetComponent<MyNetManager>();

            bool editor = false;
#if UNITY_EDITOR
            editor = true;
#endif

            if (!server && editor) { 
            
                print("im client and port free: " + mnm.discovery.Initialize());
                mnm.discovery.StartAsClient();
            }
            else
            {
                print("im Server");
                mnm.StartHost();
            }
            done = true;
        }


    }

}
