using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {


    public GameObject mMedBoxPrefab;
    public GameObject mAmmoBoxPrefab;
    public Terrain mTerrain;

    float mTerrainWidth;
    float mTerrainLength;
    float mXTerrainPos;
    float mZTerrainPos;
    float mYVal;
    public float mYOffset = 0.5f;

    [SerializeField]
    int mAmmoBoxesAmount;

    [SerializeField]
    int mMedBoxesAmount;

	// Use this for initialization
	void Start () {

        mTerrainWidth = mTerrain.terrainData.size.x;
        mTerrainLength = mTerrain.terrainData.size.z;

        mXTerrainPos = mTerrain.transform.position.x;
        mZTerrainPos = mTerrain.transform.position.z;

        for (var i = 0; i < mAmmoBoxesAmount; ++i)
        {
            float rx = Random.Range(mXTerrainPos, mXTerrainPos + mTerrainWidth);
            float rz = Random.Range(mZTerrainPos, mZTerrainPos + mTerrainLength);
            mYVal = Terrain.activeTerrain.SampleHeight(new Vector3(rx, 0, rz));
            mYVal += mYOffset;
            GameObject g = Instantiate(mAmmoBoxPrefab, new Vector3(rx, mYVal, rz), Quaternion.identity);
        }

        for(var i = 0; i < mMedBoxesAmount; ++i)
        {
            float rx = Random.Range(mXTerrainPos, mXTerrainPos + mTerrainWidth);
            float rz = Random.Range(mZTerrainPos, mZTerrainPos + mTerrainLength);
            mYVal = Terrain.activeTerrain.SampleHeight(new Vector3(rx, 0, rz));
            mYVal += mYOffset;
            GameObject g = Instantiate(mMedBoxPrefab, new Vector3(rx, mYVal, rz), Quaternion.identity);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
