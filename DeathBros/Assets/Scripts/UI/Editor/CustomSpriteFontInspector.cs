using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(SpriteFont))]
public class CustomSpriteFontInspector : Editor
{
    private SpriteFont spriteFont;

    public override void OnInspectorGUI()
    {
        spriteFont = (SpriteFont)target;

        for (int i = 0; i < spriteFont.chars.Count; i++)
        {
            Texture2D spritePreview = AssetPreview.GetAssetPreview(spriteFont.chars[i].sprite);


            EditorGUILayout.BeginHorizontal("Box");

            EditorGUILayout.BeginVertical();
            spriteFont.chars[i].character = EditorGUILayout.TextField(spriteFont.chars[i].character.ToString())[0];

            spriteFont.chars[i].sprite = (Sprite)EditorGUILayout.ObjectField(spriteFont.chars[i].sprite, typeof(Sprite), true);
            EditorGUILayout.EndVertical();

            if (GUILayout.Button(spritePreview)) { }

            EditorGUILayout.EndHorizontal();
        }

        if(GUILayout.Button("Add Empty"))
        {
            spriteFont.chars.Add(new SpriteFontCharacter());
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Empty"))
        {
            GenerateEmpty(spriteFont);
        }

        if (GUILayout.Button("Delete Everything"))
        {
            spriteFont.chars.Clear();
        }

        base.OnInspectorGUI();
    }

    private void GenerateEmpty(SpriteFont spriteFont)
    {
        for (char c = 'a'; c <= 'z'; c++)
        {
            spriteFont.chars.Add(new SpriteFontCharacter(c));
        }

        for (int i = 0; i < 9; i++)
        {
            spriteFont.chars.Add(new SpriteFontCharacter((char)i));
        }
    }
}
