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
    public GameObject greenWarningCircle;
    public GameObject yellowWarningCircle;
    public GameObject blueWarningCircle;
    public GameObject orangeWarningCircle;
    int currScenario;
    public GameObject house1;
    public GameObject house2;
    public GameObject well;
    public GameObject tree;
    public GameObject pump;

    // Use this for initialization
    void Start ()
    {
        //codeCanvas.SetActive(false);
        //UICanvas.SetActive(true);
        tags = new Dictionary<int, tagInfo>();
        origin1Offset = new Vector3(-142f, 0, 100f);
        currScenario = 0;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Debug.Log("Size of dictionary is - " + tags.Count);
        updatePoseFromTag(toilet, 161, 90);
        updatePoseFromTag(soakPit, 162, 0);
        //if(tags.ContainsKey(26))

        if (checkIfPitAndToiletNearTarget(blueWarningCircle, 32.4f))
        {
            //warningCircle.SetActive(true);
            Debug.Log("Near!!");
        }
        else
        {
            //warningCircle.SetActive(true);
            Debug.Log("Far!!");
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
        if (currScenario == 0)
        {
            greenWarningCircle.SetActive(true);
            yellowWarningCircle.SetActive(true);
            blueWarningCircle.SetActive(true);
            orangeWarningCircle.SetActive(true);


        }
    }

    bool checkIfPitAndToiletNearTarget(GameObject obj, float threshhold)
    {
        Debug.Log("Distance between soakpit and " + obj + " is " + Vector3.Distance(soakPit.transform.position, obj.transform.position));
        if (Vector3.Distance(soakPit.transform.position, obj.transform.position) < threshhold)
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
