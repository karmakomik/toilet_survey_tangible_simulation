using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveableObjectScript : MonoBehaviour, IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("Drag!");
        //if (Input.GetMouseButtonDown(0))
        {
            float distance;
            float planeHeight = 0;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.up * planeHeight);
            if (plane.Raycast(ray, out distance))
            {
                //Debug.Log(ray.GetPoint(distance));
                transform.position = ray.GetPoint(distance);
            }
        }

        //throw new NotImplementedException();
    }
    //https://forum.unity.com/threads/solved-moving-gameobject-along-x-and-z-axis-by-drag-and-drop-using-x-and-y-from-screenspace.488476/
    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
