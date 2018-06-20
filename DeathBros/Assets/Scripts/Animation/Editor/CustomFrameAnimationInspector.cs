using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FrameAnimation))]
public class CustomFrameAnimationInspector : Editor
{
    private FrameAnimation anim;

    public override void OnInspectorGUI()
    {
        anim = (FrameAnimation)target;

        anim.animationName = EditorGUILayout.TextField("Name:", anim.animationName);

        for (int i = 0; i < anim.frames.Count; i++)
        {
            DisplayFrame(anim.frames[i]);
        }

        if (GUILayout.Button("Add Frame"))
        {
            anim.frames.Add(new Frame());
        }

        EditorGUILayout.Space();

        EditorUtility.SetDirty(target);

        base.OnInspectorGUI();
    }

    private void DisplayFrame(Frame frame)
    {
        EditorGUILayout.BeginVertical("box");
        {
            frame.duration = EditorGUILayout.IntField("Duration:", frame.duration);

            frame.sprite = (Sprite)EditorGUILayout.ObjectField("Sprite:", frame.sprite, typeof(Sprite), false);
        }
        EditorGUILayout.EndVertical();
    }
}
