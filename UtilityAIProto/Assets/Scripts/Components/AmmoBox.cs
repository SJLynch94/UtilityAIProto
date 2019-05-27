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
        RW_Trace gl = other.GetComponentInChildren<RW_Trace>();
        if (gl)
        {
            StartCoroutine(Destroy(gl));
        }
        else
        {
            GetComponent<Animator>().SetBool("IsUsing", false);
        }
    }

    IEnumerator Destroy(RW_Trace gl)
    {
        if (gl.AddAmmo(mAmmoAmount))
        {
            //gl.GetComponent<AILogic>().mAmmoBoxes.Remove(this.GetComponent<AmmoBox>());
            //gl.GetComponentInParent<AILogic>().mAmmo.Value += mAmmoAmount * UtilityAIProto.UAI_Time.MyTime;
            yield return new WaitForSeconds(3.0f);
            Destroy(gameObject);
        }
        else
        {
            yield break;
        }
    }
}
