using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameEditor : Editor
{
    bool bDrawGizmos = true;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameManager myScript = (GameManager)target;

        if (GUILayout.Button("Spawn Ammo Boxes"))
        {
            myScript.SpawnAmmoBoxes();
            List<AmmoBox> ammo = new List<AmmoBox>(FindObjectsOfType<AmmoBox>());
            foreach (var w in ammo) EditorUtility.SetDirty(w);
            EditorUtility.SetDirty(this);
        }

        if (GUILayout.Button("Spawn Med Boxes"))
        {
            myScript.SpawnMedBoxes();
            List<MedBox> med = new List<MedBox>(FindObjectsOfType<MedBox>());
            foreach (var w in med) EditorUtility.SetDirty(w);
            EditorUtility.SetDirty(this);
        }

        if (GUILayout.Button("Spawn AI"))
        {
            myScript.SpawnAI();
            List<AILogic> AI = new List<AILogic>(FindObjectsOfType<AILogic>());
            foreach (var w in AI) EditorUtility.SetDirty(w);
            EditorUtility.SetDirty(this);
        }

        if (GUILayout.Button("Clear Ammo Boxes"))
        {
            myScript.ClearAmmoBoxes();
            List<AmmoBox> ammo = new List<AmmoBox>(FindObjectsOfType<AmmoBox>());
            foreach (var w in ammo) EditorUtility.SetDirty(w);
            EditorUtility.SetDirty(this);
        }

        if (GUILayout.Button("Clear Med Boxes"))
        {
            myScript.ClearMedBoxes();
            List<MedBox> med = new List<MedBox>(FindObjectsOfType<MedBox>());
            foreach (var w in med) EditorUtility.SetDirty(w);
            EditorUtility.SetDirty(this);
        }

        if (GUILayout.Button("Clear AI"))
        {
            myScript.ClearAI();
            List<AILogic> AI = new List<AILogic>(FindObjectsOfType<AILogic>());
            foreach (var w in AI) EditorUtility.SetDirty(w);
            EditorUtility.SetDirty(this);
        }
    }
}
