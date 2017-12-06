using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tagInfo
{
    public int tagid;
    int angle;
    int centerX;
    int centerY;
    int size;

    public tagInfo(int t, int a, int x, int y, int s)
    {
        tagid = t;
        angle = a;
        centerX = x;
        centerY = y;
        size = s;
    }

    public int getYRotAng()
    {
        return angle;
    }

    public Vector3 getPos()
    {
        return new Vector3(-centerX/3, 0, centerY/3);
    }
}

public class GameControllerScript : MonoBehaviour
{
    //public GameObject codeCanvas;
    public GameObject UICanvas;
    public GameObject soakPit;
    public GameObject toilet;
    public static Dictionary<int,tagInfo> tags;
    public GameObject origin1;
    Vector3 origin1Offset;
    public GameObject warningCircle;
    public GameObject warningCircleSmall;
    public GameObject warningCircleSmall2;
    public GameObject greenWarningCircle;
    public GameObject yellowWarningCircle;
    public GameObject blueWarningCircle;
    public GameObject pinkWarningCircle;
    int currScenario;
    public GameObject house1;
    public GameObject house2;
    public GameObject house3;
    public GameObject well;
    public GameObject tree;
    public GameObject tree2;
    public GameObject pump;
    public GameObject pump2;
    float soakPitScenario1Threshold = 19f;
    float toiletScenario1Threshold = 26.46f;
    Vector3 house1Pos1, house1Pos2;
    //Vector3 

    // Use this for initialization
    void Start ()
    {
        //codeCanvas.SetActive(false);
        //UICanvas.SetActive(true);
        tags = new Dictionary<int, tagInfo>();
        origin1Offset = new Vector3(-142f, 4, 100f);
        selectScenario(1);
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Debug.Log("Size of dictionary is - " + tags.Count);
        updatePoseFromTag(toilet, 161, 90);
        updatePoseFromTag(soakPit, 162, 0);
        //if(tags.ContainsKey(26))

        if (currScenario == 1)
        {
            if (checkIfPitAndToiletInsideTarget(blueWarningCircle))
            {
                blueWarningCircle.SetActive(false);
                //Debug.Log("Near!!");
            }
            if (checkIfPitAndToiletInsideTarget(pinkWarningCircle))
            {
                pinkWarningCircle.SetActive(false);
                //Debug.Log("Near!!");
            }
            if (checkIfPitAndToiletInsideTarget(greenWarningCircle))
            {
                greenWarningCircle.SetActive(false);
                //Debug.Log("Near!!");
            }
            if (checkIfPitAndToiletInsideTarget(yellowWarningCircle))
            {
                yellowWarningCircle.SetActive(false);
                //Debug.Log("Near!!");
            }
        }
        else if (currScenario == 3) //Tree
        {
            if (checkIfPitOrToiletNearTarget(warningCircleSmall, 71))
            {
                warningCircleSmall.SetActive(true);
                //Debug.Log("Near!!");
            }
            else
            {
                warningCircleSmall.SetActive(false);
            }
        }
        else if (currScenario == 4 || currScenario == 5) //Well & Pump
        {
            if (checkIfPitOrToiletNearTarget(warningCircle, 140))
            {
                warningCircle.SetActive(true);
                //Debug.Log("Near!!");
            }
            else
            {
                warningCircle.SetActive(false);
            }
        }
        else if (currScenario == 6) //Tree and Pump
        {
            if (checkIfPitOrToiletNearTarget(warningCircleSmall2, 71))
            {
                warningCircleSmall2.SetActive(true);
                //Debug.Log("Near!!");
            }
            else
            {
                warningCircleSmall2.SetActive(false);
            }
            if (checkIfPitOrToiletNearTarget(warningCircle, 140))
            {
                warningCircle.SetActive(true);
                //Debug.Log("Near!!");
            }
            else
            {
                warningCircle.SetActive(false);
            }
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            //Must add prompt
            Application.Quit();
        }
    }

