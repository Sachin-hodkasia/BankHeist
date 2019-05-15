using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using Facebook.Unity;
using UnityEngine.UI;
using Firebase;
using Firebase.Unity.Editor;

public class SplashScreen : MonoBehaviour {

    FirebaseAuth auth;
	// Use this for initialization
	void Awake () {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://bankhiest.firebaseio.com/");
        auth = FirebaseAuth.DefaultInstance;
        FB.Init(OnInitComplete);
    }

    void OnInitComplete()
    {
        if (FB.IsLoggedIn)
        {
            if (auth.CurrentUser != null)
            {
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                SceneManager.LoadScene("LoginPage");
            }
        }
        else
        {
            Debug.Log("FB NOT Logged in during Init");
            if (auth.CurrentUser != null)
            {
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                SceneManager.LoadScene("LoginPage");
            }
        }
    }


    // Update is called once per frame
    void Update () {
		
	}

  
}
