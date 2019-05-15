using System;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using Photon;

public class NetworkCar : PunBehaviour
{
    private CarUserControl m_CarInput;
    private Rigidbody rb;


    // cached values for correct position/rotation (which are then interpolated)
    private Vector3 correctPlayerPos;
    private Quaternion correctPlayerRot;
    private Quaternion topGunRot;
    private Vector3 currentVelocity;
    private float updateTime = 0;

    private void Awake()
    {
        m_CarInput = GetComponent<CarUserControl>();
        rb = GetComponent<Rigidbody>();
    }

    public void Start()
    {
        if (!photonView.isMine)
        {
            m_CarInput.TPSCamera.gameObject.SetActive(false);
            m_CarInput.UICamera.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// If it is a remote car, interpolates position and rotation
    /// received from network. 
    /// </summary>
    public void FixedUpdate()
    {
        if (!photonView.isMine)
        {
            Vector3 projectedPosition = this.correctPlayerPos + currentVelocity * (Time.time - updateTime);
            transform.position = Vector3.Lerp(transform.position, projectedPosition, Time.deltaTime * 4);
            transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot, Time.deltaTime * 4);
            m_CarInput.topGun.transform.rotation = Quaternion.Slerp(m_CarInput.topGun.transform.rotation, topGunRot, Time.deltaTime * 4);
        }
    }

    /// <summary>
    /// At each synchronization frame, sends/receives player input, position
    /// and rotation data to/from peers/owner.
    /// </summary>
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this car: send the others our input and transform data
            stream.SendNext((float)m_CarInput.Hinput);
            stream.SendNext((float)m_CarInput.Vinput);
            stream.SendNext(m_CarInput.topGun.transform.rotation);
            //stream.SendNext((float)m_CarInput.Handbrake);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(rb.velocity);
        }
        else
        {
            //Remote car, receive data
            m_CarInput.Hinput = (float)stream.ReceiveNext();
            m_CarInput.Vinput = (float)stream.ReceiveNext();
            topGunRot = (Quaternion)stream.ReceiveNext();
            //m_CarInput.Handbrake = (float)stream.ReceiveNext();
            correctPlayerPos = (Vector3)stream.ReceiveNext();
            correctPlayerRot = (Quaternion)stream.ReceiveNext();
            currentVelocity = (Vector3)stream.ReceiveNext();
            updateTime = Time.time;
        }
    }
}