    public void selectScenario(int n)
    {
        currScenario = n;
        if (currScenario == 1) //Intro
        {
            greenWarningCircle.SetActive(true);
            yellowWarningCircle.SetActive(true);
            blueWarningCircle.SetActive(true);
            pinkWarningCircle.SetActive(true);
            warningCircleSmall.SetActive(false);
            warningCircleSmall2.SetActive(false);
            warningCircle.SetActive(false);
            house1.SetActive(false);
            house2.SetActive(false);
            house3.SetActive(false);
            pump.SetActive(false);
            pump2.SetActive(false);
            well.SetActive(false);
            tree.SetActive(false);
            tree2.SetActive(false);
        }
        else if (currScenario == 2) //Road
        {
            greenWarningCircle.SetActive(false);
            yellowWarningCircle.SetActive(false);
            blueWarningCircle.SetActive(false);
            pinkWarningCircle.SetActive(false);
            warningCircle.SetActive(false);
            warningCircleSmall.SetActive(false);
            warningCircleSmall2.SetActive(false);
            house1.SetActive(true);
            house2.SetActive(false);
            house3.SetActive(false);
            pump.SetActive(false);
            pump2.SetActive(false);
            well.SetActive(false);
            tree.SetActive(false);
            tree2.SetActive(false);
        }
        else if (currScenario == 3) //Tree
        {
            greenWarningCircle.SetActive(false);
            yellowWarningCircle.SetActive(false);
            blueWarningCircle.SetActive(false);
            pinkWarningCircle.SetActive(false);
            warningCircle.SetActive(false);
            house1.SetActive(true);
            //warningCircleSmall.SetActive(true);
            warningCircleSmall2.SetActive(false);
            //warningCircleSmal
            house2.SetActive(false);
            house3.SetActive(false);
            pump.SetActive(false);
            pump2.SetActive(false);
            well.SetActive(false);
            tree2.SetActive(false);
            tree.SetActive(true);
        }
        else if (currScenario == 4) //Well
        {
            greenWarningCircle.SetActive(false);
            yellowWarningCircle.SetActive(false);
            blueWarningCircle.SetActive(false);
            pinkWarningCircle.SetActive(false);
            warningCircleSmall.SetActive(false);
            warningCircleSmall2.SetActive(false);
            //warningCircle.SetActive(true);
            warningCircle.transform.position = well.transform.position;
            house1.SetActive(false);
            house2.SetActive(true);
            house3.SetActive(false);
            pump.SetActive(false);
            pump2.SetActive(false);
            well.SetActive(true);
            tree.SetActive(false);
            tree2.SetActive(false);
        }
        else if (currScenario == 5) //Pump
        {
            greenWarningCircle.SetActive(false);
            yellowWarningCircle.SetActive(false);
            blueWarningCircle.SetActive(false);
            pinkWarningCircle.SetActive(false);
            warningCircleSmall.SetActive(false);
            warningCircleSmall2.SetActive(false);
            warningCircle.SetActive(true);
            warningCircle.transform.position = pump.transform.position;
            house1.SetActive(true);
            house2.SetActive(false);
            house3.SetActive(false);
            pump.SetActive(true);
            pump2.SetActive(false);
            well.SetActive(false);
            tree.SetActive(false);
            tree2.SetActive(false);
        }
        else if (currScenario == 6) //Pump & Tree
        {
            greenWarningCircle.SetActive(false);
            yellowWarningCircle.SetActive(false);
            blueWarningCircle.SetActive(false);
            pinkWarningCircle.SetActive(false);
            warningCircleSmall.SetActive(false);
            warningCircle.SetActive(false);
            warningCircleSmall2.SetActive(false);
            warningCircle.transform.position = pump2.transform.position;
            house1.SetActive(false);
            house2.SetActive(false);
            house3.SetActive(true);
            pump.SetActive(false);
            pump2.SetActive(true);
            well.SetActive(false);
            tree.SetActive(false);
            tree2.SetActive(true);
        }
    }

    bool checkIfPitAndToiletInsideTarget(GameObject obj)
    {
        Debug.Log("Distance between soakpit and " + obj + " is " + Vector3.Distance(soakPit.transform.position, obj.transform.position));
        Debug.Log("Distance between toilet and " + obj + " is " + Vector3.Distance(toilet.transform.position, obj.transform.position));

        if (Vector3.Distance(soakPit.transform.position, obj.transform.position) < soakPitScenario1Threshold && Vector3.Distance(toilet.transform.position, obj.transform.position) < toiletScenario1Threshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool checkIfPitOrToiletNearTarget(GameObject obj, float threshhold)
    {
        //Debug.Log("Distance between soakpit and " + obj + " is " + Vector3.Distance(soakPit.transform.position, obj.transform.position));
        if (Vector3.Distance(soakPit.transform.position, obj.transform.position) < threshhold || Vector3.Distance(toilet.transform.position, obj.transform.position) < threshhold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void updatePoseFromTag(GameObject obj, int tagNum, int offsetAng)
    {
        if (tags.ContainsKey(tagNum)) 
        {
            Debug.Log("Pos of " + obj + " - " + tags[tagNum].getPos());
            obj.transform.position = tags[tagNum].getPos() - origin1Offset;
            obj.transform.localRotation =  Quaternion.Euler(0, tags[tagNum].getYRotAng()+ offsetAng, 0); //Quaternion.AngleAxis(tags[tagNum].getYRotAng(), -1 * new Vector3(obj.transform.position.x, 0, obj.transform.position.z));


        }
    }

    public static void getTagInfo(tagInfo t)
    {
        if (tags.ContainsKey(t.tagid))
        {
            tags[t.tagid] = t;
        }
        else
        {
            tags.Add(t.tagid, t);            
        }
    }

    public void setCodeCanvasVisibility(bool status)
    {
        //codeCanvas.SetActive(status);
        //UICanvas.SetActive(!status);
    }
}
