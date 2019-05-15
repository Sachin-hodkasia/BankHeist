using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    SoundController sc;
    public bool equipped = false;

    public enum WeaponType
    {
        turrent, Misciles, Bomb
    }
    public WeaponType weaponType;

    [System.Serializable]
    public class WeaponSettings
    {
        [Header("Bullet Options")]
        public Transform bulletSpwan;
        public float damage = 30f;
        public float bulletSpeed = 5f;
        public float fireRate = 1f;
        public LayerMask bulletLayers;
        public float range = 200f;


        [Header("Effects")]
        public GameObject muzzleFlash;
        public GameObject decal;
        public GameObject hitEffect;

        [Header("Other")]
        public GameObject CrossHair;
        public float reloadDuration = 5f;
    }
    [SerializeField]
    public WeaponSettings weaponSettings;

    [System.Serializable]
    public class Ammunation
    {
        public int carryingAmmo;
        public int maxClipAmmo;
    }
    [SerializeField]
    public Ammunation ammo;

    [System.Serializable]
    public class SoundSettings
    {
        public AudioClip[] gunshotSounds;
        public AudioClip reloadSound;
        [Range(0, 3)] public float pitchMin = 1;
        [Range(0, 3)] public float pitchMax = 1.2f;
        public AudioSource audioS;
    }
    [SerializeField]
    public SoundSettings sounds;


    public bool resetClip = false;

    void Start()
    {
        sc = FindObjectOfType<SoundController>();
    }

    public void Fire(Ray ray)
    {
        if (ammo.carryingAmmo <= 0 || resetClip || !weaponSettings.bulletSpwan)
        {
            return;
        }

        RaycastHit hit;
        Transform bSpwan = weaponSettings.bulletSpwan;
        Vector3 bSpwanPoint = bSpwan.position;
        Vector3 dir = Vector3.zero;
        dir = ray.GetPoint(weaponSettings.range) - bSpwanPoint;
        dir += (Vector3)Random.insideUnitCircle * weaponSettings.bulletSpeed;
        Debug.DrawRay(bSpwanPoint, dir, Color.red);
        if (Physics.Raycast(bSpwanPoint, bSpwan.forward, out hit, weaponSettings.range, weaponSettings.bulletLayers))
        {
            //hit.transform.SendMessage("TakeDamage", weaponSettings.damage, SendMessageOptions.DontRequireReceiver);
            HitEffects(hit);
        }

        GunEffects();
        ammo.carryingAmmo--;
        resetClip = true;
        StartCoroutine(LoadNextBullet());
    }

    IEnumerator LoadNextBullet()
    {
        yield return new WaitForSeconds(weaponSettings.fireRate);
        resetClip = false;
    }

    void HitEffects(RaycastHit hit)
    {
        if (weaponSettings.hitEffect)
        {
            Vector3 hitPoint = hit.point;
            Quaternion LookRotation = Quaternion.LookRotation(hit.normal);
            GameObject blood = Instantiate(weaponSettings.hitEffect, hitPoint, LookRotation) as GameObject;
            Transform bloodT = blood.transform;
            Transform hitT = hit.transform;
            bloodT.SetParent(hitT);
            Destroy(blood, 0.5f);
        }

        if (weaponSettings.decal)
        {
            Vector3 hitPoint = hit.point;
            Quaternion LookRotation = Quaternion.LookRotation(hit.normal);
            GameObject decal = Instantiate(weaponSettings.decal, hitPoint, LookRotation) as GameObject;
            Transform decalT = decal.transform;
            Transform hitT = hit.transform;
            decalT.SetParent(hitT);
            Destroy(decal, 5f);
        }
    }

    void GunEffects()
    {
        #region muzzleFlash
        if (weaponSettings.muzzleFlash)
        {
            Vector3 bulletSpwansPos = weaponSettings.bulletSpwan.position;
            GameObject muzzleFlash = Instantiate(weaponSettings.muzzleFlash, bulletSpwansPos, Quaternion.identity);
            Transform muzzleT = muzzleFlash.transform;
            muzzleT.SetParent(weaponSettings.bulletSpwan);
            Destroy(muzzleFlash, 0.5f);
        }
        #endregion

        PlayGunshotSound();

    }

    void PlayGunshotSound()
    {
        if (sc == null)
        {
            return;
        }

        if (sounds.audioS != null)
        {
            if (sounds.gunshotSounds.Length > 0)
            {
                sc.InstantiateClip(
                    weaponSettings.bulletSpwan.position, // Where we want to play the sound from
                    sounds.gunshotSounds[Random.Range(0, sounds.gunshotSounds.Length)],  // What audio clip we will use for this sound
                    1f, // How long before we destroy the audio
                    true, // Do we want to randomize the sound?
                    sounds.pitchMin, // The minimum pitch that the sound will use.
                    sounds.pitchMax); // The maximum pitch that the sound will use.
            }
        }
    }

    public void LoadClip()
    {
        ammo.carryingAmmo = ammo.maxClipAmmo;
    }

    public void SetEquipped(bool equip)
    {
        equipped = equip;
    }
}
