using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AILogic : MonoBehaviour
{

    protected Rigidbody mRigidBody;
    protected Camera mCamera;
    Transform mCameraPos;
    [SerializeField] public GameObject mWeaponHolder;

    public SortedList<float, GameObject> mEnemies = new SortedList<float, GameObject>();
    public SortedList<float, AmmoBox> mAmmoBoxes = new SortedList<float, AmmoBox>();
    public SortedList<float, MedBox> mMedBoxes = new SortedList<float, MedBox>();

    public GameObject mCurrentTarget;

    public float mMovementSpeed;
    public Vector3 mDestination, mPreDestination;
    public bool bAtDestination, bMoving;

    UtilityAIProto.UAI_Agent mAgent;
    public UtilityAIProto.UAI_PropertyFloat mKill, mZone, mHealth, mAmmo, mAttackEnemy;
    public UtilityAIProto.UAI_PropertyBool bHasEnemy, bHasHealth, bHasAmmo, bZoneMoving, bCanSeeEnemy, bIsEnemyInDist, bIsEnemyInAttackDist;

    NavMeshAgent mNavAgent;
    [SerializeField]
    public float mMaxDistance;
    [SerializeField]
    public float mAttackDist;
    [SerializeField]
    float mDist;
    [SerializeField]
    public float mRotateSpeed = 3.0f;
    [SerializeField]
    public float mHeightMul = 1.5f;
    [SerializeField]
    public float mFOVHalf = 30.0f;

    SphereCollider mSphereCollider;

    HealthComponent mHealthComp;

    GunLogic mGunLogic;
    [SerializeField]
    RW_Trace mRWTrace;
    [SerializeField]
    Animator mAnim;
    [SerializeField]
    public int mWhatAmIDoing = 0;

    public enum EAnimatorValue
    {
        Idle, Moving, Firing, Reloading, MovingReload,
    };

    public Transform CameraTransform
    {
        get { return mCameraPos; }
    }

    // Use this for initialization
    void Start()
    {
        mAgent.SetActionDelegate("GetHealth", GetHealth);
        mAgent.SetActionDelegate("GetAmmo", GetAmmo);
        mAgent.SetActionDelegate("AttackEnemy", AttackEnemy);
        mAgent.SetActionDelegate("FindEnemy", FindEnemy);

        mMaxDistance = mRWTrace.Range;
        mAttackDist = mMaxDistance / 3.0f;
    }

    void Awake()
    {
        //mCameraPos = GameObject.FindGameObjectWithTag("CameraPos").transform;
        //mCameraPos = GameObject.Find("CameraPos").transform;
        //mCamera = GetComponent<Camera>();
        mRigidBody = GetComponent<Rigidbody>();
        mSphereCollider = GetComponent<SphereCollider>();
        mHealthComp = GetComponent<HealthComponent>();
        mRWTrace = GetComponentInChildren<RW_Trace>();
        mNavAgent = GetComponent<NavMeshAgent>();
        mAgent = GetComponent<UtilityAIProto.UAI_Agent>();
        mAnim = GetComponent<Animator>();
        mPreDestination = mRigidBody.transform.position;
        mAnim.SetInteger("WhatAmIDoing", 0);

        foreach (var e in FindObjectsOfType<AILogic>())
        {
            if (e)
            {
                if (e == this) { mCameraPos = e.GetComponentInChildren<FindCameraPos>().transform; continue; }
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

        if (bAtDestination)
        {
            if(!mCurrentTarget)
            {
                bHasEnemy.Value = false;
                bIsEnemyInDist.Value = false;
                bIsEnemyInAttackDist.Value = false;
                mKill.Value -= 40.0f * UtilityAIProto.UAI_Time.MyTime;
            }
        }
        else
        {
            MoveToDestination();
        }
    }

    private void AttackEnemy()
    {
        ResetUAI();

        if (bAtDestination)
        {
            bHasEnemy.Value = true;
            bIsEnemyInDist.Value = true;
            bIsEnemyInAttackDist.Value = true;
            mKill.Value += 60.0f * UtilityAIProto.UAI_Time.MyTime;
        }
        else
        {
            MoveToDestination();
        }
    }

    void MoveToDestination()
    {
        if(mCurrentTarget)
        {
            if(mNavAgent.hasPath)
            {
                if(mNavAgent.isPathStale)
                {
                    if(mCurrentTarget)
                    {
                        mDestination = mCurrentTarget.transform.position;
                        mPreDestination = mRigidBody.transform.position;
                        mNavAgent.SetDestination(mDestination);
                        //mAnim.SetInteger("WhatAmIDoing", (int)EAnimatorValue.Moving);
                        //mAnim.SetBool("IsMoving", mNavAgent.isStopped);
                    }
                    else
                    {
                        DetermineWhatToDo();
                    }
                }
            }
            else
            {
                mNavAgent.SetDestination(mCurrentTarget.transform.position);
            }
        }
        else
        {
            DetermineWhatToDo();
        }
    }

    private void GetAmmo()
    {
        ResetUAI();

        if(bAtDestination)
        {
            if (mRWTrace.AmmoState == Weapons.EAmmoState.High)
            {
                mAmmo.Value += 10.0f * UtilityAIProto.UAI_Time.MyTime;
            }
            if (mRWTrace.AmmoState == Weapons.EAmmoState.Medium)
            {
                mAmmo.Value += 25.0f * UtilityAIProto.UAI_Time.MyTime;
            }
            if (mRWTrace.AmmoState == Weapons.EAmmoState.Low)
            {
                mAmmo.Value += 65.0f * UtilityAIProto.UAI_Time.MyTime;
            }
        }
        else
        {
            //DetermineWhatToDo();
            MoveToDestination();
        }


        //bAtDestination = false;
        //mAmmoBoxes.TrimExcess();
        //mNavAgent.SetDestination(mDestination);
        //mNavAgent.Warp(mDestination);
        //mNavAgent.SetDestination(mAmmoBoxes.Values[0].transform.position);
        //mAmmo.Value += 5.0f * UtilityAIProto.UAI_Time.MyTime;
    }

    private void GetHealth()
    {
        ResetUAI();

        if (bAtDestination)
        {
            //mHealth.Value += 30.0f * UtilityAIProto.UAI_Time.MyTime;
            if (mHealthComp.HealthState == HealthComponent.EHealthState.Healthy)
            {
                mHealth.Value += 10.0f * UtilityAIProto.UAI_Time.MyTime;
            }
            if (mHealthComp.HealthState == HealthComponent.EHealthState.Injured)
            {
                mHealth.Value += 25.0f * UtilityAIProto.UAI_Time.MyTime;
            }
            if (mHealthComp.HealthState == HealthComponent.EHealthState.Severe)
            {
                mHealth.Value += 65.0f * UtilityAIProto.UAI_Time.MyTime;
            }
        }
        else
        {
            MoveToDestination();
            //mNavAgent.SetDestination(mDestination);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!bAtDestination)
        {
            //mNavAgent.SetDestination(mDestination);
            mAgent.UpdateUAI();
        }

        //if (mRigidBody.transform.position == mDestination ||
        //    mRigidBody.transform.position.x - 1 <= mDestination.x ||
        //    mRigidBody.transform.position.x + 1 >= mDestination.x)
        //{
        //    if (mRigidBody.transform.position.z - 1 <= mDestination.z ||
        //        mRigidBody.transform.position.z + 1 >= mDestination.z)
        //    {
        //        bAtDestination = true;
        //    }
        //}

        if(UtilityAIProto.UAI_Time.paused)
        {
            mNavAgent.isStopped = true;
            mAnim.SetInteger("WhatAmIDoing", 0);
        }
        else
        {
            mNavAgent.isStopped = false;
            mAnim.SetInteger("WhatAmIDoing", 1);
        }

        if(mAgent.IsPaused)
        {
            mNavAgent.isStopped = true;
            mAnim.SetInteger("WhatAmIDoing", 0);
        }
        else
        {
            mNavAgent.isStopped = false;
            mAnim.SetInteger("WhatAmIDoing", 1);
        }

        if(mDestination == mPreDestination)
        {
            ResetUAI();
        }

        if(mCurrentTarget)
        {
            var rott = Quaternion.LookRotation(mCurrentTarget.transform.position - mRigidBody.transform.position);
            mRigidBody.MoveRotation(Quaternion.Slerp(mRigidBody.transform.rotation, rott, t: UtilityAIProto.UAI_Time.MyTime * mRotateSpeed));
            Debug.Log(mAgent.name + " has target: " + mCurrentTarget.name + " and is in action: " + mAgent.TopAction.ToString());
        }

        if(mCurrentTarget.GetComponent<AILogic>())
        {
            bHasEnemy.Value = true;
            mDist = Vector3.Distance(mCurrentTarget.transform.position, mRigidBody.transform.position);
            if(mDist < mMaxDistance)
            {
                bIsEnemyInDist.Value = true;
                if(mDist <= mAttackDist)
                {
                    bIsEnemyInAttackDist.Value = true;
                    Ray ray = new Ray
                    {
                        origin = mRigidBody.transform.position,
                        direction = (mCurrentTarget.transform.position - mRigidBody.transform.position).normalized
                    };

                    RaycastHit hit = new RaycastHit();
                    if ((Vector3.Angle(ray.direction, mRigidBody.transform.forward)) < mFOVHalf * 2)
                    {
                        if (Physics.Raycast(ray.origin + Vector3.up * mHeightMul, ray.direction, out hit, mAttackDist))
                        {
                            if (hit.collider.transform.root.gameObject.GetComponent<AILogic>())
                            {
                                bCanSeeEnemy.Value = true;
                                mKill.Value += 90.0f * UtilityAIProto.UAI_Time.MyTime;
                                mRWTrace.OnShoot();
                                Debug.Log("Target: " + mCurrentTarget.name + " is close to the " + transform.name + " and is in front of " + transform.name);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            mKill.Value -= 150.0f * UtilityAIProto.UAI_Time.MyTime;
        }

        //if(bAtDestination)
        //{
        //    ResetUAI();
        //}
        mAgent.UpdateUAI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other) { return; }

        if (other.GetComponent<AILogic>())
        {
            bIsEnemyInDist.Value = true;
            bIsEnemyInAttackDist.Value = true;
            bHasEnemy.Value = true;
            mCurrentTarget = other.gameObject;
            var rott = Quaternion.LookRotation(other.transform.position - mRigidBody.transform.position);
            mRigidBody.MoveRotation(Quaternion.Slerp(mRigidBody.transform.rotation, rott, t: UtilityAIProto.UAI_Time.MyTime * mRotateSpeed));
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
                        bCanSeeEnemy.Value = true;
                        mKill.Value += 90.0f * UtilityAIProto.UAI_Time.MyTime;
                        mRWTrace.OnShoot();
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
            if (mMedBoxes.Count > 0)
            {
                for (var i = 0; i < mMedBoxes.Count; ++i)
                {
                    if (mMedBoxes.Values[i] != null)
                    {
                        if (mMedBoxes.Values[i].gameObject && mMedBoxes.Values[i].GetComponent<MedBox>())
                        {
                            float currentMag = (mMedBoxes.Values[i].transform.position - mRigidBody.transform.position).magnitude;
                            MedBox medbox = mMedBoxes.Values[i];
                            if (Vector3.Distance(mMedBoxes.Values[i].transform.position, mRigidBody.transform.position) <= mMaxDistance)
                            {
                                if (currentMag < mAmmoBoxes.Keys[i])
                                {
                                    mMedBoxes.Remove(mMedBoxes.Keys[i]);
                                    mMedBoxes.Add(currentMag, medbox);
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        mMedBoxes.RemoveAt(i);
                    }
                }
            }
            mPreDestination = mRigidBody.transform.position;
            mDestination = mMedBoxes.Values[0].transform.position;
            mCurrentTarget = mMedBoxes.Values[0].gameObject;
        }
        if (mAgent.TopAction.handle == GetAmmo)
        {
            if (mAmmoBoxes.Count > 0)
            {
                for (var i = 0; i < mAmmoBoxes.Count; ++i)
                {
                    if (mAmmoBoxes.Values[i] != null)
                    {
                        if (mAmmoBoxes.Values[i].gameObject && mAmmoBoxes.Values[i].GetComponent<AmmoBox>())
                        {
                            float currentMag = (mAmmoBoxes.Values[i].transform.position - mRigidBody.transform.position).magnitude;
                            AmmoBox ammobox = mAmmoBoxes.Values[i];
                            if (Vector3.Distance(mAmmoBoxes.Values[i].transform.position, mRigidBody.transform.position) <= mMaxDistance)
                            {
                                if (currentMag < mAmmoBoxes.Keys[i])
                                {
                                    mAmmoBoxes.Remove(mAmmoBoxes.Keys[i]);
                                    mAmmoBoxes.Add(currentMag, ammobox);
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        mAmmoBoxes.RemoveAt(i);
                    }
                }
            }
            mPreDestination = mRigidBody.transform.position;
            mDestination = mAmmoBoxes.Values[0].transform.position;
            mCurrentTarget = mAmmoBoxes.Values[0].gameObject;
        }
        if (mAgent.TopAction.handle == AttackEnemy)
        {
            if (mCurrentTarget && mCurrentTarget.GetComponent<AILogic>())
            {
                bHasEnemy.Value = true;
                var rott = Quaternion.LookRotation(mCurrentTarget.transform.position - mRigidBody.transform.position);
                mRigidBody.MoveRotation(Quaternion.Slerp(mRigidBody.transform.rotation, rott, t: UtilityAIProto.UAI_Time.MyTime * mRotateSpeed));
                if (Vector3.Distance(mCurrentTarget.transform.position, mRigidBody.transform.position) <= mAttackDist)
                {
                    bIsEnemyInDist.Value = true;
                    bIsEnemyInAttackDist.Value = true;
                    // Need to do a raycast to target to then set bCanSeeEnemy to true

                    float mag = (mRigidBody.transform.position - mCurrentTarget.transform.position).magnitude;
                    Ray ray = new Ray
                    {
                        origin = mRigidBody.transform.position,
                        direction = (mCurrentTarget.transform.position - mRigidBody.transform.position).normalized
                    };

                    RaycastHit hit = new RaycastHit();
                    if ((Vector3.Angle(ray.direction, mRigidBody.transform.forward)) < mFOVHalf * 2)
                    {
                        if (Physics.Raycast(ray.origin + Vector3.up * mHeightMul, ray.direction, out hit, mag))
                        {
                            if (hit.collider.transform.root.gameObject.GetComponent<AILogic>())
                            {
                                bCanSeeEnemy.Value = true;
                                Debug.Log("Target: " + mCurrentTarget.name + " is close to the " + transform.name + " and is in front of " + transform.name);
                                mRWTrace.OnShoot();
                            }
                        }
                    }
                }
                else
                {
                    mRWTrace.OnStopShooting();
                    mRWTrace.OnStopShoot();
                    bIsEnemyInAttackDist.Value = false;
                    mPreDestination = mRigidBody.transform.position;
                    mDestination = mCurrentTarget.transform.position;
                }
            }
            else
            {
                bHasEnemy.Value = false;
                bIsEnemyInAttackDist.Value = false;
                bIsEnemyInDist.Value = false;
                mKill.Value -= 20 * UtilityAIProto.UAI_Time.MyTime;
            }
        }
        if (mAgent.TopAction.handle == FindEnemy)
        {
            if (!mCurrentTarget)
            {
                if (mEnemies.Count > 0)
                {
                    for (var i = 0; i < mEnemies.Count; ++i)
                    {
                        if (mEnemies.Values[i].gameObject != null)
                        {
                            if (mEnemies.Values[i].GetComponent<AILogic>())
                            {
                                float currentMag = (mEnemies.Values[i].transform.position - mRigidBody.transform.position).magnitude;
                                GameObject AI = mEnemies.Values[i].gameObject;
                                if (Vector3.Distance(mEnemies.Values[i].transform.position, mRigidBody.transform.position) <= mMaxDistance)
                                {
                                    bIsEnemyInDist.Value = true;
                                    if (currentMag < mEnemies.Keys[i])
                                    {
                                        mEnemies.Remove(mEnemies.Keys[i]);
                                        mEnemies.Add(currentMag, AI);
                                    }
                                    if (Vector3.Distance(mEnemies.Values[i].transform.position, mRigidBody.transform.position) <= mAttackDist)
                                    {
                                        bIsEnemyInAttackDist.Value = true;
                                    }
                                }
                            }
                        }
                    }
                    mCurrentTarget = mEnemies.Values[0];
                    bHasEnemy.Value = true;
                    mPreDestination = mRigidBody.transform.position;
                    mDestination = mCurrentTarget.transform.position;
                }
            }
            else
            {
                bHasEnemy.Value = true;
                mPreDestination = mRigidBody.transform.position;
                mDestination = mCurrentTarget.transform.position;
            }
        }
    }


    void Fire()
    {
        if (mCurrentTarget)
        {
            if (mCurrentTarget.GetComponent<AILogic>())
            {
                Vector3 pos = mCurrentTarget.transform.position;
                pos.y = mRigidBody.transform.position.y;
                var rott = Quaternion.LookRotation(pos - mRigidBody.transform.position);
                mRigidBody.MoveRotation(Quaternion.Slerp(mRigidBody.transform.rotation, rott, t: UtilityAIProto.UAI_Time.MyTime * mRotateSpeed));

                Ray ray = new Ray
                {
                    origin = mRigidBody.transform.position,
                    direction = (mCurrentTarget.transform.position - mRigidBody.transform.position).normalized
                };
                if ((Vector3.Angle(ray.direction, mRigidBody.transform.forward)) < mFOVHalf * 2)
                {
                    Debug.Log("Target: " + mCurrentTarget.name + " is close to the " + transform.name + " and is in front of " + transform.name);

                    if (Vector3.Distance(ray.origin, mCurrentTarget.transform.position) <= mAttackDist)
                    {
                        bIsEnemyInAttackDist.Value = true;
                        mRWTrace.OnShoot();
                        // Play Attack Amimation
                        //mAnimCon.SetInteger("WhatAmIDoing", (int)EAnimatorValue.Firing);
                        Debug.Log(gameObject.name + " has Attacked " + mCurrentTarget.name);
                    }
                    else
                    {
                        bIsEnemyInAttackDist.Value = false;
                        return;
                    }
                }
            }
        }
    }
}
