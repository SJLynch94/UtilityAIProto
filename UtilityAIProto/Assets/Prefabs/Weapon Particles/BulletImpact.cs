using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletImpact : MonoBehaviour
{
    // Start is called before the first frame update
    protected float mBulletImpact = 60.0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        if ((mBulletImpact -= Time.fixedDeltaTime) <= 5.0f)         // SHAME
        if ((mBulletImpact -= Time.fixedDeltaTime) <= 5.0f)         // SHAME >:(
        {
            if (mBulletImpact <= 0.0f) Destroy(gameObject);
            Color col = GetComponent<MeshRenderer>().material.color;
            col.a -= (Time.fixedDeltaTime / 5.0f);
            GetComponent<MeshRenderer>().material.color = col;
        }

    }
}
