using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canvassccript : MonoBehaviour {
	public Animator upper, lower, stat, side,shoppingStat,shoppinglower;
	public AudioSource button;
	public GameObject popup;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void but()
	{
		upper.SetBool ("downtoup", true);
		lower.SetBool ("lowerpart",true);
		stat.SetBool ("stats", true);
		side.SetBool ("sidebarAnim", true);
		shoppingStat.SetBool ("shoppingstatsreverse",true);
		shoppinglower.SetBool ("shoppinglowerreverse",true);
		upper.SetBool ("uptodown", false);
		lower.SetBool ("loerpartreverse",false);
		stat.SetBool ("statsreverse", false);
		side.SetBool ("sidebarAnimreverse", false);
		shoppingStat.SetBool ("shoppingstats",false);
		shoppinglower.SetBool ("shoppinglower",false);
		button.Play ();
	}
	public void back()
	{
		upper.SetBool ("downtoup", false);
		lower.SetBool ("lowerpart",false);
		stat.SetBool ("stats", false);
		side.SetBool ("sidebarAnim", false);
		shoppingStat.SetBool ("shoppingstatsreverse",false);
		shoppinglower.SetBool ("shoppinglowerreverse",false);
		upper.SetBool ("uptodown", true);
		lower.SetBool ("loerpartreverse",true);
		stat.SetBool ("statsreverse", true);
		side.SetBool ("sidebarAnimreverse", true);
		shoppingStat.SetBool ("shoppingstats",true);
		shoppinglower.SetBool ("shoppinglower",true);
		button.Play ();
	}
	public void pop()
	{
		popup.SetActive (true);
	}
	public void popCancel()
	{
		popup.SetActive (false);
	}

}
