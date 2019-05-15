using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
public class GiftPanelControls : MonoBehaviour {

    public FixedJoystick leftJoystick, rightJoystick;
    public GameObject chestUp,chestParent,ActualGiftPanel;
    bool opened = false, interactable = false,vibrating=false;
    float maxlimit = 1.98f,difference=0;
    Vector3 initalSize,defaultPos;
    public ParticleSystem chestInside, sparks , BGPS , lastFlash;
    public Color[] colorsList;
    int giftRarity = 0;
	// Use this for initialization
	void Start () {
        chestParent.SetActive(true);
        leftJoystick.gameObject.SetActive(false);
        rightJoystick.gameObject.SetActive(false);
        initalSize = chestParent.transform.localScale;
        StartCoroutine(setInteractable());
        leftJoystick.gameObject.SetActive(true);
        rightJoystick.gameObject.SetActive(true);

        SetGiftColor(0);
        Camera.main.GetComponent<PostProcessingBehaviour>().enabled = true;
        print("post processing ko band krna mat bhulna  , and actual gift pe shine daalna na bhulna , yeh code se chest ko set active true kr rha hai");


	}
	
	// Update is called once per frame
	void LateUpdate () {

        if (leftJoystick.Horizontal <= 0 && rightJoystick.Horizontal >= 0)
        {
            difference = Mathf.Abs(leftJoystick.Horizontal) + Mathf.Abs(rightJoystick.Horizontal);
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            difference = 2f;
        }
        if (interactable == true)
        {

            //last condition
            if (difference >= 1.8f)
            {
                opened = true;
                lastFlash.Play();
                ShowActualGift();
                //Show Actual Gift
            }
            if (opened == false)
            {
                #region vibration
                if (difference > 0.2f && difference < 1f)
                {
                    if (vibrating == false)
                    {
                        StartCoroutine(vibrate(0.2f));
                    }
                }

                if (difference > 1f && difference < 1.8f)
                {
                    if (vibrating == false)
                    {
                        StartCoroutine(vibrate(1f));
                    }
                }
                #endregion

                Vector3 finalRot=  new Vector3(((difference) / 1.8f) * 45f, 0, 0);
                chestUp.transform.localEulerAngles = Vector3.Lerp(chestUp.transform.localEulerAngles, finalRot, Time.deltaTime * 5);
                Vector3 finalSize=initalSize * (1 + difference / 2);
                chestParent.transform.localScale = Vector3.Lerp(chestParent.transform.localScale,finalSize, Time.deltaTime*5);

               //------------------------------------------SHAKING----------------------------------
               Vector3 Randomoffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * difference*difference*2;

                //if (Vector3.Magnitude(chestParent.transform.position + Randomoffset - defaultPos) < 7)
                {
                    Vector3 newPos = chestParent.transform.position + Randomoffset;

                    chestParent.transform.position = newPos;
                }
                //-------------------------------------------Shaking----------------------------------
                //------------------------------------------CHestINside-------------------------------
                var EMISS = chestInside.emission;
                EMISS.rateOverTime = 50 * (1 + difference / 1.7f);
                var MAIN = chestInside.main;
                MAIN.startSize = 30 * (1 + difference / 0.7f);
                //------------------------------------------CHestINside-------------------------------
                //------------------------------------------Sparks-------------------------------

                var sparksEmiss = sparks.emission;
                sparksEmiss.rateOverTime = 37 * (1 + difference / 0.82f);
                var sparksMain = sparks.main;
                sparksMain.startSizeY = difference /2.5f;
                sparksMain.startSpeed = 80 * (1 + difference / 0.5f);

                //------------------------------------------Sparks-------------------------------
                //------------------------------------------BGPS-------------------------------
                var BGNoise = BGPS.noise;
                BGNoise.frequency = 0.01f*(1+difference/0.375f);
                BGNoise.strength = 15 * (1 + difference / 0.64f);
                var BGemission = BGPS.emission;
                BGemission.rateOverTime = difference * 100;





                //------------------------------------------BGPS-------------------------------




            }
        }

      
        
    }
    public void SetGiftColor(int Rarity=0)
    {
        var main = chestInside.main;
        main.startColor = colorsList[Rarity];
        var sparksMain = sparks.main;
        sparksMain.startColor = colorsList[Rarity];
    }
    IEnumerator setInteractable()
    {
        yield return new WaitForSeconds(0.6f);
        leftJoystick.gameObject.SetActive(true);
        rightJoystick.gameObject.SetActive(true);
        defaultPos = chestParent.transform.position;
        defaultPos.z = -141;
        //chestParent.GetComponent<Animator>().enabled = false;
        interactable = true;
    }
    IEnumerator vibrate(float intensity)
    {
        float timeOfIntensity=intensity*1000;
        Vibration.Vibrate((long)timeOfIntensity);
        vibrating = true;
        yield return new WaitForSeconds(timeOfIntensity/1000);
        vibrating = false;

    }

    float GetX(float min , float max , float maxValueOfDifference)
    {
        float x = maxValueOfDifference / (max / min - 1);
        return x;
    }

    void ShowActualGift()
    {
        ActualGiftPanel.SetActive(true);
        chestParent.SetActive(false);
        leftJoystick.gameObject.SetActive(false);
        rightJoystick.gameObject.SetActive(false);
        BGPS.gameObject.SetActive(false);
        chestParent.SetActive(false);
    }
}

