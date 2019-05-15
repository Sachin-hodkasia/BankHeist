using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using Cinemachine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityStandardAssets.Vehicles.Car;

[RequireComponent(typeof(CarController))]
public class CarUserControl : Photon.MonoBehaviour
{
    private CarController m_Car; // the car controller we want to use
    public float Hinput;
    public float Vinput;

    public WeaponHandeler weaponHandler;

    FloatingJoystick TurnJoystick;

    FixedTouchField fixedTouch;
    Button FireButton;

    float movementLerpTime = 3f;
    float rotationLerpTime = 5f;
    float TopGunrotationLerpTime = 25f;

    [HideInInspector]
    public int MovtPointerId;

    [Header("Sensitivity Settings")]

    [Range(0.001f, 1f)]
    public float TouchSensitivity_x = 10f;
    [Range(0.001f, 1f)]
    public float TouchSensitivity_y = 10f;

    //Temporary
    public Slider XAxis;
    public Slider YAxis;


    CinemachineFreeLook VcamScriptLoc;

    [Header("Recenter Settings")]
    public float RecenteringTime;
    public float RecenterWaitTime;

    Dictionary<Weapons, GameObject> crosshairPrefabMap = new Dictionary<Weapons, GameObject>();
    public Camera TPSCamera;
    public Camera UICamera;
    public GameObject topGun;
    Ray ray;
    public LayerMask aimDetectionLayers;
    Weapons PC = null;
    bool reloading;
    bool fire = false;

    private void Awake()
    {
        // get the car controller
        if (photonView.isMine)
        {
            m_Car = GetComponent<CarController>();
            TurnJoystick = FindObjectOfType<FloatingJoystick>();
            fixedTouch = FindObjectOfType<FixedTouchField>();
            VcamScriptLoc = FindObjectOfType<CinemachineFreeLook>();
            fixedTouch = FindObjectOfType<FixedTouchField>();
            FireButton = FindObjectOfType<Button>();
        }
    }

    public void Start()
    {
        if (photonView.isMine)
        {
            CinemachineCore.GetInputAxis = HandleAxisInput;
            VcamScriptLoc.m_LookAt = transform;
            VcamScriptLoc.m_Follow = transform;
            fixedTouch.CarInputPointerId = this;
            EventTrigger trigger = FireButton.gameObject.AddComponent<EventTrigger>();
            var pointerDown = new EventTrigger.Entry();
            var pointerUp = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerUp.eventID = EventTriggerType.PointerUp;
            pointerDown.callback.AddListener(FirebuttonDown);
            pointerUp.callback.AddListener(FirebuttonUp);
            trigger.triggers.Add(pointerDown);
            trigger.triggers.Add(pointerUp);
        }
    }

    public void Update()
    {
        
        if (photonView.isMine)
        {
            //Vinput = TurnJoystick.GetInputY();
            //Hinput = TurnJoystick.GetInputX();
            Vinput = Input.GetAxis("Vertical");
            Hinput = Input.GetAxis("Horizontal");
            m_Car.Move(Hinput, Vinput, Vinput, 0f);
            if (XAxis)
                TouchSensitivity_x = XAxis.value;
            if (YAxis)
                TouchSensitivity_y = YAxis.value;

            Ray aimRay = new Ray(TPSCamera.transform.position, TPSCamera.transform.forward);
            PositionCrosshair(aimRay, weaponHandler.currentWeapon);

            UpdateCrossHair();
            weaponLogic(aimRay);
        }
    }


