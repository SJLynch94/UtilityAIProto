using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RW_Trace : RangedWeapon
{
    [SerializeField] float xSpreadFactor = 0.07f;
    [SerializeField] float ySpreadFactor = 0.02f;

    public override void ShootWeapon()
    {
        GetComponentInParent<AILogic>().mAmmo.Value -= 3.0f * UtilityAIProto.UAI_Time.MyTime;
        Vector3 mShotOrigin = mMuzzleLocation.transform.position;
        Vector3 mShotDir = Vector3.zero;
        RaycastHit hit = new RaycastHit();
        Ray ray = new Ray();
        ray.origin = mMuzzleLocation.transform.position;

        ray.direction = mMuzzleLocation.transform.forward;
        mShotDir = mMuzzleLocation.transform.forward;

        mShotDir.x += Random.Range(-xSpreadFactor, xSpreadFactor);
        mShotDir.y += Random.Range(-ySpreadFactor, ySpreadFactor);

        ShootSingleTraceVector3(mShotOrigin, mShotDir, hit);
    }

    void ShootSingleTraceVector3(Vector3 origin, Vector3 direction, RaycastHit hit)
    {
        mMuzzleLocation.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        if (Physics.Raycast(origin, direction, out hit, mRange, 1, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject.transform.root.GetComponent<HealthComponent>() && hit.collider.gameObject != transform.root.gameObject)
            {
                hit.collider.gameObject.transform.root.GetComponent<HealthComponent>().Damage((short)mWeaponDamage);
            }
            // If bullet hits something that isn't a character, leave a bullet impact
            else if (hit.collider.gameObject != transform.root.gameObject && !hit.collider.gameObject.transform.root.GetComponentInChildren<AILogic>())
            {

                GameObject newImpact = Instantiate(mBulletImpact);
                newImpact.transform.position = hit.point;
                newImpact.transform.up = hit.normal;
                //newImpact2.transform.Translate(Vector3.up * 0.01f);

                GameObject newImpact2 = Instantiate(mBulletImpactImpact);
                newImpact2.transform.position = hit.point;
                newImpact2.transform.up = hit.normal;
                newImpact2.transform.Translate(Vector3.up * 0.01f);
            }
        }
        Debug.DrawLine(origin, origin + (direction * mRange), Color.green, 5.0f);
    }
}
