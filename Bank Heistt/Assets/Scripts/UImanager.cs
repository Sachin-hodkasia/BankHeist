using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UImanager : MonoBehaviour {
    public static UImanager Instance { get; set; }
    // Use this for initialization
    int currentAchievmentTab = 0;
    public Animator UpperButton;
    public Animator[] LowerButtons;
    public Animator PlayerProfile;
    public GameObject ConfirmBuy;
    public string BinaryAchievment="00010";
    public GameObject achievmentPanel , achievmentPanelTabsHolder;
    void Start () {
        Instance = this;	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void BringUpperButtons()
    {
        UpperButton.SetTrigger("Open");
    }

    public void BringLowerButtons()
    {
        foreach(Animator anim in LowerButtons)
        {
            anim.SetTrigger("Open");
        }
    }

    public void BringPlayerProfile()
    {
        PlayerProfile.SetTrigger("Open");
    }
 

    public void ExitUpperButtons()
    {
        UpperButton.SetTrigger("Close");
    }

    public void ExitLowerButtons()
    {
        foreach (Animator anim in LowerButtons)
        {
            anim.SetTrigger("Close");
        }
    }

    public void ExitPlayerProfile()
    {
        PlayerProfile.SetTrigger("Close");
    }
    //-----------------------------------------------------------------------------CONFIRM BUY-----------------------------------------------------------------------
    public void ConfirmBuyEntry(int money)
    {
        ConfirmBuy.GetComponent<ConfirmBuyPanel>().SetHeistCoins(money);
        ConfirmBuy.GetComponent<Animator>().SetTrigger("Open");
    }
    public void ConfirmBuyEntryButCash(int cash)
    {
        ConfirmBuy.GetComponent<ConfirmBuyPanel>().SetHeistCash(cash);
        ConfirmBuy.GetComponent<Animator>().SetTrigger("Open");
    }
    public void ConfirmBuyExit(bool purchaseMade)
    {
        ConfirmBuy.GetComponent<Animator>().SetTrigger("Close");
    }
    //-----------------------------------------------------------------------------CONFIRM BUY-----------------------------------------------------------------------
    //-----------------------------------------------------------------------------ACHIEVMENT PANEL------------------------------------------------------------------
    //                                            to tell which tab has new info , then change is variable BinaryAchievment to desired number , 1= new , 0 = nothing new
    public void AchievmentPanelOpen()
    {
        BinaryAchievment = "11000";
        //  *********************************   get this from player prefs or something else
    }
    public void AchievmentPanelOpenTab(int tabNumber)
    {
        currentAchievmentTab = tabNumber;
        achievmentPanel.GetComponent<Animator>().SetTrigger("TransitionIn");
        achievmentPanelTabsHolder.GetComponent<Animator>().SetTrigger("Entry");
        achievmentPanelTabsHolder.transform.GetChild(tabNumber).gameObject.SetActive(true);
        
    }
    public void AchievmentPanelExitCurrentTab()
    {
        achievmentPanelTabsHolder.GetComponent<Animator>().SetTrigger("Exit");
        achievmentPanelTabsHolder.transform.GetChild(currentAchievmentTab).gameObject.GetComponent<Animator>().SetTrigger("Exit");
        achievmentPanel.GetComponent<Animator>().SetTrigger("TransitionOut");
    }
    //-----------------------------------------------------------------------------ACHIEVMENT PANEL end------------------------------------------------------------------

    //-----------------------------------------------------------------------------Efficiency------------------------------------------------------------------

    public void ExitEntireMainMenu()
    {
        ExitUpperButtons();
        ExitLowerButtons();
        ExitPlayerProfile();
    }

    public void BringEntireMenu()
    {
        BringUpperButtons();
        BringLowerButtons();
        BringPlayerProfile();
    }

    //-----------------------------------------------------------------------------Efficiency end------------------------------------------------------------------
}
