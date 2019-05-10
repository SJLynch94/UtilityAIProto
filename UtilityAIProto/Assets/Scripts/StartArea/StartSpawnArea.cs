using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSpawnArea : MonoBehaviour {

    public float areaSizeX;
    public float areaSizeZ;
    public float AIAmount;
    public GameObject AI;

    public GameObject GameManager;

    protected Vector3 spawnPosition;

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3.up * 1.0f), new Vector3(areaSizeX, 2.0f, areaSizeZ));
    }

    public Vector3 GetSpawnPosition()
    {
        return transform.position + new Vector3(Random.Range(-areaSizeX / 2.0f, areaSizeX / 2.0f), 0.0f, Random.Range(-areaSizeZ / 2.0f, areaSizeZ / 2.0f));
    }

    public void Start()
    {
        //for(int i = 0; i < AIAmount; i++)
        //{
        //    spawnPosition = GetSpawnPosition();
        //    Instantiate(AI, spawnPosition, Quaternion.identity);
        //}
    }
}
