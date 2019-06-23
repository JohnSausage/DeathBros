using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(SpriteFont))]
public class CustomSpriteFontInspector : Editor
{
    private SpriteFont spriteFont;

    public List<Sprite> sprites;

    public override void OnInspectorGUI()
    {
        spriteFont = (SpriteFont)target;

        if (spriteFont.chars == null)
        {
            spriteFont.chars = new List<SpriteFontCharacter>();
        }

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

        if (GUILayout.Button("Add Empty"))
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

        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("addSprites"), true);

        AddSpritesAsCharacters();
        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();
    }

    private void GenerateEmpty(SpriteFont spriteFont)
    {
        for (char c = 'A'; c <= 'Z'; c++)
        {
            spriteFont.chars.Add(new SpriteFontCharacter(c));
        }

        for (char c = 'a'; c <= 'z'; c++)
        {
            spriteFont.chars.Add(new SpriteFontCharacter(c));
        }

        for (int i = 0; i < 9; i++)
        {
            spriteFont.chars.Add(new SpriteFontCharacter((char)i));
        }
    }

    private void AddSpritesAsCharacters()
    {
        if (spriteFont.addSprites == null)
        {
            spriteFont.addSprites = new List<Sprite>();
            return;
        }

        if (spriteFont.addSprites.Count == 0)
        {
            return;
        }

        char c = 'A';

        for (int i = 0; i < spriteFont.addSprites.Count; i++)
        {
            spriteFont.chars.Add(new SpriteFontCharacter(c, spriteFont.addSprites[i]));
            c++;

            if (c == ('Z' + 1))
            {
                c = 'a';
            }

            if (c == ('z' + 1))
            {
                c = '0';
            }
        }

        spriteFont.addSprites.Clear();
    }
}