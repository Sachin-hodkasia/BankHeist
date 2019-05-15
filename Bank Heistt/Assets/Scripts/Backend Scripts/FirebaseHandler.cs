using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase.Auth;
using Facebook.Unity;
using Firebase.Database;
using Firebase;
using Firebase.Unity.Editor;
using UnityEngine.SceneManagement;

public class FirebaseHandler : MonoBehaviour
{
    string deviceId;
    FirebaseUser user;
    DatabaseReference mReference;
    bool loggedIn = false;
    bool guestButton = false;
    public Button FacebookLoginButton;
    public Button GuestLoginButton;
    public TMP_InputField userNameInputField;

    void Awake()
    {
        deviceId = SystemInfo.deviceUniqueIdentifier.ToString();
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://bankhiest.firebaseio.com/");
        if (!FB.IsInitialized)
        {
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                    FB.ActivateApp();
                else
                    Debug.LogError("Couldn't initialize");
            },
            isGameShown =>
            {
                if (!isGameShown)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;
            });
        }
        else
            FB.ActivateApp();
    }

    public void FBlogin()
    {
        List<string> permissions = new List<string>();
        permissions.Add("public_profile");
        permissions.Add("email");
        permissions.Add("user_friends");
        FB.LogInWithReadPermissions(permissions, AuthCallback);
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            FirebaseAuth auth =
            FirebaseAuth.DefaultInstance;

            // AccessToken class will have session details
            var aToken = AccessToken.CurrentAccessToken;
            // Print current access token's User 


            Credential credential = FacebookAuthProvider.GetCredential(aToken.TokenString);
            auth.SignInWithCredentialAsync(credential).ContinueWith(task=> {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithCredentialAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    
                    Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                     return;
                }
                if (task.IsCompleted) {
                    user = task.Result;
                    writeNewUser(user.UserId, user.DisplayName, user.Email, aToken.UserId);
                }
                
            });
            FacebookLoginButton.interactable = false;
            GuestLoginButton.interactable = false;
        }
        else
        {
            FacebookLoginButton.interactable = true;
            GuestLoginButton.interactable = true;
            Debug.Log("User cancelled login");
        }
    }

    public void Start()
    {
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://bankhiest.firebaseio.com/");

        // Get the root reference location of the database.
        mReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void Update()
    {
        if (loggedIn)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void writeNewUser(string userId, string name, string email,string fbID)
    {
        FirebaseDatabase.DefaultInstance.GetReference("users/"+fbID).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.Log("FB ID Not Found");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Value==null)
                {
                    User user = new User(name, email, fbID, userId);
                    UserStats userStats = new UserStats(0, 0, 0, 0, 2500, 5);
                    string json = JsonUtility.ToJson(user);
                    string statsJson = JsonUtility.ToJson(userStats);
                    mReference.Child("users").Child(fbID).SetRawJsonValueAsync(json);
                    mReference.Child("users").Child(fbID).Child("userStats").SetRawJsonValueAsync(statsJson);
                    loggedIn = true;
                }
                else
                {
                    loggedIn = true;
                }
            }
        });
        
    }

    private void writeAnonymousUser(string name,string userId)
    {
        FirebaseDatabase.DefaultInstance.GetReference("users/" + deviceId).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.Log("FB ID Not Found");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Value == null)
                {
                    User user = new User(name, userId, deviceId);
                    UserStats userStats = new UserStats(0, 0, 0, 0, 2500, 5);
                    string json = JsonUtility.ToJson(user);
                    string statsJson = JsonUtility.ToJson(userStats);
                    mReference.Child("users").Child(deviceId).SetRawJsonValueAsync(json);
                    mReference.Child("users").Child(deviceId).Child("userStats").SetRawJsonValueAsync(statsJson);
                    loggedIn = true;
                }
                else
                {
                    loggedIn = true;
                }
            }
        });
        
    }

    public void AnonymousLogin()
    {
        if (!guestButton)
        {
            userNameInputField.gameObject.SetActive(true);
            FacebookLoginButton.gameObject.SetActive(false);
            guestButton = true;
        }
        else
        {
            GuestLoginButton.interactable = false;
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            auth.SignInAnonymouslyAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInAnonymouslyAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                    return;
                }

                FirebaseUser newUser = task.Result;
                writeAnonymousUser(userNameInputField.text, newUser.UserId);
                
            });
            
        }
    }

}

