﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class Weapons : MonoBehaviour
{
    public enum EWeaponState { Idle, Firing, Reloading };
    public enum EWeaponFireType { SemiAuto, Burst, Auto };

    protected AudioSource mWeaponAudioSource; // Weapons audio source

    public EWeaponFireType mWeaponFireType;
    public EWeaponState mWeaponState;

    public string mWeaponName = "";
    public string mWeaponDescription = "";

    public Vector3 mWeaponLocation; // Location of weapon on weapon socket
    public Vector3 mWeaponRotation; // Rotation of weapon in weapon socket


    public int mWeaponDamage = 0; // Damage per bullet, Base Grenade damage etc...
    public int mAmmo = 0; // Current ammo for weapon being carried 
    public int mMaxAmmo = 0; // Max amount of ammo that can be carried for current weapon
    public System.Int32 mMaxMagAmmo; // Max ammo a mag can have
    public System.Int32 mMagAmmo; // current ammo in mag

    public abstract void OnShoot();
    public abstract void OnStopShooting();
    public abstract void OnReload();
    public abstract void OnStopShoot();




    // Attempt to add passed ammo and return true if ammo has been added
    public bool AddAmmo(int ammo)
    {
        if (mAmmo < mMaxAmmo)
        {
            mAmmo += ammo;
            if (mAmmo > mMaxAmmo) mAmmo = mMaxAmmo;
            return true;
        }
        return false;
    }

    public int GetMaxAmmo() { return mMaxAmmo; }

    private void Start()
    {
        mWeaponAudioSource = GetComponent<AudioSource>();
    }
}