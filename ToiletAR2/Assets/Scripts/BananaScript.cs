using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaScript : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        GetComponent<Animation>().Play("bananaappear");
        	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            GetComponent<Animation>().Play("banana_anim");
        }
	}
}
