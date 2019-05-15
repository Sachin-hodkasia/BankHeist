using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : Photon.MonoBehaviour {



    public GameObject topGun;
    [HideInInspector]
    public float moveSpeed = 8f;


    public Joystick joystick;
    Quaternion carRotation;
    public Vector3 moveVector;
    Vector3 internetPosition;
    Quaternion internetRotation, internetTopGunRotation;
    float movementLerpTime=3f;
    float rotationLerpTime = 5f;
    float TopGunrotationLerpTime = 25f;


    private void Start()
    {
        joystick = GameObject.FindGameObjectWithTag("FixedJoystick").GetComponent<Joystick>();
    }

    void Update()
    {
   
        
        if (photonView.isMine)
        {
            
            moveVector = (Vector3.right * joystick.Horizontal + Vector3.forward * joystick.Vertical);
            
           
           
            if (moveVector != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(moveVector);
                //transform.Translate(moveVector * moveSpeed * Time.deltaTime, Space.World);
                

                GetComponent<Rigidbody>().AddForce(moveVector*3000*Time.deltaTime);
            }
        }
        else
        {
            //check for extrapolation here
            Vector3 internetDifference = internetPosition - transform.position;
            if (Mathf.Abs(Vector3.Magnitude(internetDifference)) < 4f) // agar normal limit mein hai internet ka difference
            {
                transform.position = Vector3.Lerp(transform.position, internetPosition, Time.deltaTime * movementLerpTime);
            }else //  agar dooriyan badh gyi hain to
            {
                print("ExtraPolation wali updation");
                float extrapolationFactor = 10f;
                Vector3 newInternetPosition = internetPosition + (internetPosition - transform.position).normalized * extrapolationFactor;
                transform.position = Vector3.Lerp(transform.position, newInternetPosition , Time.deltaTime * movementLerpTime );
            }
            transform.rotation = Quaternion.Lerp(transform.rotation, internetRotation, Time.deltaTime * rotationLerpTime);
            topGun.transform.rotation = Quaternion.Lerp(topGun.transform.rotation, internetTopGunRotation, Time.deltaTime * TopGunrotationLerpTime);
    
            
        }
    }

    void OnPhotonSerializeView(PhotonStream stream , PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(topGun.transform.rotation);
        
        }
        else
        {
            internetPosition = (Vector3)stream.ReceiveNext();
            internetRotation = (Quaternion)stream.ReceiveNext();
            internetTopGunRotation = (Quaternion)stream.ReceiveNext();
           
        }
    }
}