    float HandleAxisInput(string axisName)
    {
        #region OLD

        //Debug.Log("Called");

        /*
        if(TouchField.Pressed)
        {
            switch (axisName)
            {
                case "Mouse X":

                    if (Input.touchCount > 0)
                    {
                        return Input.touches[0].deltaPosition.x / TouchSensitivity_x;
                    }
                    else
                    {
                        return Input.GetAxis(axisName);
                    }

                case "Mouse Y":
                    if (Input.touchCount > 0)
                    {
                        return Input.touches[0].deltaPosition.y / TouchSensitivity_y;
                    }
                    else
                    {
                        return Input.GetAxis(axisName);
                    }

                default:
                    Debug.LogError("Input <" + axisName + "> not recognyzed.", this);
                    break;
            }

            return 0f;
        }
        else
        {
            return 0f;
        }
        */
        #endregion

        if (fixedTouch.Pressed)
        {
            VcamScriptLoc.m_RecenterToTargetHeading = new CinemachineOrbitalTransposer.Recentering(false, 1, 1);

            switch (axisName)
            {
                case "Mouse X":

                    #region OLD
                    /*
                    if (MovtPointerId >= 0 && MovtPointerId < Input.touches.Length)
                    {
                        Debug.Log("Good Going so far");

                        Debug.Log(Input.touches[MovtPointerId].deltaPosition.x / TouchSensitivity_x + " is x Sensitivity");
                        return Input.touches[MovtPointerId].deltaPosition.x / TouchSensitivity_x;

                    }
                    else
                    {
                        Debug.Log("Always in else");

                        Debug.Log(Input.GetAxis(axisName));

                        return Input.GetAxis(axisName)*TouchSensitivity_x;
                    }

                case "Mouse Y":
                    if (MovtPointerId >= 0 && MovtPointerId < Input.touches.Length)
                    {

                        Debug.Log(Input.touches[MovtPointerId].deltaPosition.x / TouchSensitivity_x + " is y Sensitivity");
                        return Input.touches[MovtPointerId].deltaPosition.y / TouchSensitivity_y;
                    }
                    else
                    {
                        return Input.GetAxis(axisName)*TouchSensitivity_y;
                    }
                    */
                    #endregion

                    if (MovtPointerId >= 0 && MovtPointerId < Input.touches.Length)
                    {

                        return Input.touches[MovtPointerId].deltaPosition.x * TouchSensitivity_x;

                    }
                    else
                    {
                        return Input.GetAxis(axisName) * TouchSensitivity_x;
                    }

                case "Mouse Y":
                    if (MovtPointerId >= 0 && MovtPointerId < Input.touches.Length)
                    {
                        return Input.touches[MovtPointerId].deltaPosition.y * TouchSensitivity_y;
                    }
                    else
                    {
                        return Input.GetAxis(axisName) * TouchSensitivity_y;
                    }

                default:
                    Debug.LogError("Input <" + axisName + "> not recognized.", this);
                    break;
            }

            return 0f;
        }
        else
        {

            VcamScriptLoc.m_RecenterToTargetHeading
            = new CinemachineOrbitalTransposer.Recentering(true, RecenterWaitTime, RecenteringTime);

            VcamScriptLoc.m_YAxis.Value = 0.5f;

            return 0f;
        }

    }

    #region Crosshair
    public void CreateCrosshair(Weapons wep)
    {
        GameObject prefab = wep.weaponSettings.CrossHair;
        if (prefab != null)
        {
            prefab = Instantiate(prefab);
            //prefab.GetComponent<Canvas>().worldCamera = UIcamera;
            crosshairPrefabMap.Add(wep, prefab);
        }
    }

    void DeleteCrosshair(Weapons wep)
    {
        if (!crosshairPrefabMap.ContainsKey(wep))
            return;

        Destroy(crosshairPrefabMap[wep]);
        crosshairPrefabMap.Remove(wep);
    }

    // Position the crosshair to the point that we are aiming
    void PositionCrosshair(Ray ray, Weapons wep)
    {
        Weapons curWeapon = weaponHandler.currentWeapon;
        if (curWeapon == null)
            return;
        if (!crosshairPrefabMap.ContainsKey(wep))
            return;
        GameObject crosshairPrefab = crosshairPrefabMap[wep];
        RaycastHit hit;
        Transform bSpawn = curWeapon.weaponSettings.bulletSpwan;
        Vector3 bSpawnPoint = bSpawn.position;
        Vector3 dir = Vector3.zero;
        dir = ray.GetPoint(curWeapon.weaponSettings.range) - bSpawnPoint;
        if (Physics.Raycast(bSpawnPoint, bSpawn.forward, out hit, curWeapon.weaponSettings.range, aimDetectionLayers))
        {
            if (crosshairPrefab != null)
            {
                crosshairPrefab.transform.position = hit.point;
                crosshairPrefab.transform.LookAt(TPSCamera.transform);
            }
        }
    }

    void UpdateCrossHair()
    {
        if (weaponHandler.weaponList.Count == 0)
            return;
        if (PC != weaponHandler.currentWeapon)
        {
            foreach (Weapons wep in weaponHandler.weaponList)
            {
                if (wep == weaponHandler.currentWeapon)
                {
                    CreateCrosshair(wep);
                }
                else
                    DeleteCrosshair(wep);
            }
            PC = weaponHandler.currentWeapon;
        }
    }
    #endregion

    void weaponLogic(Ray aimRay)
    {
        if (fire)
        {
            if (weaponHandler.currentWeapon)
                weaponHandler.currentWeapon.Fire(aimRay);
        }
    }

    public void FirebuttonDown(BaseEventData baseEventData)
    {
        fire = true;
    }

    public void FirebuttonUp(BaseEventData baseEventData)
    {
        fire = false;
    }

}
