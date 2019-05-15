using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MafiaHouse : Photon.MonoBehaviour {
    public TextMeshProUGUI timerText;
    int maxMoney = 1000; //TODO: get this from roomProperties
    public float GameTime = 40f;
    float onlineGameTime = 40f;
    float maxTime = 40f;
	void Start () {
		
	}
	
	void Update () {
        if (photonView.isMine) {
            GameTime -= Time.deltaTime;
            if (GameTime < 0)
            {
                photonView.RPC("ResetGame", PhotonTargets.All, new object[] { });
            }
        }
        else
        {
            GameTime = onlineGameTime;
        }
        timerText.text = GameTime.ToString("0") + "s";
    }
    private void OnCollisionEnter(Collision collision)
    {
        GameObject other =  collision.gameObject;
        if(other.tag == "Player")
        {
            Player_Main pleyaCollided = other.GetComponent<Player_Main>();
            


            if (pleyaCollided.moneyOnPlayer >= maxMoney/2 )
            {
                
                // reward player on the basis of the money on player
                // reset the gamescene
                //leaderboard mein update krdo values ko TODO:
                if(pleyaCollided.moneyOnPlayer >= maxMoney/2  && pleyaCollided.moneyOnPlayer < maxMoney * 8 / 10)// 50 - 80% wala case hai yeh
                {
                    //reward accordingly
                    pleyaCollided.score += 50;
                   
                        
                }
                if(pleyaCollided.moneyOnPlayer >= (maxMoney *8) / 10 && pleyaCollided.moneyOnPlayer < maxMoney ) //80-100% wala case hai yeh
                {
                    pleyaCollided.score += 80;
                    
                }
                if(pleyaCollided.moneyOnPlayer == maxMoney)//100% wala case hai yeh
                {
                    pleyaCollided.score += 100;
                }

                photonView.RPC("SpawnGameMoney", PhotonTargets.All, new object[] { });
                photonView.RPC("ResetGame", PhotonTargets.AllBuffered, new object[] { });
            }
        }
    }
    [PunRPC]
    public void ResetGame()
    {
        GameTime = maxTime;
        Player_Main[] players = GameObject.FindObjectsOfType<Player_Main>();
        foreach (Player_Main pleya in players)
        {
            pleya.SetPlayerInRoom(); // this includes resetting player properties
        }

        GameObject bankHolder = GameObject.FindGameObjectWithTag("Banks");
        for(int i=0; i<bankHolder.transform.childCount; i++)
        {
            bankHolder.transform.GetChild(i).transform.GetChild(0).gameObject.GetComponent<Bank>().ResetBank();
        }


        
    }
    [PunRPC]
    void SpawnGameMoney()
    {

        GameObject[] money = GameObject.FindGameObjectsWithTag("Money");
        foreach (GameObject go in money)
        {
            Destroy(go, 0f);
        }
        GameObject MoneyPoints = GameObject.FindGameObjectWithTag("MoneyPoints");
        int points = MoneyPoints.transform.childCount;

        for (int i = 0; i < points; i++)
        {
            Transform point = MoneyPoints.transform.GetChild(i);
            PhotonNetwork.InstantiateSceneObject("Money", point.position, Quaternion.identity, 0, new object[] { });
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(GameTime);
        }
        else
        {
            onlineGameTime = (float)stream.ReceiveNext();
        }
    }

}