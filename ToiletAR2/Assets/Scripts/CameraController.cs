using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float lookSmooth = 0.09f;
    public Vector3 offsetFromTarget = new Vector3(-8, -1, 8);//new Vector3(10, 16, 18); //new Vector3(0, -6, -8);
    public float xTilt = 10;
    public Camera camera;
    public float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.

    bool overHeadViewToggle = true;

    Quaternion camDefaultOrientation;

    Vector3 destination = Vector3.zero;
    //CharacterController charController;
    float rotateVel = 0;

	// Use this for initialization
	void Start ()
    {        
        SetCameraTarget(target);
        camera = gameObject.GetComponent<Camera>();
        camera.fieldOfView = 80;
        //camDefaultOrientation = transform.rotation;
        //transform.rotation = Quaternion.Euler(45,-45, 0);
        changeCamView();

        //transform.rotation = Quaternion.AngleAxis(90, Vector3.right);
        //transform.position = new Vector3(target.position.x, 18.8f, target.position.z);
    }

    void SetCameraTarget(Transform t)
    {
        target = t;

        /*if (target != null)
        {
            if (target.GetComponent<CharacterController>())
            {
                charController = target.GetComponent<CharacterController>();
            }
            else
            {
                Debug.LogError("The camera's target needs a character controller.");
            }
        }*/
    }

    void Update()
    {
#if UNITY_STANDALONE || UNITY_EDITOR_WIN

        camera.fieldOfView += Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 1000;
        /*if (camera.fieldOfView > 99)
        {
            camera.fieldOfView = 60;
            transform.rotation = Quaternion.AngleAxis(90, Vector3.right);
            transform.position = new Vector3(target.position.x, 18.8f, target.position.z);
            overHeadViewToggle = true;
        }*/

        // Clamp the field of view to make sure it's between 0 and 180.
        camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 35f, 99.9f);
#endif

#if UNITY_ANDROID
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // If the camera is orthographic...
            if (camera.orthographic)
            {
                // ... change the orthographic size based on the change in distance between the touches.
                //camera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

                // Make sure the orthographic size never drops below zero.
                //camera.orthographicSize = Mathf.Max(camera.orthographicSize, 0.1f);
            }
            else
            {
                // Otherwise change the field of view based on the change in distance between the touches.
                camera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;
                /*if (camera.fieldOfView > 99)
                {
                    camera.fieldOfView = 60;
                    transform.rotation = Quaternion.AngleAxis(90, Vector3.right);
                    transform.position = new Vector3(target.position.x, 18.8f, target.position.z);
                    overHeadViewToggle = true;
                }*/

                // Clamp the field of view to make sure it's between 0 and 180.
                camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 35f, 99.9f);
            }
        }
#endif
    }

	// Update is called once per frame
	void LateUpdate ()
    {
        FollowTarget();
        //MoveToTarget();
        //LookAtTarget();	
    }

    Vector2 camOffset = new Vector3(-5, 5);
    Vector2 camOffsetDest;
    Vector2 tempCamOffset;

    float angle1 = 225;
    float angle2 = 225;
    float angle2Dest = 225;

    void FollowTarget()
    {
        /*if (!overHeadViewToggle)
        {*/
        //tempCamOffset = Vector2.Lerp(camOffset,camOffsetDest,)
        camOffset = Vector2.Lerp(camOffset, camOffsetDest, 2 * Time.deltaTime);
        transform.position = target.position - new Vector3(camOffset.x, -5, camOffset.y); //new Vector3(-5, -7, 5);

        if (angle2 != angle2Dest)
        {
            angle2 = Mathf.Lerp(angle2, angle2Dest, 2f * Time.deltaTime);
            transform.rotation = Quaternion.Euler(45, angle2, 0);
        }
        /*}
        else
        {
            transform.position = new Vector3(target.position.x, 18.8f, target.position.z);
        }*/

    }

    public void changeCamView()
    {
        // x = r * cos(a)
        // z = r * sin(a)
        angle1 -= 90;
        angle1 = angle1 % 360;
        angle2Dest += 90;
        //angle2Dest = angle2Dest % 360;
        
        //Debug.Log("Angle - " + angle1);
        camOffsetDest = new Vector2(8 * Mathf.Cos(Mathf.Deg2Rad * angle1), 8 * Mathf.Sin(Mathf.Deg2Rad * angle1));
        
    }
}
