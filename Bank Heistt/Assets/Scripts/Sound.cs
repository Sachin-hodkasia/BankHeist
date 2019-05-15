using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Sound{
    public AudioClip clip;
    [HideInInspector]
    public AudioSource audioSource;


    public string nameOfSound;
    [Range(0f, 1f)]
    public float volume;

    [Range(0.1f, 3f)]
    public float pitch;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
