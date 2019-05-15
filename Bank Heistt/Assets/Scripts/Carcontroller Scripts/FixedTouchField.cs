using UnityEngine;
using UnityEngine.EventSystems;
using Photon;
public class FixedTouchField : Photon.MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    /*
    public Vector2 TouchDist;
    public Vector2 PointerOld;
    [HideInInspector]
    */

    public bool Pressed;

    [HideInInspector]
    protected int PointerId;


    public CarUserControl CarInputPointerId;

    // Use this for initialization
    void Start()
    {
       
        
    }

    // Update is called once per frame
    void Update()
    {
        
        #region OLD
        //NOTE : Not needed as of now

        /*
        if (Pressed)
        {
            if (PointerId >= 0 && PointerId < Input.touches.Length)
            {
                TouchDist = Input.touches[PointerId].position - PointerOld;
                PointerOld = Input.touches[PointerId].position;
            }
            else
            {
                TouchDist = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - PointerOld;
                PointerOld = Input.mousePosition;
            }
        }
        else
        {
            TouchDist = new Vector2();
        }
        */
        #endregion
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Pressed = true;
        PointerId = eventData.pointerId;

        //Update the pointerId in other script;
        CarInputPointerId.MovtPointerId = PointerId;
        //PointerOld = eventData.position;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        Pressed = false;
    }

}