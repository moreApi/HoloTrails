using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//assuming only one Isowheel is active
public class FeedbackConditionManager : MonoBehaviour {

    public bool sound = false;
    public bool glow = true;
    public bool cursor = true;
    public bool gravity = false;

    public bool updated = false;

    public GameObject cursorObj;

	// Update is called once per frame
	void Update () {
		if (!updated)
        {
            Apply();

            updated = true;
        }
	}

    public void Apply()
    {
        //assuming only one Isowheel is active
        GameObject currentIsoWheel = GameObject.Find("IsoWheel");

        GameObject[] targets = currentIsoWheel.GetComponent<IsoMorph>().targets;

        foreach (GameObject obj in targets)
        {
            obj.GetComponent<FeedbackSound>().active = sound;
            obj.GetComponent<FeedbackGlow>().active = glow;
            obj.GetComponent<FeedbackGravity>().active = gravity;
        }

        cursorObj.SetActive(cursor);
    }
}
