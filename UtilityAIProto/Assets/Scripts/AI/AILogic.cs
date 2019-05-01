using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AILogic : MonoBehaviour {

    protected Rigidbody mRigidBody;
    protected Camera mPlayerCamera;
    [SerializeField] public GameObject mWeaponHolder;

    SortedList<float, GameObject> mEnemies = new SortedList<float, GameObject>();
    SortedList<float, AmmoBox> mAmmoBoxes = new SortedList<float, AmmoBox>();
    SortedList<float, MedBox> mMedBoxes = new SortedList<float, MedBox>();

    public GameObject mCurrentTarget;

    public float mMovementSpeed;
    public Vector3 mDestination, mPreDestination;
    public bool bAtDestination;

    UtilityAIProto.UAI_Agent mAgent;
    public UtilityAIProto.UAI_PropertyFloat mKill, mZone, mHealth, mAmmo, mAttackEnemy;
    public UtilityAIProto.UAI_PropertyBool bHasEnemy, bHasHealth, bHasAmmo, bZoneMoving, bCanSeeEnemy, bIsEnemyInDist;

    NavMeshAgent mNavAgent;
    public float mMaxDistance;
    public float mAttackDist;

    GunLogic mGunLogic;
    Animator mAnim;
    int mWhatAmIDoing = 0;

    // Use this for initialization
    void Start () {
        mAgent.SetActionDelegate("GetHealth", GetHealth);
        mAgent.SetActionDelegate("GetAmmo", GetAmmo);
        //mAgent.SetActionDelegate("MoveToArea", MoveToArea);
        mAgent.SetActionDelegate("AttackEnemy", AttackEnemy);
        mAgent.SetActionDelegate("FindEnemy", FindEnemy);
	}

    void Awake()
    {
        mGunLogic = GetComponent<GunLogic>();
        mNavAgent = GetComponent<NavMeshAgent>();
        mAgent = GetComponent<UtilityAIProto.UAI_Agent>();
        mAnim = GetComponent<Animator>();
        mPreDestination = transform.position;
        mAnim.SetInteger("WhatAmIDoing", mWhatAmIDoing);

        foreach (var i in FindObjectsOfType<AILogic>())
        {
            if (i == this)
            {
                continue;
            }
            else
            {
                if (i.GetComponent<AILogic>())
                {
                    float mag = (i.transform.position - transform.position).magnitude;
                    mEnemies.Add(mag, i.gameObject);
                }
            }
        }

        foreach(var i in FindObjectsOfType<MedBox>())
        {
            float mag = (i.transform.position - transform.position).magnitude;
            mMedBoxes.Add(mag, i.GetComponent<MedBox>());
        }

        foreach (var i in FindObjectsOfType<AmmoBox>())
        {
            float mag = (i.transform.position - transform.position).magnitude;
            mAmmoBoxes.Add(mag, i.GetComponent<AmmoBox>());
        }
    }

    private void FindEnemy()
    {
        ResetUAI();
        if(!mCurrentTarget)
        {
            //foreach(var e in mEnemies)
            //{
            //    if(e)
            //    {
            //        if (Vector3.Distance(e.transform.position, transform.position) <= mMaxDistance)
            //        {
            //            mCurrentTarget = e;
            //            bHasEnemy.Value = true;
            //            mDestination = mCurrentTarget.transform.position;
            //        }
            //    }
            //}
            for(var i = 0; i < mEnemies.Count; ++i)
            {
                if(mEnemies.Values[i].gameObject)
                {
                    if (mEnemies.Values[i].GetComponent<AILogic>())
                    {
                        float currentMag = (mEnemies.Values[i].transform.position - transform.position).magnitude;
                        GameObject AI = mEnemies.Values[i].gameObject;
                        if(Vector3.Distance(mEnemies.Values[i].transform.position, transform.position) <= mMaxDistance)
                        {
                            if(currentMag < mEnemies.Keys[i])
                            {
                                mEnemies.Remove(mEnemies.Keys[i]);
                                mEnemies.Add(currentMag, AI);
                            }
                        }
                    }
                }
            }
            mCurrentTarget = mEnemies.Values[0];
            bHasEnemy.Value = true;
            mDestination = mCurrentTarget.transform.position;
        }
    }

    private void AttackEnemy()
    {
        ResetUAI();
        if(mCurrentTarget)
        {
            if(Vector3.Distance(mCurrentTarget.transform.position, transform.position) <= mAttackDist)
            {
                bIsEnemyInDist.Value = true;
                // Need to do a raycast to target to then set bCanSeeEnemy to true

                mWhatAmIDoing = 2;
                mAnim.SetInteger("WhatAmIDoing", mWhatAmIDoing);
                mAttackEnemy.Value += 10.0f * UtilityAIProto.UAI_Time.MyTime;
                mAmmo.Value += 5.0f * UtilityAIProto.UAI_Time.MyTime;
            }
        }
    }

    private void MoveToArea()
    {
        //float step = mMovementSpeed * UtilityAILynch.UAI_Time.MyTime;
        if(mNavAgent.hasPath)
        {
            if(mNavAgent.isPathStale)
            {
                if(mCurrentTarget)
                {
                    mPreDestination = transform.position;
                    mWhatAmIDoing = 1;
                    mAnim.SetInteger("WhatAmIDoing", mWhatAmIDoing);
                    mNavAgent.SetDestination(mCurrentTarget.transform.position);
                    mDestination = mCurrentTarget.transform.position;
                }
                DetermineWhatToDo();
            }
            Vector3 x = (mCurrentTarget.transform.position - transform.position).normalized;
            mNavAgent.Move(x * UtilityAIProto.UAI_Time.MyTime);
        }
        else
        {
            if(mCurrentTarget)
            {
                mNavAgent.SetDestination(mCurrentTarget.transform.position);
                Vector3 x = (mCurrentTarget.transform.position - transform.position).normalized;
                mNavAgent.Move(x * UtilityAIProto.UAI_Time.MyTime);
            }
            else
            {

            }
        }
    }

    private void GetAmmo()
    {
        ResetUAI();

        for(var i = 0; i < mAmmoBoxes.Count; ++i)
        {
            if(mAmmoBoxes.Values[i].gameObject && mAmmoBoxes.Values[i].GetComponent<AmmoBox>())
            {
                float currentMag = (mAmmoBoxes.Values[i].transform.position - transform.position).magnitude;
                AmmoBox ammobox = mAmmoBoxes.Values[i];
                if (Vector3.Distance(mAmmoBoxes.Values[i].transform.position, transform.position) <= mMaxDistance)
                {
                    if (currentMag < mAmmoBoxes.Keys[i])
                    {
                        mAmmoBoxes.Remove(mAmmoBoxes.Keys[i]);
                        mAmmoBoxes.Add(currentMag, ammobox);
                    }
                }
            }
        }
        mAmmoBoxes.TrimExcess();
        mNavAgent.SetDestination(mAmmoBoxes.Values[0].transform.position);
        mAmmo.Value += 5.0f * UtilityAIProto.UAI_Time.MyTime;
    }

    private void GetHealth()
    {
        ResetUAI();

        for (var i = 0; i < mMedBoxes.Count; ++i)
        {
            if (mMedBoxes.Values[i].gameObject && mMedBoxes.Values[i].GetComponent<MedBox>())
            {
                float currentMag = (mMedBoxes.Values[i].transform.position - transform.position).magnitude;
                MedBox medbox = mMedBoxes.Values[i];
                if (Vector3.Distance(mMedBoxes.Values[i].transform.position, transform.position) <= mMaxDistance)
                {
                    if (currentMag < mAmmoBoxes.Keys[i])
                    {
                        mMedBoxes.Remove(mMedBoxes.Keys[i]);
                        mMedBoxes.Add(currentMag, medbox);
                    }
                }
            }
        }
        mMedBoxes.TrimExcess();
        mNavAgent.SetDestination(mMedBoxes.Values[0].transform.position);
        mHealth.Value += 5.0f * UtilityAIProto.UAI_Time.MyTime;
    }

    // Update is called once per frame
    void Update () {
        mAgent.UpdateUAI();
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<AILogic>())
        {

        }
    }

    void ResetUAI()
    {
        if(mAgent.NewAction)
        {
            bAtDestination = false;
            mAgent.NewAction = false;
        }
    }

    void DetermineWhatToDo()
    {
        if(mAgent.TopAction.handle == GetHealth)
        {
            
        }
        if (mAgent.TopAction.handle == GetAmmo)
        {

        }
        if (mAgent.TopAction.handle == MoveToArea)
        {

        }
        if (mAgent.TopAction.handle == AttackEnemy)
        {

        }
        if(mAgent.TopAction.handle == FindEnemy)
        {

        }
    }
}
