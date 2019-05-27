using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangedWeapon : Weapons
{
    [SerializeField] protected float mFireRate = 10.0f; // Amount of rounds can be fired per second (RPS)
    [SerializeField] protected float mRange = 100.0f; // Range per meters bullets last for. Set to 0 for bullet drop
    [SerializeField] protected float mReloadTime = 0.0f;
    [SerializeField] protected float mMaxReloadTime = 1.0f;
    [SerializeField] protected GameObject mMuzzleLocation;
    [SerializeField] protected GameObject mMuzzleFlash;
    [SerializeField] protected GameObject mBulletImpact;
    [SerializeField] protected GameObject mBulletImpactImpact;

    [SerializeField] protected List<AudioClip> mWeaponShootClips = new List<AudioClip>(); // Clips(s) played on shoot
    [SerializeField] protected AudioClip mWeaponStartReloadClip; // Clip played when reload is started
    [SerializeField] protected AudioClip mWeaponEndReloadClip; // Clip played when reload ends
    [SerializeField] protected AudioClip mWeaponRechamberClip; // Clip played between shots -- e.g. shotgun
 
    protected float mFireRateTimer = 0.0f; // Time until weapon can be fired again

    public float Range
    {
        get { return mRange; }
    }

    private void FixedUpdate()
    {
        if (mFireRateTimer > 0.0f) mFireRateTimer -= UtilityAIProto.UAI_Time.MyTime;
        if (mWeaponState == EWeaponState.Reloading)
        {
            //GetComponentInParent<Animator>().SetInteger("WhatAmIDoing", (int)AILogic.EAnimatorValue.Reloading);
            if ((mReloadTime -= UtilityAIProto.UAI_Time.MyTime) <= 0.0f)
            {
                mWeaponState = EWeaponState.Idle;
                FinishReload();
            }
        }

        if (mAmmo >= (mMaxAmmo / 3) * 2 && mAmmo <= mMaxAmmo)
        {
            AmmoState = EAmmoState.High;
        }
        if (mAmmo <= (mMaxAmmo / 3) * 2 - 1 && mAmmo >= (mMaxAmmo / 3))
        {
            AmmoState = EAmmoState.Medium;
        }
        if (mAmmo <= (mMaxAmmo / 3) - 1 && mAmmo >= 0)
        {
            AmmoState = EAmmoState.Low;
        }
    }

    public override void OnShoot()
    {
        if (mWeaponState == EWeaponState.Idle || mWeaponState == EWeaponState.Firing && mWeaponFireType == EWeaponFireType.Auto)
        {
            if (mFireRateTimer <= 0.0f && mMagAmmo > 0)
            {
                mFireRateTimer = (1.0f) / (mFireRate);
                mMagAmmo--;
                mWeaponState = EWeaponState.Firing;
                GetComponentInParent<Animator>().SetInteger("WhatAmIDoing", 2);
                ShootWeapon();
                mMuzzleFlash.gameObject.SetActive(true);
                if (mMagAmmo <= 0)
                {
                    OnReload();
                    mMuzzleFlash.gameObject.SetActive(false);
                }

                if (mWeaponShootClips.Count > 0)
                {
                    mWeaponAudioSource.clip = mWeaponShootClips[Random.Range(0, mWeaponShootClips.Count)];
                    mWeaponAudioSource.Play();
                }
            }
        }
    } 

    public override void OnStopShoot()
    {
        mMuzzleFlash.gameObject.SetActive(false);
    }

    public abstract void ShootWeapon();

    public override void OnReload()
    {
        if (mMagAmmo < mMaxMagAmmo)
        {   
            mWeaponState = EWeaponState.Reloading;
            mReloadTime = mMaxReloadTime;
        }
    }

    void FinishReload()
    {
        int neededAmmo = mMaxMagAmmo - mMagAmmo;
        if (neededAmmo <= mAmmo)
        {
            mMagAmmo += neededAmmo;
            mAmmo -= neededAmmo;
            GetComponentInParent<AILogic>().mAmmo.Value += neededAmmo * UtilityAIProto.UAI_Time.MyTime;
        }
        else
        {
            mMagAmmo += mAmmo;
            mAmmo = 0;
            GetComponentInParent<AILogic>().mAmmo.Value += mMaxMagAmmo * UtilityAIProto.UAI_Time.MyTime;
            GetComponentInParent<AILogic>().bHasAmmo.Value = false;
        }
    }

    public override void OnStopShooting()
    {
        if (mWeaponState != EWeaponState.Reloading)
        {
            mWeaponState = EWeaponState.Idle;
        }
    }
}

