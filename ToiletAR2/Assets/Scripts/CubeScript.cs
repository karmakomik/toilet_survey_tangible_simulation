using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class CubeScript : MonoBehaviour
{
    public GameObject codePanel;
    List<string> commList = new List<string>();
    bool moveToNextCommand = false;
    Thread timerThread;
    Vector3 haathiPos;
    Quaternion haathiRot;
    bool isExecute = false;
    Vector3 haathiForwardFactVec = new Vector3(0,0,0);

    // Use this for initialization
    void Start()
    {
        //commList = new List<string>();
        haathiPos = transform.position;//new Vector3(-3.18f, 0, -0.7f);
    }

    public void waitSecs(object arg)
    {
        float time = (float)arg;
        Thread.Sleep((int)time * 1000);
        //Thread.Sleep(3000);
        moveToNextCommand = true;
        //var watch = System.Diagnostics.Stopwatch.StartNew();
        // the code that you want to measure comes here
        //watch.Stop();
        //var elapsedMs = watch.ElapsedMilliseconds;
    }

    public void addCommandToPool(string command)
    {
        Debug.Log("Command received - " + command);
        commList.Add(command);
    }

    public void clearCommandPool()
    {
        commList = new List<string>();
    }

    public void startExecution()
    {
        isExecute = true;
        //codePanel.SetActive(false);
        //Debug.Log("Commands given -  ");
        moveToNextCommand = true;
        //bool areAllCommandsProcessed = false;
        /*foreach (string s in commList)
            Debug.Log(s);*/

        //Debug.Log("Starting execution - ");

    }

    public void executeCommands()
    {
        if (commList.Count > 0)
        {
            string currComm = commList[0];

            Debug.Log("Currently processing command - " + currComm);
            //Debug.Log("moveToNextCommand - " + moveToNextCommand);
            if (moveToNextCommand)
            {
                if (currComm.StartsWith("move"))
                {
                    float dist = 0;
                    if (float.TryParse(currComm.Split(' ')[1], out dist)){ }
                    Debug.Log("move units - " + dist);
                    move(dist);
                    commList.RemoveAt(0);
                }
                else if (currComm.StartsWith("rotate"))
                {
                    float ang = 0;
                    if (float.TryParse(currComm.Split(' ')[1], out ang)){ }
                    rotate(ang);
                    commList.RemoveAt(0);
                }
                else if (currComm.StartsWith("wait"))
                {
                    moveToNextCommand = false;
                    float time = 0;
                    if (float.TryParse(currComm.Split(' ')[1], out time)){ }
                    //Debug.Log("Start wait");
                    //timerThread = new Thread(new ParameterizedThreadStart(waitSecs));
                    //timerThread.Start(time);
                    wait(time);
                    //Debug.Log("Call to wait done");
                    commList.RemoveAt(0);
                }
                else
                {
                    Debug.Log("Unprocesssed");
                }
                /*else if ()
                {

                }
                else if ()
                {

                }
                else if ()
                {

                }
                else if ()
                {

                }
                else if ()
                {

                }*/
            }
        }
        else
        {
            //isExecute = false;
            //codePanel.SetActive(true);
        }
    }

    public void move(float units)
    {
        //Debug.Log("Move");
        //StartCoroutine(_move(units));
        //transform.Translate(0, 0, units / 100);
        haathiForwardFactVec.Set(0, 0, units/100);
        haathiPos = transform.position + transform.TransformDirection(haathiForwardFactVec);
    }
    
    private IEnumerator _wait(float units)
    {        
        //objectPaused = true;
        //Debug.Log("Start wait");
        yield return new WaitForSeconds(units);        
        //Debug.Log("End wait");
        moveToNextCommand = true;
    }

    public void rotate(float units)
    {
        transform.Rotate(0, units, 0);
    }

    public void wait(float time)
    {
        StartCoroutine(_wait(time));
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = haathiPos;

        if (isExecute)
        {
            executeCommands();
        }
    }
}

