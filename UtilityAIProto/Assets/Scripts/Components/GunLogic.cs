using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunLogic : MonoBehaviour
{
    // The Bullet Spawn Point
    [SerializeField]
    Transform mWeaponSocket;

    // The Bullet Spawn Point
    [SerializeField]
    float mShotCooldown = 0.5f;

    bool bCanShoot = true;

    // VFX
    [SerializeField]
    ParticleSystem mFlare;

    [SerializeField]
    ParticleSystem mSmoke;

    [SerializeField]
    ParticleSystem mSparks;

    // SFX
    [SerializeField]
    AudioClip mBulletSound;

    // The AudioSource to play Sounds for this object
    AudioSource mAudioSource;

    [SerializeField]
    public int mMaxAmmo = 120;

    [SerializeField]
    public int mAmmo;

    [SerializeField]
    public int mBulletLength;

    [SerializeField]
    public int mDamage = 12;



    // Use this for initialization
    void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
        mAmmo = mMaxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (!bCanShoot)
        {
            mShotCooldown -= Time.deltaTime;
            if (mShotCooldown < 0.0f)
            {
                bCanShoot = true;
            }
        }

        if (bCanShoot)
        {
            Fire();
            bCanShoot = false;
        }
    }

    void Fire()
    {
        // Reduce the Ammo count
        --mAmmo;

        // Play Particle Effects
        PlayFireFX();
        RaycastHit hit;
        if(Physics.Raycast(mWeaponSocket.position, mWeaponSocket.forward, out hit, mBulletLength))
        {
            if(hit.collider.tag == "AI")
            {
                if(hit.collider.GetComponent<AILogic>())
                {
                    if(hit.collider.GetComponent<HealthComponent>())
                    {
                        hit.collider.GetComponent<HealthComponent>().Damage(mDamage);
                    }
                }
            }
        }

        // Play Sound effect
        if (mAudioSource && mBulletSound)
        {
            mAudioSource.PlayOneShot(mBulletSound);
        }
    }

    void PlayFireFX()
    {
        if (mFlare)
        {
            mFlare.Play();
        }

        if (mSparks)
        {
            mSparks.Play();
        }

        if (mSmoke)
        {
            mSmoke.Play();
        }
    }

    public void AddAmmo(int bullets)
    {
        mAmmo += bullets;
    }
}
