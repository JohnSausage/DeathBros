using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FrameAnimation))]
public class CustomFrameAnimationInspector : Editor
{
    private FrameAnimation anim;
    private int frameCounter = 0;
    private float timer = 0;

    public override void OnInspectorGUI()
    {
        anim = (FrameAnimation)target;

        anim.animationName = EditorGUILayout.TextField("Name:", anim.animationName);

        
        EditorGUILayout.BeginVertical();
        if (anim.frames.Count > 0)
        {
            Repaint();
            Frame currentFrame = anim.frames[frameCounter];

            if (currentFrame.sprite != null)
                DrawTexturePreview(new Rect(80, 80, 100, 100), currentFrame.sprite);

            GUILayout.Space(120);

            timer += Time.unscaledDeltaTime;
            float duration = (currentFrame.duration);

            if (timer * 60 > duration)
            {
                frameCounter++;
                timer = 0;
            }

            if (frameCounter >= anim.frames.Count)
                frameCounter = 0;
        }
        EditorGUILayout.EndVertical();

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

    private void DrawTexturePreview(Rect position, Sprite sprite)
    {
        Vector2 fullSize = new Vector2(sprite.texture.width, sprite.texture.height);
        Vector2 size = new Vector2(sprite.textureRect.width, sprite.textureRect.height);

        Rect coords = sprite.textureRect;
        coords.x /= fullSize.x;
        coords.width /= fullSize.x;
        coords.y /= fullSize.y;
        coords.height /= fullSize.y;

        Vector2 ratio;
        ratio.x = position.width / size.x;
        ratio.y = position.height / size.y;
        float minRatio = Mathf.Min(ratio.x, ratio.y);

        Vector2 center = position.center;
        position.width = size.x * minRatio;
        position.height = size.y * minRatio;
        position.center = center;

        GUI.DrawTextureWithTexCoords(position, sprite.texture, coords);
    }
}
