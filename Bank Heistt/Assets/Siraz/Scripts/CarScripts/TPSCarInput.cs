using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using Cinemachine;
using UnityEngine.UI;
using Photon;
using UnityEngine.EventSystems;
public class TPSCarInput : Photon.MonoBehaviour
{
    Vector3 internetPosition;
    Quaternion internetRotation, internetTopGunRotation;

    public WeaponHandeler weaponHandler;

    FloatingJoystick TurnJoystick;
    FixedTouchField TouchField;
    CarUserControl CarControl;

    FixedTouchField fixedTouch;
    Button FireButton;

    float movementLerpTime = 3f;
    float rotationLerpTime = 5f;
    float TopGunrotationLerpTime = 25f;

    [HideInInspector]
    public int MovtPointerId;

    [Header("Sensitivity Settings")]

    [Range(0.001f,1f)]
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
    public Camera UIcamera;
    public GameObject topGun;
    Ray ray;
    public LayerMask aimDetectionLayers;
    Weapons PC = null;
    bool reloading;
    bool fire = false;
    private Vector3 correctPlayerPos;
    private Quaternion correctPlayerRot;
    private Vector3 currentVelocity;
    private float updateTime = 0f;
    private Rigidbody rb;

    /*
    protected float cameraAngleY;
    public float cameraAngleSpeed = 0.1f;

    protected float CameraPosY;
    public float cameraPosSpeed = 0.1f;
    */

    // Start is called before the first frame update

    public void Awake()
    {
        if (photonView.isMine)
        {
            TurnJoystick = FindObjectOfType<FloatingJoystick>();
            TouchField = FindObjectOfType<FixedTouchField>();
            VcamScriptLoc = FindObjectOfType<CinemachineFreeLook>();
            CarControl = GetComponent<CarUserControl>();
            fixedTouch = FindObjectOfType<FixedTouchField>();
            FireButton = FindObjectOfType<Button>();
            rb = GetComponent<Rigidbody>();
        }
    }

    void Start()
    {
        
        if (photonView.isMine)
        {
            CinemachineCore.GetInputAxis = HandleAxisInput;
            VcamScriptLoc.m_LookAt = transform;
            VcamScriptLoc.m_Follow = transform;
            //fixedTouch.CarInputPointerId = this;
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
        else
        {
            UIcamera.gameObject.SetActive(false);
            TPSCamera.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine)
        {
            //CarControl.Vinput = TurnJoystick.GetInputY();
            //CarControl.Hinput = TurnJoystick.GetInputX();

            CarControl.Vinput = Input.GetAxis("Vertical");
            CarControl.Hinput = Input.GetAxis("Horizontal");

            if (XAxis)
                TouchSensitivity_x = XAxis.value;
            if (YAxis)
                TouchSensitivity_y = YAxis.value;

            Ray aimRay = new Ray(TPSCamera.transform.position, TPSCamera.transform.forward);
            PositionCrosshair(aimRay, weaponHandler.currentWeapon);

            UpdateCrossHair();
            weaponLogic(aimRay);
            
        }

        #region OLD
        /*else
        {
            Vector3 internetDifference = internetPosition - transform.position;
            if (Mathf.Abs(Vector3.Magnitude(internetDifference)) < 4f) // agar normal limit mein hai internet ka difference
            {
                transform.position = Vector3.Lerp(transform.position, internetPosition, Time.deltaTime * movementLerpTime);
            }
            else //  agar dooriyan badh gyi hain to
            {
                print("ExtraPolation wali updation");
                float extrapolationFactor = 10f;
                Vector3 newInternetPosition = internetPosition + (internetPosition - transform.position).normalized * extrapolationFactor;
                transform.position = Vector3.Lerp(transform.position, newInternetPosition, Time.deltaTime * movementLerpTime);
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, internetRotation, Time.deltaTime * rotationLerpTime);
            topGun.transform.rotation = Quaternion.Slerp(topGun.transform.rotation, internetTopGunRotation, Time.deltaTime * TopGunrotationLerpTime);
        }*/
        /*
        if (TouchField.Pressed)
        {
            cameraAngleY += TouchField.TouchDist.x * cameraAngleSpeed;
            CameraPosY = Mathf.Clamp(CameraPosY - TouchField.TouchDist.y * cameraPosSpeed, 0.8f, 6f);

            Camera.main.transform.position = transform.position + Quaternion.AngleAxis(cameraAngleY + 180, Vector3.up) * new Vector3(0, CameraPosY, 7);
            Camera.main.transform.rotation = Quaternion.LookRotation(transform.position + Vector3.up * 2f - Camera.main.transform.position, Vector3.up);

        }
        else
        {
            //Quaternion final = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            //transform.rotation = Quaternion.Slerp(transform.rotation, final, Time.deltaTime * 10f);
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, CamOrig.position, Time.deltaTime * 5f);
            Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, CamOrig.rotation, Time.deltaTime * 5f);
            cameraAngleY = CamOrig.rotation.y;
            CameraPosY = CamOrig.position.y;
        }
        */

        /*
        if(TouchField.Pressed)
        {
            CinemachineCore.GetInputAxis = HandleAxisInput;
        }
        */

        /*
        Debug.Log("Original " + CamOrig);
        Debug.Log("CamPosY " + CameraPosY);
        Debug.Log("CameraAngleY" + cameraAngleY);
        */

        #endregion
    }

    public void FixedUpdate()
    {
        if (!photonView.isMine)
        {
            Vector3 projectedPosition = this.correctPlayerPos + currentVelocity * (Time.time - updateTime);
            transform.position = Vector3.Lerp(transform.position, projectedPosition, Time.deltaTime * 4);
            transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot, Time.deltaTime * 4);
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

        if (TouchField.Pressed)
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
        if (Physics.Raycast(bSpawnPoint, bSpawn.forward , out hit, curWeapon.weaponSettings.range, aimDetectionLayers))
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

    void OnPhotonSerializeView(PhotonStream stream , PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            stream.SendNext((float)CarControl.Hinput);
            stream.SendNext((float)CarControl.Vinput);
            stream.SendNext(rb.velocity);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(topGun.transform.rotation);
        }
        else
        {
            CarControl.Hinput = (float)stream.ReceiveNext();
            CarControl.Vinput = (float)stream.ReceiveNext();
            correctPlayerPos = (Vector3)stream.ReceiveNext();
            correctPlayerRot = (Quaternion)stream.ReceiveNext();
            currentVelocity = (Vector3)stream.ReceiveNext();
            updateTime = Time.time;
            //internetPosition = (Vector3)stream.ReceiveNext();
            //internetRotation = (Quaternion)stream.ReceiveNext();
            internetTopGunRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
