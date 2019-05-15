using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
public class Bomb : Photon.MonoBehaviour {

    public GameObject explosion;
    public GameObject trails;
    public Collider explosionCollider;
	// Use this for initialization
	void Start () {
        Vector3 vel = (Vector3)photonView.instantiationData[0];
        GetComponent<Rigidbody>().velocity = vel;
        StartCoroutine(turnOnCollider());
        Destroy(gameObject, 3f);
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<Collider>().enabled = false;
        StartCoroutine(shakeCamera());
        explosion.transform.parent = null;
        explosion.SetActive(true);
        trails.SetActive(false);
        explosionCollider.enabled = true;
        Destroy(explosion, 1.5f);
       
    }
    IEnumerator turnOnCollider()
    {
        yield return new WaitForSeconds(0.7f);
        GetComponent<Collider>().enabled = true;
    }
    IEnumerator shakeCamera()
    {
        CameraShaker.Instance.ShakeOnce(8f, 0f, 0.2f, 0.5f);

        yield return null;
        
    }
}
