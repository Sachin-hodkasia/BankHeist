using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : Photon.MonoBehaviour {

    public int money = 100;
    public bool picked=false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && picked == false)
        {
            picked = true;
            Player_Main pleya = other.gameObject.GetComponent<Player_Main>();
            PickUpMoney(pleya.photonView.viewID);    

        }
    }
    void PickUpMoney(int viewidOfPicker)
    {

        if (PhotonView.Find(viewidOfPicker).isMine)
        {
            photonView.RPC("OnPickUp", PhotonTargets.AllBuffered, new object[] { viewidOfPicker, money });
        }

    }


    [PunRPC]
    void OnPickUp(int viewidOfPicker , int money)
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<Animator>().SetTrigger("Picked");
        
        PhotonView pvPicker = PhotonView.Find(viewidOfPicker);
        pvPicker.gameObject.GetComponent<Player_Main>().moneyOnPlayer += money;
    }
}
