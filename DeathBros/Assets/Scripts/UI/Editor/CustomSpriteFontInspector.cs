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

    protected int width = 0;

    public override void OnInspectorGUI()
    {
        spriteFont = (SpriteFont)target;

        if (spriteFont.chars == null)
        {
            spriteFont.chars = new List<SpriteFontCharacter>();
        }

        width = EditorGUILayout.IntField("Width:", width);

        if (GUILayout.Button("Set width for all"))
        {
            for (int i = 0; i < spriteFont.chars.Count; i++)
            {
                spriteFont.chars[i].width = width;
            }
        }

        for (int i = 0; i < spriteFont.chars.Count; i++)
        {
            //Texture2D spritePreview = AssetPreview.GetAssetPreview(spriteFont.chars[i].sprite);


            EditorGUILayout.BeginHorizontal("Box");

            EditorGUILayout.BeginVertical();
            spriteFont.chars[i].character = EditorGUILayout.TextField(spriteFont.chars[i].character.ToString())[0];
            spriteFont.chars[i].width = EditorGUILayout.IntField(spriteFont.chars[i].width);
            EditorGUILayout.EndVertical();

            //spriteFont.chars[i].sprite = (Sprite)EditorGUILayout.ObjectField("Spriter", spriteFont.chars[i].sprite, typeof(Sprite), false, GUILayout.Width(32), GUILayout.Height(32));
            spriteFont.chars[i].sprite = (Sprite)EditorGUILayout.ObjectField("Sprite:", spriteFont.chars[i].sprite, typeof(Sprite), false);

            //if (GUILayout.Button(spritePreview)) { }

            EditorGUILayout.EndHorizontal();
        }

        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("addSprites"), true);

        AddSpritesAsCharacters();
        serializedObject.ApplyModifiedProperties();

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

        char c = (char)32;

        for (int i = 0; i < spriteFont.addSprites.Count; i++)
        {
            spriteFont.chars.Add(new SpriteFontCharacter(c, spriteFont.addSprites[i]));
            c++;
        }

        spriteFont.addSprites.Clear();
    }
}