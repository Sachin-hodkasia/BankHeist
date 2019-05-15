using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionCollider : MonoBehaviour {

    public int damage = 80;
    bool turningoff = false;
	// Use this for initialization
	void Start () {
      
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        if (turningoff == false)
        {
            StartCoroutine(turnOffCollider());
            turningoff = true;
        }
        Debug.Log(other.name + "  - se takraya hai yeh bomb");
        if (other.CompareTag("Player"))
        {
            
            other.GetComponent<PhotonView>().RPC("DealDamageToPlayer", PhotonTargets.All, new object[] { damage });
        }
    }
    
    IEnumerator turnOffCollider()
    {
        print("turnoffcollider called");
        yield return new WaitForSeconds(0.1f);
        GetComponent<Collider>().enabled = false;

        
    }
}
