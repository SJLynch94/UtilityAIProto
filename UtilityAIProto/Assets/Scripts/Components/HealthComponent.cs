﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public enum EHealthState { Healthy, Injured, Severe, Dead };
    EHealthState mHealthState = EHealthState.Healthy;
    protected int mHealth = 100;
    protected int mMaxHealth = 100;
    protected int mMinHealth = 0;

    UtilityAIProto.OverlayUI UI;
    AILogic mAgent;

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
        if (Health >= 75 && Health <= mMaxHealth)
        {
            HealthState = EHealthState.Healthy;
        }
        if (Health <= 74 && Health >= 40)
        {
            HealthState = EHealthState.Injured;
        }
        if (Health <= 39 && Health >= 1)
        {
            HealthState = EHealthState.Severe;
        }
    }

    public void Damage(int d)
    {
        if (Health > 0)
        {
            Health -= d;
            if(Health >= 75 && Health <= mMaxHealth)
            {
                HealthState = EHealthState.Healthy;
            }
            if (Health <= 74 && Health >= 40)
            {
                HealthState = EHealthState.Injured;
            }
            if(Health <= 39 && Health >= 1)
            {
                HealthState = EHealthState.Severe;
            }
            if (Health <= 0)
            {
                StartCoroutine(Die());
            }
        }
        else if (Health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        if (UI)
        {
            if (GetComponent<UtilityAIProto.Agent>() == UI.DisplayedAgent)
            {
                Camera.main.transform.parent = null;
            }
        }
        HealthState = EHealthState.Dead;
        mAgent.bHasHealth.Value = false;
        GetComponent<Animator>().SetBool("IsDead", true);
        yield return new WaitForSeconds(3.0f);
        UI.RemoveAgent(transform.name);
        Destroy(gameObject);
    }

    void Awake()
    {
        mAgent = GetComponent<AILogic>();
        UI = FindObjectOfType<UtilityAIProto.OverlayUI>() as UtilityAIProto.OverlayUI;
    }
}
