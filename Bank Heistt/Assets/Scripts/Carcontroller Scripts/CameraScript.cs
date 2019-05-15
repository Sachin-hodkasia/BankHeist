using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraScript : MonoBehaviour
{
    GameObject Car;
    private Vector3 firstpoint; //change type on Vector3
    private Vector3 secondpoint;
    private float xAngle = 0f; //angle for axes x for rotation
    private float yAngle = 0f;
    private float xAngTemp = 0f; //temp variable for angle
    private float yAngTemp = 0f;
    private float resetTrigger = 0f;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    public void Start()
    {
        xAngle = 0f;
        yAngle = 0f;
        m_Raycaster = FindObjectOfType<GraphicRaycaster>();
        m_EventSystem = FindObjectOfType<EventSystem>();
        Car = GetComponentInParent<UserInput>().gameObject;
    }

    public void Update()
    {
        //transform.position = Car.transform.position + offset;
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                m_PointerEventData = new PointerEventData(m_EventSystem);
                m_PointerEventData.position = Input.GetTouch(i).position;
                List<RaycastResult> results = new List<RaycastResult>();
                m_Raycaster.Raycast(m_PointerEventData, results);

                foreach (RaycastResult result in results)
                {
                    if (result.gameObject.tag == "touchLayer")
                    {
                        MoveCameraWithTouch(i);
                    }
                    else
                    {
                        ResetCamera();
                    }
                }
            }
        }
        else
        {
            ResetCamera();
        }
    }

    void MoveCameraWithTouch(int touchIndex)
    {
       
        resetTrigger = 1.5f;
        if (Input.GetTouch(touchIndex).phase == TouchPhase.Began)
        {
            firstpoint = Input.GetTouch(touchIndex).position;
            xAngTemp = xAngle;
            yAngTemp = yAngle;
        }
        //Move finger by screen
        if (Input.GetTouch(touchIndex).phase == TouchPhase.Moved)
        {
            secondpoint = Input.GetTouch(touchIndex).position;
            //Mainly, about rotate camera. For example, for Screen.width rotate on 180 degree
            xAngle = xAngTemp + (secondpoint.x - firstpoint.x) * 180f / Screen.width;
            yAngle = yAngTemp - (secondpoint.y - firstpoint.y) * 90f / Screen.height;
            xAngle = Mathf.Clamp(xAngle, -60f, 60f);
            yAngle = Mathf.Clamp(yAngle, -15f, 15f);
            AddOffset(xAngle, yAngle,Car.transform.rotation.eulerAngles.y);
        }
        
    }

    void ResetCamera()
    {
        resetTrigger -= Time.deltaTime;
        if (resetTrigger <= 0f)
        {
            xAngle = 0f;
            yAngle = 0f;
            Quaternion final = Quaternion.Euler(0, Car.transform.rotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, final, Time.deltaTime * 5f);
        }
    }

    void AddOffset(float xAngle, float yAngle,float carRotY)
    {
        if (carRotY >= 0f && carRotY <= 120f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(yAngle, xAngle + carRotY, 0f), Time.deltaTime * 15f);
        }
        else if(carRotY > 120f && carRotY < 240f)
        {
            if (xAngle >= 0)
            {
                xAngle = xAngle - (180 + (180 - carRotY));
            }
            else
            {
                xAngle = xAngle + carRotY;
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(yAngle, xAngle, 0f), Time.deltaTime * 15f);
        }
        else if(carRotY >= 240f && carRotY <= 360)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(yAngle, xAngle-(360f- carRotY), 0f), Time.deltaTime * 15f);
        }
    }

}
