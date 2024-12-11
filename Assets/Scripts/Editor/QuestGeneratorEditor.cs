using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QuestGenerator))]
public class QuestGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Quests"))
        {
            QuestGenerator questGenerator = (QuestGenerator)target;
            questGenerator.Generate();
        }
        
    }
}
