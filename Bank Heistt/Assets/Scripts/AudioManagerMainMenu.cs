using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManagerMainMenu : MonoBehaviour {

    //-----------------------------------------------------------------------Dont change name of any audio clip or else it wont run----------------
    public static AudioManagerMainMenu Instance { get; set; }
    [Header("UIFX")]
    public Sound[] Sounds;


    [Header("Theme")]
    public AudioSource ThemeSound;
    public AudioClip[] ThemeMusicList;
    int currentThemeIndex;
    // Use this for initialization
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        
        foreach(Sound s in Sounds)
        {
            s.audioSource = gameObject.AddComponent<AudioSource>();
            s.audioSource.playOnAwake = false;
            s.audioSource.clip = s.clip;
            s.audioSource.volume = s.volume;
            s.audioSource.pitch = s.pitch;
        }
    }
    void Start () {
        MainMenuEntry();
        currentThemeIndex =  Random.Range(0, ThemeMusicList.Length);
        ThemeSound.clip = ThemeMusicList[currentThemeIndex];
        ThemeSound.Play();
	}
	
	// Update is called once per frame
	void Update () {
		if(ThemeSound.isPlaying == false)
        {
            currentThemeIndex= (currentThemeIndex + 1) % ThemeMusicList.Length;
            ThemeSound.clip = ThemeMusicList[currentThemeIndex];
            ThemeSound.Play();
        }
	}

    public void MainMenuEntry()
    {
        Play("Loading",true);
        Play("TypingBeep", true);
        Play("Screwd", true);
    }
    
    public void AchievmentPanelEntry()
    {
        Play("NormalClick",false);
        Play("CloseSimpleShatter",false);
    }

    public void NormalClick()
    {
        Play("NormalClick", false);
    }

    public void AchievmentPanelExit()
    {
        Play("CloseSimpleShatter", false);
    }

    public void LeaderboardPanelEntry()
    {
        Play("NormalClick", false);
        Play("CloseSimpleShatter", false);
    }















    //this is the basic to play any sound
    public void Play(string nameOfSound , bool pitchIncrease)
    {
        foreach (Sound s in Sounds)
        {
            if (s.nameOfSound == nameOfSound)
            {
                s.audioSource.Play();
                if (pitchIncrease)
                {
                    s.audioSource.pitch = 1.5f;
                }

            }
        }
    }
}
