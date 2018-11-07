using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AIStatesSO))]
public class AIStatesSOInspector : Editor
{
    private enum AIStateType { Walk, Follow, Flee, Attack }
    private AIStatesSO aIStatesSO;

    private AIStateType addStateType;

    public override void OnInspectorGUI()
    {
        aIStatesSO = (AIStatesSO)target;

        addStateType = (AIStateType)EditorGUILayout.EnumPopup("Add state type:", addStateType);

        if (GUILayout.Button("Create State"))
        {
            switch (addStateType)
            {
                case AIStateType.Walk:
                    {
                        aIStatesSO.aiStates.Add(new AI_Walk());
                        return;
                    }

                default: return;
            }
        }

        EditorUtility.SetDirty(target);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        base.OnInspectorGUI();
    }

    private void DisplayAIState()
    {

    }
}
