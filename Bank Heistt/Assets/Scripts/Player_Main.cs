using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.UI;
public class Player_Main : Photon.MonoBehaviour {



    Ray ray;
    RaycastHit hit;
    public GameObject Gun;
    Vector3 AimingDirection;
    PlayerMovement playerMovementscript;
    public TextMesh cashText;
    public GameObject direction;
    public GameObject body;
    bool moving = false;
    bool rotating = false;
    bool emergency = false;
    public Transform spawnPoint;
    public GameObject bomb;
    public GameObject GunBase;

    [Header("PlayerProperties")]
    int GunBaseIndex = 0;
    int bulletIndex = 4;
    public int health_player = 100;
    int damageForBullet=20;
    public float reloadTimer=0;
    int ignoredTouch = -9;
    public GameObject[] bullets;
  
    public int moneyOnPlayer = 0;
    Touch[] touches;
    bool moneySpawned = false;
    Transform mySpawnPoint;
    public int score=0;
    public Image healthImage;


    [Header("RoomProperties")]
    int maxMoney = 1000;
    public bool GrenadeMode = true;
    Button bombButton;
    void Start () {

        
        foreach(PhotonPlayer player in PhotonNetwork.playerList)
        {
            print(player.NickName);
        }
        if (photonView.isMine)
        {
            
            playerMovementscript = GetComponent<PlayerMovement>();
            SetPlayerInRoom();
            if (photonView.isMine && photonView.ownerId == 1 && moneySpawned == false)
            {
                photonView.RPC("SpawnMoney", PhotonTargets.AllBuffered, new object[] { });
                moneySpawned = true;
            }
        }
        
        bombButton = GameObject.Find("BombButton_0").GetComponent<Button>();
        bombButton.onClick.AddListener(ChangeGrenadeMode);
        if(bombButton == null)
        {
            print("bomb button not found");
        }
        //bulletIndex = photonView.instantiationData[];
        //GunBaseIndex = photonView.instantiationData[];
        GunBase.transform.GetChild(GunBaseIndex).gameObject.SetActive(true);
        touches = Input.touches;
    }
    
	
	// Update is called once per frame
	void Update () {

        cashText.text = "$" + moneyOnPlayer.ToString();
        cashText.transform.LookAt(Camera.main.transform);

       
        if (photonView.isMine)
        {
            healthImage.fillAmount = Mathf.Lerp(healthImage.fillAmount, health_player/100f, Time.deltaTime * 10f);
            int counter = 0;
            #region directioning at the top player
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < PhotonNetwork.room.PlayerCount; i++)
            {
                //---------------------------------------              THIS IS FOR THE HAND AIMING AT THE TOP PLAYER --------------------------------------
                if (players[i].GetComponent<Player_Main>().moneyOnPlayer > maxMoney / 2 && players[i] != gameObject)
                {
                    //Debug.Log("Player detected , money on player = " + players[i].GetComponent<Player_Main>().moneyOnPlayer.ToString());
                    direction.SetActive(true);
                    direction.transform.LookAt(players[i].transform);
                    emergency = true;
                    counter++;
                }
                
            }
            if (counter == 0)
            {
                direction.SetActive(false);
            }
            #endregion
            reloadTimer -= Time.deltaTime;
            if(playerMovementscript.moveVector == Vector3.zero)
            { moving = false; }
            else { moving = true; }
            
            touches = Input.touches;
            if(touches.Length == 0)
            {
                ignoredTouch = -9;
            } // resetting the ignored touch when touches become 0


            #region shooting Mechanism



           
            for (int i =0; i<2; i++)
            {
                
               if (touches[i].phase == TouchPhase.Began  && EventSystem.current.IsPointerOverGameObject(touches[i].fingerId))
                {
                   
                    ignoredTouch = touches[i].fingerId ;
                    
               
                }// detecting the specific touch to ignore
               if(touches[i].fingerId == ignoredTouch)
                {
                    continue;
                }// if this touch is to be ignored then continue; else aim with this one only
               else
                {
                    
                    Touch aimTouch = touches[i];
                    ray = Camera.main.ScreenPointToRay(aimTouch.position);
                    //Debug.DrawRay(ray.origin, ray.direction * 200, Color.red);
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {

                        
                        AimingDirection = hit.point - Gun.transform.position;
                        Debug.DrawRay(Gun.transform.position, AimingDirection * 200, Color.red);
                        AimAlongThis(AimingDirection);
                        
                        
                       
                        
                    }
                    if (aimTouch.phase == TouchPhase.Ended && GrenadeMode == false)
                    {

                        ShootBullet();
                        rotating = false;
                    }
                    if(aimTouch.phase == TouchPhase.Ended && GrenadeMode == true)
                    {

                        ThrowGrenade(hit.point , spawnPoint.position);
                    }
                }
            } // main mechanisms detecting only 2 touches
            #endregion

            #region PC controls
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ShootBullet();
            }
            #endregion
        }
        


    }

    void ChangeGrenadeMode()
    {
        GrenadeMode = !GrenadeMode;
    }
    public void SetPlayerInRoom()
    {
        #region position and collider setting
        GameObject sp1;
        sp1 = GameObject.FindGameObjectWithTag("SpawnPoint");
        mySpawnPoint = sp1.transform.GetChild(photonView.ownerId-1);
        transform.position = mySpawnPoint.position;
        GetComponent<Collider>().enabled = true;
        body.SetActive(true);
        #endregion
        #region setting player properties to initial values

        photonView.RPC("ResetPlayerProperties", PhotonTargets.All, new object[] { photonView.viewID });

        #endregion

    }
    void AimAlongThis(Vector3 direction)
    {
        Quaternion rot = Quaternion.LookRotation(direction);
        rot.x = 0;
        rot.z = 0;
        Gun.transform.rotation = Quaternion.Lerp(Gun.transform.rotation, rot, 25f * Time.deltaTime);
        
    }
    public void ShootBullet()
    {
        if (reloadTimer <= 0)
        {
            Vector3 velocityDir = AimingDirection.normalized;
            velocityDir.y = 0;
            PhotonPlayer shootingPleya= PhotonNetwork.player;
            int latency = PhotonNetwork.GetPing();
            int senderId = photonView.ownerId ;
            photonView.RPC("OnShoot", PhotonTargets.All, new object[] {senderId , latency,bulletIndex , spawnPoint.position , spawnPoint.rotation, damageForBullet , velocityDir,shootingPleya });
            
            reloadTimer = 0.01f;
        }

    }
    public bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
    void ThrowGrenade(Vector3 target , Vector3 origin)
    {
        GameObject bumb = (GameObject)PhotonNetwork.InstantiateSceneObject("Bomb", origin , new Quaternion(0,0,0,0) ,0,new object[] { CalculateThrowableVelocity(target , origin , 1.5f) });
    }
    Vector3 CalculateThrowableVelocity(Vector3 target , Vector3 origin , float time)
    {
        //define the x and y first
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0;

        //create float to represent the distance
        float Sy = distance.y;
        float Sxz = distanceXZ.magnitude;

        float Vxz = Sxz / time;
        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;

        return result;

    }
    [PunRPC]
    void ResetPlayerProperties(int viewId)
    {
        PhotonView pv = PhotonView.Find(viewId);
        pv.gameObject.GetComponent<Player_Main>().moneyOnPlayer = 0;
        pv.gameObject.GetComponent<Player_Main>().health_player = 100;
        pv.gameObject.GetComponent<Player_Main>().reloadTimer = 0;
        /*moneyOnPlayer = 0;
        health_player = 100;
        reloadTimer = 0;*/
    }
    [PunRPC]
    void SpawnMoney()
    {
        GameObject MoneyPoints = GameObject.FindGameObjectWithTag("MoneyPoints");
        int points = MoneyPoints.transform.childCount;
      
        for(int i=0;i<points; i++)
        {
            Transform point = MoneyPoints.transform.GetChild(i);
            PhotonNetwork.InstantiateSceneObject("Money", point.position, Quaternion.identity , 0 , new object[] { } );
        }
    }
    [PunRPC]
    void OnShoot(int senderId ,int latency ,int bulletIndex ,Vector3 spawnPoint, Quaternion spawnRotation,  int damage , Vector3 velocity ,PhotonPlayer shootingPlayer, PhotonMessageInfo info)
    {
        //double timeSinceSpawn = PhotonNetwork.time - info.timestamp;
        
        if(shootingPlayer == PhotonNetwork.player) // if meri screen pe hi maine fire kra hai
        {
            GameObject bulletvfx =(GameObject) GameObject.Instantiate(bullets[bulletIndex] , spawnPoint , spawnRotation);
            bullet bulletScript = bulletvfx.GetComponent<bullet>();
            bulletScript.velocityOfBullet = velocity;
            bulletScript.bulletDamage = damage;
            bulletScript.senderId = senderId;
            //bulletScript.bulletSpeed = something
        }
        else
        {
            //print("time since bullet spawn = " + timeSinceSpawn.ToString()  +"  and ping received was -"   + latency.ToString()) ;
            Vector3 offsetPosition;
            float lag = (float)(latency / 100);
            offsetPosition = (velocity * lag)  *  1.5f  ;  // 1.5 is just to offset
            GameObject bulletvfx = (GameObject) GameObject.Instantiate(bullets[0], spawnPoint+offsetPosition, spawnRotation);
            bullet bulletScript = bulletvfx.GetComponent<bullet>();
            bulletScript.velocityOfBullet = velocity;
            bulletScript.bulletDamage = damage;
            bulletScript.senderId = senderId;
            //bulletScript.bulletSpeed = something
            //
        }
    }
 
    [PunRPC]
    void DealDamageToPlayer(int damage)
    {
        health_player -= damage;
        if(health_player <= 0 )
        {
            print("Dead nibba");
            KillPlayer();
           
        }
    }
    [PunRPC]
    void KillPlayer()
    {
        for (int i = 0; i < moneyOnPlayer / 100; i++)
        {
            PhotonNetwork.InstantiateSceneObject("Money", transform.position + new Vector3(Random.Range(0, 3), 0, Random.Range(0, 3)), Quaternion.identity, 0, new object[] { });
        }
        GetComponent<Collider>().enabled = false;
        body.SetActive(false);
       
        SetPlayerInRoom();
        //Destroy(gameObject, 0.01f);
    }

    void OnPhotonSerializeView(PhotonStream stream , PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            stream.SendNext(score);
            stream.SendNext(moneyOnPlayer);
            
        }
        else
        {
            score = (int)stream.ReceiveNext();//TODO: optimize score and money updation on data usage
            moneyOnPlayer = (int)stream.ReceiveNext();
         
        }
    }
    private void OnDestroy()
    {
        KillPlayer();
    }

}
