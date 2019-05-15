using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public GameObject player;
    public Vector3 offset;
    public float followTimeLapse ;
    public GameObject mainCam;
    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        mainCam.GetComponent<Camera>().orthographicSize = 14f;

	}
	
	// Update is called once per frame
	void FixedUpdate () {
        
        transform.position = Vector3.Slerp(gameObject.transform.position, player.transform.position + offset, followTimeLapse * Time.deltaTime);
       
	}
}
