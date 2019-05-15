using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pm_mainMenu : MonoBehaviour {
    public Text connectingTxt,PlayerNameInput;
	// Use this for initialization
	void Start () {
        if (PhotonNetwork.connected == false)
        {
           
            SceneManager.sceneLoaded += OnSceneLoaded;
            PhotonNetwork.ConnectUsingSettings("1");
        }
        if (PhotonNetwork.connected)
        {
            connectingTxt.text = "Connected";
            print("Connected");
                
        }
    }
    private void OnConnectedToMaster()
    {
        connectingTxt.text = "Connected";
        connectingTxt.color = Color.green;
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }
    private void OnFailedToConnect(DisconnectCause cause)
    {
        Debug.Log("CHeck internet connection -" + cause.ToString());
        Debug.Log("Retrying.....");
        PhotonNetwork.ConnectUsingSettings("1");
    }
    void OnJoinedLobby()
    {
        connectingTxt.text = "Join Game";
    }
    void OnSceneLoaded(Scene scene , LoadSceneMode mode)
    {
        if (scene.name == "GamePlayTest")
        {
            GameObject sp1;
            int x = Random.Range(0, 2);
            sp1 = GameObject.FindGameObjectWithTag("SpawnPoint").transform.GetChild(x).gameObject;

            
            PhotonNetwork.Instantiate("LamboCarCMOnline", sp1.transform.position, Quaternion.identity, 0, new object[] { });

            int numOfPlayersIncurrentRoom;
            numOfPlayersIncurrentRoom = PhotonNetwork.room.PlayerCount;
            if (numOfPlayersIncurrentRoom == 4)
            {
                //Debug.Log("Players in room are 2");
                PhotonNetwork.room.IsVisible = false;
            }
        }
    }
    public void FindAndJoinMatch() // how to join random with expected Properties ///////////////////////////////////////////////////////////////////////////////////////
    {
            PhotonNetwork.JoinRandomRoom();
    }
    void OnPhotonRandomJoinFailed()
    {
            Debug.Log("No room found , creating custom match");
            RoomOptions roomProperties = new RoomOptions();
            roomProperties.IsVisible = true;
            roomProperties.MaxPlayers = 4;
            PhotonNetwork.JoinOrCreateRoom(Random.Range(0, 99999999).ToString(), roomProperties, TypedLobby.Default);
        
    }
    void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GamePlayTest");
        //StartCoroutine(WaitForPlayersThenJoin());

        //PhotonNetwork.LoadLevel("MultiPlayerGameScene");
    }
    IEnumerator WaitForPlayersThenJoin()
    {
        while (true)
        {
            //yield return new WaitForSeconds(1f);
            if (PhotonNetwork.room != null && PhotonNetwork.room.PlayerCount == 1)
            {

                connectingTxt.text = "Room Created , waiting..";

            }


            if (PhotonNetwork.room != null && PhotonNetwork.room.PlayerCount == 2)
            {
                // PhotonNetwork.LoadLevel("MultiPlayerGameScene");
                PhotonNetwork.LoadLevel("GamePlayTest");
            }
            yield return null;
        }
    }
    // Update is called once per frame
    void Update () {
        if (PlayerNameInput.text != "")
        {
            PhotonNetwork.player.NickName = PlayerNameInput.text;
        }else
        {
            PhotonNetwork.player.NickName = "No nick name" + Random.Range(0, 10).ToString();
        }
	}
}
