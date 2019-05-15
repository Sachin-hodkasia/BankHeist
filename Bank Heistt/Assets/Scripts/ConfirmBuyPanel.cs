using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmBuyPanel : MonoBehaviour {
    public int HeistCoins;
    public int HeistCash;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void SetHeistCoins(int value)
    {
        HeistCoins = value;
    }

    public void SetHeistCash(int value)
    {
        HeistCash = value;
    }
}
