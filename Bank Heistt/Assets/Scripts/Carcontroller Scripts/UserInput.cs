using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class UserInput : MonoBehaviour
{
    Joystick joystick;
    CarController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CarController>();
        //joystick = GameObject.FindGameObjectWithTag("FixedJoystick").GetComponent<Joystick>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        controller.Move(h, v,v,0f);

    }
}
