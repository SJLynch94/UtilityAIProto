using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AILogic : MonoBehaviour
{

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
    public UtilityAIProto.UAI_PropertyBool bHasEnemy, bHasHealth, bHasAmmo, bZoneMoving, bCanSeeEnemy, bIsEnemyInDist, bIsEnemyInAttackDist;

    NavMeshAgent mNavAgent;
    [SerializeField]
    public float mMaxDistance;
    [SerializeField]
    public float mAttackDist;
    [SerializeField]
    public float mRotateSpeed = 3.0f;
    [SerializeField]
    public float mHeightMul = 1.5f;
    [SerializeField]
    public float mFOVHalf = 30.0f;

    SphereCollider mSphereCollider;

    GunLogic mGunLogic;
    [SerializeField]
    RW_Trace mRWTrace;
    [SerializeField]
    Animator mAnim;
    [SerializeField]
    int mWhatAmIDoing = 0;

    // Use this for initialization
    void Start()
    {
        mAgent.SetActionDelegate("GetHealth", GetHealth);
        mAgent.SetActionDelegate("GetAmmo", GetAmmo);
        //mAgent.SetActionDelegate("MoveToArea", MoveToArea);
        mAgent.SetActionDelegate("AttackEnemy", AttackEnemy);
        mAgent.SetActionDelegate("FindEnemy", FindEnemy);

        //foreach (var e in FindObjectsOfType<GameObject>())
        //{
        //    if (e)
        //    {
        //        if (e == this) { continue; }
        //        if (e.GetComponent<AILogic>())
        //        {
        //            float mag = (e.transform.position - mRigidBody.transform.position).magnitude;
        //            mEnemies.Add(mag, e.gameObject);
        //        }
        //        if (e.GetComponent<MedBox>())
        //        {
        //            float mag = (e.transform.position - mRigidBody.transform.position).magnitude;
        //            mMedBoxes.Add(mag, e.GetComponent<MedBox>());
        //        }
        //        if (e.GetComponent<AmmoBox>())
        //        {
        //            float mag = (e.transform.position - mRigidBody.transform.position).magnitude;
        //            mAmmoBoxes.Add(mag, e.GetComponent<AmmoBox>());
        //        }
        //    }

        //}
    }

    void Awake()
    {
        mRigidBody = GetComponent<Rigidbody>();
        mSphereCollider = GetComponent<SphereCollider>();
        mGunLogic = GetComponent<GunLogic>();
        mRWTrace = GetComponentInChildren<RW_Trace>();
        mNavAgent = GetComponent<NavMeshAgent>();
        mAgent = GetComponent<UtilityAIProto.UAI_Agent>();
        mAnim = GetComponent<Animator>();
        mPreDestination = transform.position;
        mAnim.SetInteger("WhatAmIDoing", mWhatAmIDoing);

        foreach(var e in FindObjectsOfType<AILogic>())
        {
            if (e)
            {
                if(e == this) { continue; }
                float mag = (e.transform.position - mRigidBody.transform.position).magnitude;
                mEnemies.Add(mag, e.gameObject);
            }
        }

        foreach (var e in FindObjectsOfType<MedBox>())
        {
            if (e)
            {
                float mag = (e.transform.position - mRigidBody.transform.position).magnitude;
                mMedBoxes.Add(mag, e.GetComponent<MedBox>());
            }
        }

        foreach (var e in FindObjectsOfType<AmmoBox>())
        {
            if (e)
            {
                float mag = (e.transform.position - mRigidBody.transform.position).magnitude;
                mAmmoBoxes.Add(mag, e.GetComponent<AmmoBox>());
            }
        }

    }

    private void FindEnemy()
    {
        ResetUAI();
        if (!mCurrentTarget)
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
            for (var i = 0; i < mEnemies.Count; ++i)
            {
                if (mEnemies.Values[i].gameObject)
                {
                    if (mEnemies.Values[i].GetComponent<AILogic>())
                    {
                        float currentMag = (mEnemies.Values[i].transform.position - transform.position).magnitude;
                        GameObject AI = mEnemies.Values[i].gameObject;
                        if (Vector3.Distance(mEnemies.Values[i].transform.position, transform.position) <= mMaxDistance)
                        {
                            bIsEnemyInDist.Value = true;
                            if (currentMag < mEnemies.Keys[i])
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
        if (mCurrentTarget)
        {
            if (Vector3.Distance(mCurrentTarget.transform.position, transform.position) <= mAttackDist)
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
        if (mNavAgent.hasPath)
        {
            if (mNavAgent.isPathStale)
            {
                if (mCurrentTarget)
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
            if (mCurrentTarget)
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

        if (mAmmoBoxes.Count <= 0)
        {
            //mAmmo -= 3.0f * UtilityAIProto.UAI_Time.MyTime;
            mAgent;
            return;
        }

        for (var i = 0; i < mAmmoBoxes.Count; ++i)
        {
            if (mAmmoBoxes.Values[i].gameObject && mAmmoBoxes.Values[i].GetComponent<AmmoBox>())
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

        if (mMedBoxes.Count <= 0)
        {
            mAmmo -= 3.0f * UtilityAIProto.UAI_Time.MyTime;
        }

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
    void Update()
    {
        mAgent.UpdateUAI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other) { return; }

        if (other.GetComponent<AILogic>())
        {
            var rott = Quaternion.LookRotation(other.transform.position - mRigidBody.transform.position);
            mRigidBody.MoveRotation(Quaternion.Slerp(mRigidBody.transform.rotation, rott, t: Time.fixedDeltaTime * mRotateSpeed));
            float mag = (mRigidBody.transform.position - other.transform.position).magnitude;
            Ray ray = new Ray
            {
                origin = mRigidBody.transform.position,
                direction = (other.transform.position - mRigidBody.transform.position).normalized
            };

            RaycastHit hit = new RaycastHit();
            if ((Vector3.Angle(ray.direction, mRigidBody.transform.forward)) < mFOVHalf * 2)
            {
                if (Physics.Raycast(ray.origin + Vector3.up * mHeightMul, ray.direction, out hit, mSphereCollider.radius))
                {
                    if (hit.collider.transform.root.gameObject.GetComponent<AILogic>())
                    {
                        Debug.Log("Target: " + other.name + " is close to the " + transform.name + " and is in front of " + transform.name);
                    }
                }
            }
        }
    }

    void ResetUAI()
    {
        if (mAgent.NewAction)
        {
            bAtDestination = false;
            mAgent.NewAction = false;
        }
    }

    void DetermineWhatToDo()
    {
        if (mAgent.TopAction.handle == GetHealth)
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
        if (mAgent.TopAction.handle == FindEnemy)
        {

        }
    }
}
