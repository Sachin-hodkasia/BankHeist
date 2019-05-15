using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : Photon.MonoBehaviour {

    

    [Header("BulletProperties")]
    public Vector3 velocityOfBullet; // to be set by instantiation player
    public float bulletSpeed = 20;
    public int bulletDamage = 0;
    public int senderId;
    // Use this for initialization
    void Start() {
        
        StartCoroutine(OnTheCollider());            // it didnt work
        Destroy(gameObject, 5f);
        RaycastBack();
        GetComponent<Rigidbody>().velocity = velocityOfBullet.normalized * bulletSpeed;
    }

    // Update is called once per frame
    void Update() {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        print(other.gameObject.name.ToString());
        if (other.gameObject.GetPhotonView() != null)   // if collided with a thing with photonview
        {
            //TODO: show offline hit effect


            //int collisionOwnerId = other.gameObject.GetPhotonView().ownerId;
            int collisionviewId = other.gameObject.GetPhotonView().viewID; // exact id of the gameObject which was hit
            if(other.gameObject.GetPhotonView().photonView.isMine==true)//if the bullet hit him on his screen
            {
                photonView.RPC("BulletHit", PhotonTargets.All, new object[] { collisionviewId });
                
            }

        }
        
        //DestroyBullet("Bina photon view wala");
    }

    void RaycastBack()
    {
        Vector3 rayDir = -velocityOfBullet ;
        Ray ray = new Ray(transform.position, rayDir);
        Debug.DrawRay(transform.position, rayDir);
        RaycastHit hit;
        if(Physics.Raycast(ray , out hit))
        {

            PhotonView hitPhotonView = hit.transform.gameObject.GetPhotonView();
            if(hitPhotonView != null  && hitPhotonView.ownerId != senderId)
            {
                photonView.RPC("BulletHit", PhotonTargets.All, new object[] { hitPhotonView.viewID });
            }
        }
    }
    IEnumerator OnTheCollider()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        GetComponent<Collider>().enabled = true;

    }
    [PunRPC]
    void BulletHit(int hit_viewId ,PhotonMessageInfo info)
    {
        Player_Main pleyaHit = PhotonView.Find(hit_viewId).gameObject.GetComponent<Player_Main>();
        if (pleyaHit != null)
        {
            pleyaHit.photonView.RPC("DealDamageToPlayer", PhotonTargets.All, new object[] { bulletDamage });
        }

        Bank bank = PhotonView.Find(hit_viewId).GetComponent<Bank>();
        if(bank != null)
        {
            bank.photonView.RPC("DamageDealtToBank", PhotonTargets.All, new object[] { bulletDamage});
        }

        
        DestroyBullet("Photon view wala");
    }
    void DestroyBullet(string bhida)
    {
        //TODO: Show vfx
        Debug.Log("BUllet destroyed called is se bhidey - " +bhida);
        Destroy(gameObject);
    }
}
