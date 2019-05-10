using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedBox : MonoBehaviour
{
    [SerializeField]
    int mHealAmount = 25;

    void OnTriggerEnter(Collider other)
    {
        GetComponent<Animator>().SetBool("IsUsing", true);
        HealthComponent healing = other.GetComponentInChildren<HealthComponent>();
        if (healing)
        {
            healing.AddHealth(mHealAmount);
            Destroy(gameObject);
        }
    }
}
