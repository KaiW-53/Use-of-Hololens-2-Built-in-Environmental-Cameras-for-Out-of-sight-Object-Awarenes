using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slidebar : MonoBehaviour
{
    Camera m_MainCamera;
    GameObject leftBar;
    GameObject rightBar;
    GameObject rightCam;
    GameObject leftCam;
    Vector3 trackingPos;
    
    public int numOfTrackedObj;
    public GameObject[] trackedObjs;
    GameObject[] markers;


    bool isLeft;

    // Start is called before the first frame update
    void Start()
    {
        //This gets the Main Camera from the Scene
        m_MainCamera = Camera.main;
        //This enables Main Camera
        m_MainCamera.enabled = true;

        //Create left and right cameras
        rightCam = new GameObject();
        rightCam.AddComponent<Camera>();
        rightCam.GetComponent<Camera>().transform.SetParent(m_MainCamera.transform);
        rightCam.GetComponent<Camera>().transform.localPosition = new Vector3(4f, 1f, -2f);
        rightCam.GetComponent<Camera>().transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 75.0f, 0.0f));
        rightCam.name = "rightCam";
        rightCam.GetComponent<Camera>().targetDisplay = 1;

        leftCam = new GameObject();
        leftCam.AddComponent<Camera>();
        leftCam.GetComponent<Camera>().transform.SetParent(m_MainCamera.transform);
        leftCam.GetComponent<Camera>().transform.localPosition = new Vector3(-4f, 1f, -2f);
        leftCam.GetComponent<Camera>().transform.localRotation = Quaternion.Euler(new Vector3(0.0f, -75.0f, 0.0f));
        leftCam.name = "leftCam";
        leftCam.GetComponent<Camera>().targetDisplay = 2;

        //Create left and right slide bars
        leftBar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        leftBar.name = "Left Bar";
        leftBar.transform.SetParent(m_MainCamera.transform);
        leftBar.transform.localScale = new Vector3(0.5f, 9f, 0.5f);
        leftBar.GetComponent<MeshRenderer>().material.color = Color.blue;
        leftBar.transform.localPosition = new Vector3(-20f, 0f, 20f);

        rightBar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rightBar.name = "Right Bar";
        rightBar.transform.SetParent(m_MainCamera.transform);
        rightBar.transform.localScale = new Vector3(0.5f, 9f, 0.5f);
        rightBar.GetComponent<MeshRenderer>().material.color = Color.blue;
        rightBar.transform.localPosition = new Vector3(20f, 0f, 20f);

        //Create sphere for tracker

        markers = new GameObject[numOfTrackedObj];
        for (int i = 0; i < numOfTrackedObj; i++)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = "tracked object " + i;
            marker.transform.SetParent(m_MainCamera.transform);
            marker.transform.localScale = new Vector3(1f, 1f, 1f);
            marker.GetComponent<MeshRenderer>().material.color = Color.red;
            markers[i] = marker;
        }

        isLeft = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        //placeholders
        //trackingPos = new Vector3(20f, 0f, 20f);
        
        for (int i = 0; i < numOfTrackedObj; i++)
        {
            trackingPos = new Vector3(trackedObjs[i].transform.position.x, trackedObjs[i].transform.position.y, trackedObjs[i].transform.position.z);
            isLeft = true;

            if (isLeft == true)
            {
                markers[i].transform.localPosition = new Vector3(leftBar.transform.localPosition.x, findScaledPosYOfTrackedObj(trackingPos, isLeft), leftBar.transform.localPosition.z);
                Debug.Log(findScaledPosYOfTrackedObj(trackingPos, isLeft));
            }
            else 
            {
                markers[i].transform.localPosition = new Vector3(rightBar.transform.localPosition.x, findScaledPosYOfTrackedObj(trackingPos, isLeft), rightBar.transform.localPosition.z);
            }
            markers[i].GetComponent<Renderer>().enabled = isInView(trackingPos, isLeft) ?  true : false;
        }
    }

    //Finds the scaled y position of the object in relation to the bar.
    float findScaledPosYOfTrackedObj(Vector3 trackingPos, bool isLeft)
    {
        Vector3 result;
        //Find pixel size of left camera screen
        float screenHeight = leftCam.GetComponent<Camera>().pixelHeight; //832 pixels

        //Find pixel size of right camera screen
        float screenHeightR = rightCam.GetComponent<Camera>().pixelHeight; //832 pixels

        //Find pixel size of bars - both are equal in size
        Vector3 posStart = m_MainCamera.WorldToScreenPoint(leftBar.GetComponent<Renderer>().bounds.min);
        Vector3 posEnd = m_MainCamera.WorldToScreenPoint(leftBar.GetComponent<Renderer>().bounds.max);
        int barHeight = (int)(posEnd.y - posStart.y);
        Debug.Log("Quadspace: (" + barHeight + ")");

        //screen location of the quads:
        Vector3 leftBarScreenPos = m_MainCamera.WorldToScreenPoint(leftBar.transform.position);
        Debug.Log("quad screen coordinates (main cam): " + leftBarScreenPos); 
        Vector3 rightBarScreenPos = m_MainCamera.WorldToScreenPoint(rightBar.transform.position);

        if (isLeft == true)
        {
            //Find position of object on camera screen view (2D)
            Vector3 marker1ScreenPos = leftCam.GetComponent<Camera>().WorldToScreenPoint(trackingPos);   
            Debug.Log("target screen coordinates (left cam): " + marker1ScreenPos); 

            //Convert object's position to its position relative to the quad
            float posY = marker1ScreenPos.y * barHeight / screenHeight; 

            //Find actual position of object on the quad
            result = m_MainCamera.ScreenToWorldPoint(new Vector3(leftBarScreenPos.x, leftBarScreenPos.y - (barHeight/2f) + posY, leftBarScreenPos.z));
            return result.y;
        }

        else
        {
            //Find position of object on camera screen view (2D)
            Vector3 marker1ScreenPos = rightCam.GetComponent<Camera>().WorldToScreenPoint(trackingPos);   
            Debug.Log("target screen coordinates (right cam): " + marker1ScreenPos); 

            //Convert object's position to its position relative to the quad
            float posY = marker1ScreenPos.y * barHeight / screenHeightR; 

            //Find actual position of object on the quad
            result = m_MainCamera.ScreenToWorldPoint(new Vector3(rightBarScreenPos.x, rightBarScreenPos.y - (barHeight/2) + posY, rightBarScreenPos.z));
            return result.y;
        }
    }

    //Determines if object is in the view of the side cameras
    bool isInView(Vector3 trackingPos, bool isLeft) 
    {
        Vector3 screenPos;
        if (isLeft == true) 
        {
            screenPos = leftCam.GetComponent<Camera>().WorldToViewportPoint(trackingPos);  
        }
        else 
        {
            screenPos = rightCam.GetComponent<Camera>().WorldToViewportPoint(trackingPos);  
        }
        return screenPos.z > 0 && screenPos.x > 0 && screenPos.x < 1 && screenPos.y > 0 && screenPos.y < 1; 
    }
}
