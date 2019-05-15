using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LeaderboardPanel : MonoBehaviour {

    public Sprite[] selected;
    public Sprite[] Nonselected;
    public Transform FriendOrGlobalHolder, ElementsTabHolder;
    // Use this for initialization
    void Start()
    {
        CHANGETOCURRENTTAB(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CHANGETOCURRENTTAB(int childIndex)
    {
        for (int i = 0; i < FriendOrGlobalHolder.childCount; i++)
        {
            if (i == childIndex)
            {
                FriendOrGlobalHolder.GetChild(childIndex).GetComponent<Image>().sprite = selected[childIndex];
                ElementsTabHolder.GetChild(childIndex).gameObject.SetActive(true);
            }
            else
            {
                FriendOrGlobalHolder.GetChild(i).GetComponent<Image>().sprite = Nonselected[i];
                ElementsTabHolder.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    
}
