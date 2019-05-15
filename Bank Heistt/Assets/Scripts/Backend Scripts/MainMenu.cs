using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using Firebase.Auth;
using Facebook.Unity;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    DatabaseReference reference;
    public TextMeshProUGUI userName;
    public TextMeshProUGUI coinsGUI;
    public TextMeshProUGUI cashGUI;
    public Text kills;
    public Text matchPlayed;
    public Text betsWon;
    public Text streak;
    public GameObject Friend;
    public GameObject FriendListUI;
    public Button logOutButton;

    string userNametext = "";
    string killstext="Kills : ";
    string matchPlayedtext= "Match Played : ";
    string betsWonText="Bets Won : ";
    string streakText="Streak : ";
    string coinsText = "0";
    string cashText = "0";
    string deviceID = "";

    int killsInt = 0;
    int matchPlayedInt = 0;
    int betsWonInt = 0;
    int streakInt = 0;
    int coinsInt = 0;
    int cashInt = 0;

    [SerializeField]
    List<string> FacebookFriendsIDList = new List<string>();
    FirebaseAuth auth;
    FirebaseUser user;
    User localUser;

    public string url;
    public Image profileImage;
    bool reload = false;

    // Use this for initialization
    void Awake () {
        deviceID = SystemInfo.deviceUniqueIdentifier.ToString();
        reload = false;
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://bankhiest.firebaseio.com/");
        auth = FirebaseAuth.DefaultInstance;
        // Get the root reference location of the database.
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        if (auth.CurrentUser != null)
        {
            user = auth.CurrentUser;
        }
        else
        {
            Debug.Log("Error");
        }
        if (!user.IsAnonymous)
        {
            reference.Child("users").Child(AccessToken.CurrentAccessToken.UserId.ToString()).Child("online").SetValueAsync("true");

            //Update All Firebase users now

            localUser = new User(user.DisplayName, user.Email, AccessToken.CurrentAccessToken.UserId, user.UserId);
            FB.API("me/picture?type=square&height=350&width=350", HttpMethod.GET, GetPicture);
            RetrivefromDatabase(localUser);
        }
        else
        {
            reference.Child("users").Child(deviceID).Child("online").SetValueAsync("true");
            localUser = new User(user.DisplayName, user.UserId,deviceID);
            RetrivefromDatabaseAnonymousUser(localUser);
        }
    }

    public void Start()
    {
        if (!user.IsAnonymous)
        {
            GetFriendsPlayingThisGame();
        }
    }
    // Update is called once per frame
    void Update () {
        kills.text = killstext;
        matchPlayed.text = matchPlayedtext;
        betsWon.text = betsWonText;
        streak.text = streakText;
        userName.text = userNametext;
        coinsGUI.text = coinsText;
        cashGUI.text = cashText;
        
        if (reload)
        {
            //reference.Child("users").Child(deviceID).SetValueAsync(null);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("GamePlayTest");
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Do something with the data in args.Snapshot
        if (args.Snapshot.Key.ToString() == "kills")
        {
            killsInt = int.Parse(args.Snapshot.Value.ToString());
            killstext = "Kills : " + args.Snapshot.Value.ToString();
        }
        else if (args.Snapshot.Key.ToString() == "matchPlayed")
        {
            matchPlayedInt = int.Parse(args.Snapshot.Value.ToString());
            matchPlayedtext = "Match Played : " + args.Snapshot.Value.ToString();
        }
        else if (args.Snapshot.Key.ToString() == "betsWon")
        {
            betsWonInt = int.Parse(args.Snapshot.Value.ToString());
            betsWonText = "Bets Won : " + args.Snapshot.Value.ToString();
        }
        else if (args.Snapshot.Key.ToString() == "streak")
        {
            streakInt = int.Parse(args.Snapshot.Value.ToString());
            streakText = "Streak : " + args.Snapshot.Value.ToString();
        }
        else if (args.Snapshot.Key.ToString() == "coins")
        {
            coinsInt = int.Parse(args.Snapshot.Value.ToString());
            coinsText = args.Snapshot.Value.ToString();
        }
        else if (args.Snapshot.Key.ToString() == "cash")
        {
            cashInt = int.Parse(args.Snapshot.Value.ToString());
            cashText = "$" + args.Snapshot.Value.ToString();
        }
        else if (args.Snapshot.Key.ToString() == "username")
            userNametext = args.Snapshot.Value.ToString();
    }

    void RetrivefromDatabase(User localUser)
    {
        FirebaseDatabase.DefaultInstance.GetReference("users/" + localUser.fbID + "/" +"userStats/kills").ValueChanged += HandleValueChanged;
        FirebaseDatabase.DefaultInstance.GetReference("users/" + localUser.fbID + "/" + "userStats/matchPlayed").ValueChanged += HandleValueChanged;
        FirebaseDatabase.DefaultInstance.GetReference("users/" + localUser.fbID + "/" + "userStats/betsWon").ValueChanged += HandleValueChanged;
        FirebaseDatabase.DefaultInstance.GetReference("users/" + localUser.fbID + "/" + "userStats/streak").ValueChanged += HandleValueChanged;
        FirebaseDatabase.DefaultInstance.GetReference("users/" + localUser.fbID + "/" + "userStats/coins").ValueChanged += HandleValueChanged;
        FirebaseDatabase.DefaultInstance.GetReference("users/" + localUser.fbID + "/" + "userStats/cash").ValueChanged += HandleValueChanged;
        FirebaseDatabase.DefaultInstance.GetReference("users/" + localUser.fbID + "/" + "username").ValueChanged += HandleValueChanged;
    }

    void RetrivefromDatabaseAnonymousUser(User localUser)
    {
        FirebaseDatabase.DefaultInstance.GetReference("users/" + deviceID + "/" + "userStats/kills").ValueChanged += HandleValueChanged;
        FirebaseDatabase.DefaultInstance.GetReference("users/" + deviceID + "/" + "userStats/matchPlayed").ValueChanged += HandleValueChanged;
        FirebaseDatabase.DefaultInstance.GetReference("users/" + deviceID + "/" + "userStats/betsWon").ValueChanged += HandleValueChanged;
        FirebaseDatabase.DefaultInstance.GetReference("users/" + deviceID + "/" + "userStats/streak").ValueChanged += HandleValueChanged;
        FirebaseDatabase.DefaultInstance.GetReference("users/" + deviceID + "/" + "userStats/coins").ValueChanged += HandleValueChanged;
        FirebaseDatabase.DefaultInstance.GetReference("users/" + deviceID + "/" + "userStats/cash").ValueChanged += HandleValueChanged;
        FirebaseDatabase.DefaultInstance.GetReference("users/" + deviceID + "/" + "username").ValueChanged += HandleValueChanged;
    }

    public void ConvertAnonymousUserToPermanentUser()
    {
        if (user.IsAnonymous)
        {
            FBlogin();
        }
    }

    private void FBlogin()
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
           
            // AccessToken class will have session details
            var aToken = AccessToken.CurrentAccessToken;
            // Print current access token's User 


            Credential credential = FacebookAuthProvider.GetCredential(aToken.TokenString);
            user.LinkWithCredentialAsync(credential).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithCredentialAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    auth.SignOut();
                    Debug.Log("Called1");
                    auth.SignInWithCredentialAsync(credential).ContinueWith(task2 =>
                    {
                        if (task2.IsCanceled)
                        {
                            return;
                        }
                        if (task2.IsFaulted)
                        {
                            return;
                        }
                        if (task2.IsCompleted)
                        {
                            reference.Child("users").Child(deviceID).Child("online").SetValueAsync("false");
                            user = task2.Result;
                            reload = true;
                        }
                    });
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Logged In");
                    user = task.Result;
                    localUser = new User(userNametext, user.Email, aToken.UserId, user.UserId);
                    writeNewUser(user.UserId, userNametext, user.Email, aToken.UserId);
                }

            });

        }
        else
        { 
            Debug.Log("User cancelled login");
        }
    }

    private void writeNewUser(string userId, string name, string email, string fbID)
    {
        FirebaseDatabase.DefaultInstance.GetReference("users/" + fbID).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.Log("FB ID Not Found");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Value == null)
                {
                    Debug.Log("Called");
                    User user = new User(name, email, fbID, userId);
                    UserStats userStats = new UserStats(killsInt, matchPlayedInt, betsWonInt, streakInt, coinsInt, cashInt);
                    string json = JsonUtility.ToJson(user);
                    string statsJson = JsonUtility.ToJson(userStats);
                    reference.Child("users").Child(fbID).SetRawJsonValueAsync(json);
                    reference.Child("users").Child(fbID).Child("userStats").SetRawJsonValueAsync(json);
                    reload = true;
                }
                else
                {
                    reload = true;
                }
            }
        });

    }

    #region Facebook Functions
    private void GetPicture(IGraphResult result)
    {
        if (result.Error == null && result.Texture != null)
        {
      
            profileImage.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 350, 350), new Vector2());
        }
    }

    public void FacebookShare()
    {
        FB.ShareLink(new System.Uri("https://resocoder.com"), "Check it out!",
            "Good programming tutorials lol!",
            new System.Uri("https://resocoder.com/wp-content/uploads/2017/01/logoRound512.png"));
    }
   
    public void FacebookGameRequest()
    {
        FB.AppRequest("Hey! Come and play this awesome game!", title: "Bank Heiest");
    }

    public void FacebookInvite()
    {
        FB.Mobile.AppInvite(new System.Uri("https://play.google.com/store/apps/details?id=com.tappybyte.byteaway"));
    }

    public void GetFriendsPlayingThisGame()
    {
        string query = "/me/friends";
        FB.API(query, HttpMethod.GET, result =>
        {
            var dictionary = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(result.RawResult);
            var friendsList = (List<object>)dictionary["data"];
            foreach (var dict in friendsList)
            {
                FacebookFriendsIDList.Add(((Dictionary<string, object>)dict)["id"].ToString());
                CreateFriend(((Dictionary<string, object>)dict)["name"].ToString(), ((Dictionary<string, object>)dict)["id"].ToString());
            }
        });
    }

    void CreateFriend(string name, string id)
    {
        GameObject myFriend = Instantiate(Friend);
        Transform parent = FriendListUI.transform;
        myFriend.transform.SetParent(parent);
        myFriend.transform.localScale = Vector3.one;
        myFriend.GetComponentInChildren<TextMeshProUGUI>().text = name;
        FB.API(id + "/picture?width=100&height=100", HttpMethod.GET, delegate (IGraphResult result) {
            myFriend.GetComponentsInChildren<Image>()[0].sprite = Sprite.Create(result.Texture, new Rect(0, 0, 100, 100), new Vector2(0.5f, 0.5f));
        });
    }

    public void Challenge()
    {
        FB.AppRequest("Custom message", null, new List<object> { "app_users" }, null, null, "Data", "Challenge your friends!", ChallengeCallback);
    }

    void ChallengeCallback(IAppRequestResult result)
    {
        if (result.Cancelled)
        {
            Debug.Log("Challenge cancelled.");
        }
        else if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("Error in challenge:" + result.Error);
        }
        else
        {
            Debug.Log("Challenge was successful:" + result.RawResult);
        }
    }

    public void OnApplicationQuit()
    {
        if (!user.IsAnonymous)
        {
            reference.Child("users").Child(AccessToken.CurrentAccessToken.UserId.ToString()).Child("online").SetValueAsync("false");
        }
        else
        {
            reference.Child("users").Child(deviceID).Child("online").SetValueAsync("false");
        }
    }

    #endregion
}
