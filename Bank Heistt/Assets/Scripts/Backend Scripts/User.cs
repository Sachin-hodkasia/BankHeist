using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class User
{
    public string username;
    public string email;
    public string fbID;
    public string userID;
    public string deviceID;
    public string online ="false" ;
    
    public UserStats userStats;
    

    public User()
    {

    }

    public User(string username, string email, string fbID, string userID)
    {
        this.username = username;
        this.email = email;
        this.fbID = fbID;
        this.userID = userID;
    }
    public User(string username, string userID, string deviceID)
    {
        this.username = username;
        this.userID = userID;
        this.deviceID = deviceID;
    }

}
