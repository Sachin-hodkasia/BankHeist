using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bank : Photon.MonoBehaviour {

    public int health=100 , moneyOnBank=100;
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
    private void OnCollisionEnter(Collision collision)
    {
    }
    [PunRPC]
    public void DamageDealtToBank(int damage)
    {
        if (health > 0)
        {
            health -= damage;
            if (health <= 0)
            {
               // photonView.RPC("DestroyBank", PhotonTargets.All, new object[] { });
                DestroyBank();
            }
        }
    }
    void OnPhotonSerializeView(PhotonStream stream , PhotonMessageInfo info)
    {

    }

   // [PunRPC]
    public void DestroyBank()
    {
        //throwing money around
        // TODO: explosion effects
        for (int i = 0; i < moneyOnBank / 100; i++)
        {
            PhotonNetwork.InstantiateSceneObject("Money", transform.position + new Vector3(Random.Range(0, 3), -transform.position.y + 1f, Random.Range(0, 3)), Quaternion.identity, 0, new object[] { });
        }

        gameObject.SetActive(false);

    }

    public void ResetBank()
    {
        gameObject.SetActive(true);
        health = 100;
    }
}
