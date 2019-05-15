using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PivotRotateTurret : MonoBehaviour
{
    CinemachineFreeLook CamRot;

    public float LowestBendAngle;
    public float HighestBendAngle;

    public void Start()
    {
        CamRot = FindObjectOfType<CinemachineFreeLook>();
    }

    // Update is called once per frame
    void Update()
    {
        float XInput = CamRot.m_XAxis.Value;
        float YInput = (((CamRot.m_YAxis.Value)) * (HighestBendAngle - LowestBendAngle) + LowestBendAngle);

        //Debug.Log(transform.parent.rotation.eulerAngles.y + transform.parent.name);

        Quaternion final
            = Quaternion.Euler(0f,
            XInput-90f + transform.parent.transform.rotation.eulerAngles.y,
            -YInput);

        transform.rotation = Quaternion.Lerp(transform.rotation, final , 10f * Time.deltaTime);
    }
}
