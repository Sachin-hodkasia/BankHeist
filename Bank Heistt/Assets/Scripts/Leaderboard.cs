using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboard : Photon.MonoBehaviour {
    public List<GameObject> players;
    public GameObject rankElement;
	// Use this for initialization
	void Start () {
        // starting ke population
		foreach(GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            
            players.Add(go);
            AddRankElement(go.GetComponent<Player_Main>().photonView.owner.NickName, go.GetComponent<Player_Main>().moneyOnPlayer, go.GetComponent<Player_Main>().photonView.viewID);
        }
	}
	
	// Update is called once per frame
	void Update () {
        UpdateLeaderboard();
	}

    void UpdateLeaderboard()
    {
        #region Adding elements
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            if(players.Contains(go) == false)
            {
                players.Add(go);
                AddRankElement(go.GetComponent<Player_Main>().photonView.owner.NickName, go.GetComponent<Player_Main>().moneyOnPlayer , go.GetComponent<Player_Main>().photonView.viewID);
            }
            else
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (go.GetComponent<Player_Main>().photonView.viewID == transform.GetChild(i).gameObject.GetComponent < RankElement>().viewId)
                    {
                        // Debug.Log( go.GetComponent<Player_Main>().photonView.owner.NickName "    -   yeh hai nickname bhai ka");
                        transform.GetChild(i).gameObject.GetComponent<RankElement>().moneyString.text = "$" + go.GetComponent<Player_Main>().moneyOnPlayer.ToString() ;
                        transform.GetChild(i).gameObject.GetComponent<RankElement>().moneyValue = go.GetComponent<Player_Main>().moneyOnPlayer;
                    }
                }
            }
        }
        #endregion
        //element's children is populated right now

        #region Sorting and ranking visual
        for (int k = 0; k < transform.childCount-1; k++)
        {
            if (transform.GetChild(k).gameObject.GetComponent<RankElement>().moneyValue  < transform.GetChild(k+1).gameObject.GetComponent<RankElement>().moneyValue)
            {
                //sorting required
                transform.GetChild(k).SetSiblingIndex(k+1);
                
            }
            
        }
        for (int k = 0; k < transform.childCount ; k++)
        {
            transform.GetChild(k).gameObject.GetComponent<RankElement>().rankString.text = (k + 1).ToString();
        }

        #endregion

    }

    void AddRankElement(string name , int money , int viewId)
    {
        GameObject element = Instantiate(rankElement, transform);
        element.GetComponent<RankElement>().name.text = name;
        element.GetComponent<RankElement>().moneyString.text = "$" + money.ToString();
        element.GetComponent<RankElement>().moneyValue = money;
        element.GetComponent<RankElement>().viewId = viewId;

    }
}
