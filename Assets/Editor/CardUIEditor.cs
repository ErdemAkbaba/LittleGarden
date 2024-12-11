using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.TerrainTools;

[CustomEditor(typeof(CardUI))]
public class CardUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CardUI cardUI = (CardUI)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("LevelUp"))
        {
            //cardUI.LevelUp();
        }
    }
}
