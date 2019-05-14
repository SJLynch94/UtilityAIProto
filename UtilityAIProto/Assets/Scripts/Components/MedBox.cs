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
        HealthComponent healing = other.GetComponent<HealthComponent>();
        if (healing)
        {
            StartCoroutine(Destroy(healing));
        }
        else
        {
            GetComponent<Animator>().SetBool("IsUsing", false);
        }
    }

    IEnumerator Destroy(HealthComponent healing)
    {
        healing.AddHealth(mHealAmount);
        //healing.GetComponent<AILogic>().mMedBoxes.Remove(this);
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }
}
