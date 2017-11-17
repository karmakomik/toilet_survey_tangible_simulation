using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenManagerScript : MonoBehaviour
{
    public GameObject haathiObj;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        haathiObj.transform.Rotate(Vector3.up, Time.deltaTime * 20);
	}

    public void startGame()
    {
        SceneManager.LoadScene(1);
    }
}
