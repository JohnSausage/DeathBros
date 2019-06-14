using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AIStatesSO))]
public class AIStatesSOInspector : Editor
{
    private AIStatesSO aIStatesSO;

    private AIActionSO addAIAction;
    private AIExitConditionSO addExit;

    //private GUIStyle actionStyle;
    //private GUIStyle exitStyle;

    //private void OnEnable()
    //{
    //    actionStyle = new GUIStyle()
    //    {
    //        alignment = TextAnchor.MiddleCenter,
    //        fontSize = 15,
    //        fontStyle = FontStyle.Bold
    //    };

    //    exitStyle = new GUIStyle()
    //    {
    //        alignment = TextAnchor.MiddleCenter,
    //        fontSize = 10,
    //        fontStyle = FontStyle.Bold
    //    };
    //}

    public override void OnInspectorGUI()
    {


        aIStatesSO = (AIStatesSO)target;

        AddAIAction();

        DisplayAIActions();

        EditorUtility.SetDirty(target);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        base.OnInspectorGUI();
    }

    private void AddAIAction()
    {
        GUILayout.Label("Add AIAction:");

        EditorGUILayout.BeginVertical("box");
        addAIAction = (AIActionSO)EditorGUILayout.ObjectField(addAIAction, typeof(AIActionSO), false);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        if (addAIAction != null)
        {
            aIStatesSO.aiActions.Add(addAIAction);
            addAIAction = null;
        }
    }

    private void AddExitCondition(AIActionSO aiAction)
    {
        GUILayout.Label("Add Exit Condition:");

        addExit = (AIExitConditionSO)EditorGUILayout.ObjectField(addExit, typeof(AIExitConditionSO), false);

        EditorGUILayout.Space();

        if (addExit != null)
        {
            aiAction.aiExitConditions.Add(addExit);
            addExit = null;
        }
    }

    private void DisplayAIActions()
    {
        if(aIStatesSO.aiActions == null)
        {
            aIStatesSO.aiActions = new List<AIActionSO>();
        }
        for (int i = 0; i < aIStatesSO.aiActions.Count; i++)
        {

            //EditorGUILayout.BeginVertical("box");

            //GUILayout.Label(aIStatesSO.aiActions[i].actionName, actionStyle);
            //EditorGUILayout.Space();

            aIStatesSO.aiActions[i] = (AIActionSO)EditorGUILayout.ObjectField(aIStatesSO.aiActions[i], typeof(AIActionSO), false);



            //foreach (AIExitConditionSO exitCOndition in aIStatesSO.aiActions[i].aiExitConditions)
            //{
            //    EditorGUILayout.BeginVertical("box");

            //    GUILayout.Label(exitCOndition.ToString(), exitStyle);
            //    exitCOndition.exitState = EditorGUILayout.TextField("Exit Action:", exitCOndition.exitState);
            //    EditorGUILayout.Space();


            //    EditorGUILayout.EndVertical();
            //    EditorGUILayout.Space();
            //}

            //AddExitCondition(aIStatesSO.aiActions[i]);

            //EditorGUILayout.EndVertical();
            //EditorGUILayout.Space();
        }
    }
}

[CustomPropertyDrawer(typeof(ScriptableObjectWithDrawer), true)]
public class ExtendedScriptableObjectDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float totalHeight = EditorGUIUtility.singleLineHeight;
        if (property.isExpanded)
        {
            var data = property.objectReferenceValue as ScriptableObject;
            if (data == null) return EditorGUIUtility.singleLineHeight;
            SerializedObject serializedObject = new SerializedObject(data);
            SerializedProperty prop = serializedObject.GetIterator();
            if (prop.NextVisible(true))
            {
                do
                {
                    if (prop.name == "m_Script") continue;
                    var subProp = serializedObject.FindProperty(prop.name);
                    float height = EditorGUI.GetPropertyHeight(subProp, null, true) + EditorGUIUtility.standardVerticalSpacing;
                    totalHeight += height;
                }
                while (prop.NextVisible(false));
            }
            // Add a tiny bit of height if open for the background
            totalHeight += EditorGUIUtility.standardVerticalSpacing;
        }
        return totalHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        if (property.objectReferenceValue != null)
        {
            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), property.isExpanded, property.displayName, true);
            EditorGUI.PropertyField(new Rect(EditorGUIUtility.labelWidth + 14, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), property, GUIContent.none, true);
            if (GUI.changed) property.serializedObject.ApplyModifiedProperties();
            if (property.objectReferenceValue == null) EditorGUIUtility.ExitGUI();

            if (property.isExpanded)
            {
                // Draw a background that shows us clearly which fields are part of the ScriptableObject
                GUI.Box(new Rect(0, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing - 1, Screen.width, position.height - EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing), "");

                EditorGUI.indentLevel++;
                var data = (ScriptableObject)property.objectReferenceValue;
                SerializedObject serializedObject = new SerializedObject(data);

                // Iterate over all the values and draw them
                SerializedProperty prop = serializedObject.GetIterator();
                float y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                if (prop.NextVisible(true))
                {
                    do
                    {
                        // Don't bother drawing the class file
                        if (prop.name == "m_Script") continue;
                        float height = EditorGUI.GetPropertyHeight(prop, new GUIContent(prop.displayName), true);
                        EditorGUI.PropertyField(new Rect(position.x, y, position.width, height), prop, true);
                        y += height + EditorGUIUtility.standardVerticalSpacing;
                    }
                    while (prop.NextVisible(false));
                }
                if (GUI.changed)
                    serializedObject.ApplyModifiedProperties();

                EditorGUI.indentLevel--;
            }
        }
        else
        {
            EditorGUI.ObjectField(new Rect(position.x, position.y, position.width - 60, EditorGUIUtility.singleLineHeight), property);
            if (GUI.Button(new Rect(position.x + position.width - 58, position.y, 58, EditorGUIUtility.singleLineHeight), "Create"))
            {
                string selectedAssetPath = "Assets";
                if (property.serializedObject.targetObject is MonoBehaviour)
                {
                    MonoScript ms = MonoScript.FromMonoBehaviour((MonoBehaviour)property.serializedObject.targetObject);
                    selectedAssetPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(ms));
                }
                Type type = fieldInfo.FieldType;
                if (type.IsArray) type = type.GetElementType();
                else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) type = type.GetGenericArguments()[0];
                property.objectReferenceValue = CreateAssetWithSavePrompt(type, selectedAssetPath);
            }
        }
        property.serializedObject.ApplyModifiedProperties();
        EditorGUI.EndProperty();
    }

    // Creates a new ScriptableObject via the default Save File panel
    ScriptableObject CreateAssetWithSavePrompt(Type type, string path)
    {
        path = EditorUtility.SaveFilePanelInProject("Save ScriptableObject", "New " + type.Name + ".asset", "asset", "Enter a file name for the ScriptableObject.", path);
        if (path == "") return null;
        ScriptableObject asset = ScriptableObject.CreateInstance(type);
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        EditorGUIUtility.PingObject(asset);
        return asset;
    }
}

