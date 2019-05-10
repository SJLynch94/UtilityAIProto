using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    [SerializeField]
    int mAmmoAmount = 50;

    void OnTriggerEnter(Collider other)
    {
        GetComponent<Animator>().SetBool("IsUsing", true);
        GunLogic gl = other.GetComponentInChildren<GunLogic>();
        if (gl)
        {
            gl.AddAmmo(mAmmoAmount);
            Destroy(gameObject);
        }
    }
}
