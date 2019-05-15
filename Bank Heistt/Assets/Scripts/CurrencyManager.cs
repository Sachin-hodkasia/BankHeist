using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CurrencyManager : MonoBehaviour {
    public static CurrencyManager Instance { get; set; }
    public int heistCoins,heistCash;

    public TextMeshProUGUI heistCashText, heistCoinsText;
	// Use this for initialization
	void Start () {
        Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
