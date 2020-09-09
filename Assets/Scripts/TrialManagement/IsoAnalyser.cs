using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoAnalyser : MonoBehaviour {


    private Vector3 view;

    TrialLogger logger;

    void Start()
    {
        logger = GameObject.FindObjectOfType<TrialLogger>();
    }


    public void Analyze()
    {
        view = Camera.main.transform.position;
        Transform target = transform.Find("Isotarget000").transform;

        //calc fitts dist
        Vector3 center = transform.position;
        Vector3 targetPos = target.position;

        float distanceDeg = DegFromView(center, targetPos);
        float distanceEul = Vector3.Distance(center, targetPos);

        //calc fitts width
        Vector3 borderOffset = new Vector3(target.localScale.x, 0, 0);
        Vector3 targLeft = targetPos + borderOffset;
        Vector3 targRight = targetPos - borderOffset;

        float widthDeg = DegFromView(targLeft, targRight);
        float widthEul = Vector3.Distance(targLeft, targRight);

        //calc difficulty index
        float difficultyIndexDeg = Mathf.Log((distanceDeg + widthDeg) / widthDeg, 2);
        float difficultyIndexEul = Mathf.Log((distanceEul + widthEul) / widthEul, 2);


        logger.serverLog(TrialEventType.IsoAnalysis,"Fitts values: Deg. Distance: " + distanceDeg 
            + " Deg. Width: " + widthDeg + " Deg. Difficulty Index: " + difficultyIndexDeg + " Bit;"
            + " Eul.Distance: " + distanceEul 
            + " Eul. Width: " + widthEul + " Eul. Difficulty Index: " + difficultyIndexEul + " Bit");
    }
	
	float DegFromView(Vector3 A, Vector3 B)
    {
        Vector3 diffA = A - view;
        Vector3 diffB = B - view;

        return Vector3.Angle(diffA,diffB);
    }
}
