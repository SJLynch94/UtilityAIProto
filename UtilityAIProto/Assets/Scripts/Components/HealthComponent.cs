using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public enum EHealthState { Healthy, Injured, Severe, Dead };
    EHealthState mHealthState = EHealthState.Healthy;
    protected int mHealth = 100;
    protected int mMaxHealth = 100;

    AILogic mAgent;
    protected int mMinHealth = 0;

    public EHealthState HealthState
    {
        get { return mHealthState; }
        set { mHealthState = value; }
    }

    public int Health
    {
        get { return mHealth; }
        set { mHealth = value; }
    }

    public void AddHealth(int h)
    {
        Health += h;
        Health = Mathf.Clamp(Health, mMinHealth, mMaxHealth);
    }

    public void Damage(int d)
    {
        if (Health > 0)
        {
            Health -= d;
            if(Health >= 75 && Health <= mMaxHealth)
            {
                HealthState = EHealthState.Healthy;
                mAgent.mHealth.Value += 10.0f * UtilityAIProto.UAI_Time.MyTime;
            }
            if (Health <= 74 && Health >= 40)
            {
                HealthState = EHealthState.Injured;
                mAgent.mHealth.Value += 25.0f * UtilityAIProto.UAI_Time.MyTime;
            }
            if(Health <= 39 && Health >= 1)
            {
                HealthState = EHealthState.Severe;
                mAgent.mHealth.Value += 65.0f * UtilityAIProto.UAI_Time.MyTime;
            }
            if (Health <= 0)
            {
                HealthState = EHealthState.Dead;
                mAgent.bHasHealth.Value = false;
                Destroy(gameObject);
            }
        }
        else if (Health <= 0)
        {
            HealthState = EHealthState.Dead;
            mAgent.bHasHealth.Value = false;
            Destroy(gameObject);
        }
    }

    void Awake()
    {
        mAgent = GetComponent<AILogic>();
    }
}
