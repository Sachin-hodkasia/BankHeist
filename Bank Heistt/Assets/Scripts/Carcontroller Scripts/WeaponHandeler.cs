using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandeler : MonoBehaviour {
    SoundController sc;
    public Weapons currentWeapon;
    public List<Weapons> weaponList = new List<Weapons>();
    public int maxWeapons = 2;
    public bool reload;
    public int weapontype;
    bool settingWeapon;

    private void Start()
    {
        sc = FindObjectOfType<SoundController>();
        SetUpWeapons();
    }

	void SetUpWeapons()
    {
        if (currentWeapon)
        {

            currentWeapon.SetEquipped(true);
            AddWeapontoList(currentWeapon);
            if (currentWeapon.ammo.carryingAmmo <= 0)
                Reload();
            if (reload)
                if (settingWeapon)
                    reload = false;
        }
        if (weaponList.Count > 0)
        {
            for (int i = 0; i < weaponList.Count; i++)
            {
                if (weaponList[i] != currentWeapon)
                {
                    weaponList[i].SetEquipped(false);
                }
            }
        }
    }
	
	void Update () {

        SetUpWeapons();
	}

    void AddWeapontoList(Weapons weapon)
    {
        if (weaponList.Contains(weapon))
            return;
        weaponList.Add(weapon);

    }


    public void Reload()
    {
        if (reload || !currentWeapon)
            return;
        if (currentWeapon.ammo.carryingAmmo <= 0)
            return;

        if (sc != null)
        {
            if (currentWeapon.sounds.reloadSound != null)
            {
                if (currentWeapon.sounds.audioS != null)
                {
                    sc.PlaySound(currentWeapon.sounds.audioS, currentWeapon.sounds.reloadSound, true, currentWeapon.sounds.pitchMin, currentWeapon.sounds.pitchMax);
                }
            }
        }
        reload = true;
        StartCoroutine(StopReload());
    }

    IEnumerator StopReload()
    {
        yield return new WaitForSeconds(currentWeapon.weaponSettings.reloadDuration);
        currentWeapon.LoadClip();
        reload = false;
    }


    public void switchWeapons()
    {
        if (settingWeapon||weaponList.Count==0)
            return;
        if (currentWeapon)
        {
            int currentWeaponIndex = weaponList.IndexOf(currentWeapon);
            int nextWeaonIndex = (currentWeaponIndex + 1) % weaponList.Count;
            currentWeapon = weaponList[nextWeaonIndex];
        }
        else 
        {
            currentWeapon = weaponList[0];
        }
        settingWeapon = true;
        StartCoroutine(StopSettinWeapon());
    }


    IEnumerator StopSettinWeapon()
    {
        yield return new WaitForSeconds(0.7f);
        settingWeapon = false;
    }


    public void UnequipCurrentWeapon()
    {
        if (!currentWeapon)
            return;
        currentWeapon.SetEquipped(false);
        currentWeapon = null;
    }

}
