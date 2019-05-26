using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {


    public GameObject mMedBoxPrefab;
    public GameObject mAmmoBoxPrefab;
    public Terrain mTerrain;

    [SerializeField]
    public float mAIAmount;
    public GameObject AI;

    List<AILogic> mAIList = new List<AILogic>();
    List<AmmoBox> mAmmoList = new List<AmmoBox>();
    List<MedBox> mMedList = new List<MedBox>();

    float mTerrainWidth;
    float mTerrainLength;
    float mXTerrainPos;
    float mZTerrainPos;
    float mYVal;
    public float mYOffset = 0.2f;

    [SerializeField]
    int mAmmoBoxesAmount;

    [SerializeField]
    int mMedBoxesAmount;

    List<GameObject> mPlayers = new List<GameObject>();

	// Use this for initialization
	void Awake () {

        mAIAmount = PlayerPrefs.GetInt("mAIAmount");

        mTerrainWidth = mTerrain.terrainData.size.x;
        mTerrainLength = mTerrain.terrainData.size.z;

        mXTerrainPos = mTerrain.transform.position.x;
        mZTerrainPos = mTerrain.transform.position.z;
    }

    void Start()
    {
        GameObject g;
        for (var i = 0; i < mAIAmount; ++i)
        {
            float rx = Random.Range(mXTerrainPos, mXTerrainPos + mTerrainWidth);
            float rz = Random.Range(mZTerrainPos, mZTerrainPos + mTerrainLength);
            mYVal = Terrain.activeTerrain.SampleHeight(new Vector3(rx, 0, rz));
            mYVal += mYOffset;
            g = Instantiate(AI, new Vector3(rx, mYVal, rz), Quaternion.identity);
            g.GetComponent<UtilityAIProto.UAI_Agent>().agentName = "Soldier " + i;
            mPlayers.Add(g);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void SpawnAmmoBoxes()
    {
        ClearAmmoBoxes();
        mTerrainWidth = mTerrain.terrainData.size.x;
        mTerrainLength = mTerrain.terrainData.size.z;

        mXTerrainPos = mTerrain.transform.position.x;
        mZTerrainPos = mTerrain.transform.position.z;
        GameObject g;
        for (var i = 0; i < mAmmoBoxesAmount; ++i)
        {
            float rx = Random.Range(mXTerrainPos, mXTerrainPos + mTerrainWidth);
            float rz = Random.Range(mZTerrainPos, mZTerrainPos + mTerrainLength);
            mYVal = Terrain.activeTerrain.SampleHeight(new Vector3(rx, 0, rz));
            mYVal += mYOffset;
            g = Instantiate(mAmmoBoxPrefab, new Vector3(rx, mYVal, rz), Quaternion.identity);
            mAmmoList.Add(g.GetComponent<AmmoBox>());
        }
    }

    public void SpawnMedBoxes()
    {
        ClearMedBoxes();
        mTerrainWidth = mTerrain.terrainData.size.x;
        mTerrainLength = mTerrain.terrainData.size.z;

        mXTerrainPos = mTerrain.transform.position.x;
        mZTerrainPos = mTerrain.transform.position.z;
        GameObject g;
        for (var i = 0; i < mMedBoxesAmount; ++i)
        {
            float rx = Random.Range(mXTerrainPos, mXTerrainPos + mTerrainWidth);
            float rz = Random.Range(mZTerrainPos, mZTerrainPos + mTerrainLength);
            mYVal = Terrain.activeTerrain.SampleHeight(new Vector3(rx, 0, rz));
            mYVal += mYOffset;
            g = Instantiate(mMedBoxPrefab, new Vector3(rx, mYVal, rz), Quaternion.identity);
            mMedList.Add(g.GetComponent<MedBox>());
        }
    }

    public void SpawnAI()
    {
        ClearAI();
        mTerrainWidth = mTerrain.terrainData.size.x;
        mTerrainLength = mTerrain.terrainData.size.z;

        mXTerrainPos = mTerrain.transform.position.x;
        mZTerrainPos = mTerrain.transform.position.z;
        GameObject g;
        for (var i = 0; i < mAIAmount; ++i)
        {
            float rx = Random.Range(mXTerrainPos, mXTerrainPos + mTerrainWidth);
            float rz = Random.Range(mZTerrainPos, mZTerrainPos + mTerrainLength);
            mYVal = Terrain.activeTerrain.SampleHeight(new Vector3(rx, 0, rz));
            mYVal += mYOffset;
            g = Instantiate(AI, new Vector3(rx, mYVal, rz), Quaternion.identity);
            g.GetComponent<UtilityAIProto.UAI_Agent>().agentName = "Soldier " + i;
            mAIList.Add(g.GetComponent<AILogic>());
        }
    }

    public void ClearAI()
    {
        foreach (var w in mAIList) if (w && w.gameObject) DestroyImmediate(w.gameObject);
        mAIList.Clear();
    }

    public void ClearAmmoBoxes()
    {
        foreach (var w in mAmmoList) if (w && w.gameObject) DestroyImmediate(w.gameObject);
        mAmmoList.Clear();
    }

    public void ClearMedBoxes()
    {
        foreach (var w in mMedList) if (w && w.gameObject) DestroyImmediate(w.gameObject);
        mMedList.Clear();
    }
}
