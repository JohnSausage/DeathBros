﻿using UnityEngine;
using UnityEditor;
using System;

public class FrameAnimatorEditor : EditorWindow
{
    [MenuItem("Window/FrameAnimator")]
    public static void ShowWindow()
    {
        GetWindow<FrameAnimatorEditor>("Frame Animator");
    }

    private FrameAnimator anim;
    private FrameAnimationsSO animSO;
    private FrameAnimation currentAnimation;
    private int currentAnimationNr;
    private Frame currentFrame;
    private int currentFrameNr;

    private FrameAnimation addFrameAnimation;

    private bool animationIsPlaying;
    private Vector2 animationListScrollVector;


    private Texture2D animationListTexture;
    private Texture2D previewAnimationTexture;
    private Texture2D frameButtonsTexture;
    private Texture2D generalFrameInfoTexture;
    private Texture2D specialFrameInfoTexture;

    private Rect animationListRect;
    private Rect previewAnimationRect;
    private Rect frameButtonsRect;
    private Rect generalFrameInfoRect;
    private Rect specialFrameInfoRect;


    void OnEnable()
    {
        EditorApplication.update += Update;
        InitTextures();
    }

    void OnDisable() { EditorApplication.update -= Update; }

    private void Update()
    {

    }

    private void OnGUI()
    {
        GameObject activeGO = Selection.activeGameObject;

        if (activeGO != null)
            anim = Selection.activeGameObject.GetComponent<FrameAnimator>();

        if (anim == null)
        {
            EditorGUILayout.HelpBox("No Frame Animator selected!", MessageType.Warning);
        }
        else if (anim.frameAnimationsSO == null)
        {
            EditorGUILayout.HelpBox("No Frame Animatons ScriptabelObject found!", MessageType.Warning);
        }
        else
        {
            animSO = anim.frameAnimationsSO;

            DrawLayouts();

            DrawAnimationList();
            DrawPreviewAnimation();
            DrawFrameButtons();
            DrawGeneralFrameInfo();
            DrawSpecialFrameInfo();
        }
    }

    private void InitTextures()
    {
        animationListTexture = new Texture2D(1, 1);
        animationListTexture.SetPixel(0, 0, Color.white);
        animationListTexture.Apply();

        previewAnimationTexture = new Texture2D(1, 1);
        previewAnimationTexture.SetPixel(0, 0, Color.black);
        previewAnimationTexture.Apply();

        frameButtonsTexture = new Texture2D(1, 1);
        frameButtonsTexture.SetPixel(0, 0, Color.gray);
        frameButtonsTexture.Apply();

        generalFrameInfoTexture = new Texture2D(1, 1);
        generalFrameInfoTexture.SetPixel(0, 0, Color.white);
        generalFrameInfoTexture.Apply();

        specialFrameInfoTexture = new Texture2D(1, 1);
        specialFrameInfoTexture.SetPixel(0, 0, Color.black);
        specialFrameInfoTexture.Apply();

    }

    private void DrawLayouts()
    {
        animationListRect.x = 0;
        animationListRect.y = 0;
        animationListRect.width = 300;
        animationListRect.height = Screen.height / 2f;

        previewAnimationRect.x = 0;
        previewAnimationRect.y = Screen.height / 2f;
        previewAnimationRect.width = animationListRect.width;
        previewAnimationRect.height = Screen.height / 2f;

        frameButtonsRect.x = animationListRect.width;
        frameButtonsRect.y = 0;
        frameButtonsRect.width = Screen.width - animationListRect.width;
        frameButtonsRect.height = 50;

        generalFrameInfoRect.x = animationListRect.width;
        generalFrameInfoRect.y = frameButtonsRect.height;
        generalFrameInfoRect.width = 300;
        generalFrameInfoRect.height = Screen.height - frameButtonsRect.height;

        specialFrameInfoRect.x = generalFrameInfoRect.x + generalFrameInfoRect.width;
        specialFrameInfoRect.y = frameButtonsRect.height;
        specialFrameInfoRect.width = Screen.width - animationListRect.width - generalFrameInfoRect.width;
        specialFrameInfoRect.height = Screen.height - frameButtonsRect.height;

        GUI.DrawTexture(animationListRect, animationListTexture);
        GUI.DrawTexture(previewAnimationRect, previewAnimationTexture);
        GUI.DrawTexture(frameButtonsRect, frameButtonsTexture);
        GUI.DrawTexture(generalFrameInfoRect, generalFrameInfoTexture);
        GUI.DrawTexture(specialFrameInfoRect, specialFrameInfoTexture);
    }

    private void DrawAnimationList()
    {
        GUILayout.BeginArea(animationListRect);

        GUILayout.Label("Add Animation:");
        addFrameAnimation = (FrameAnimation)EditorGUILayout.ObjectField(addFrameAnimation, typeof(FrameAnimation), false);

        if (addFrameAnimation != null)
        {
            animSO.frameAnimations.Add(addFrameAnimation);
            addFrameAnimation = null;
        }

        animationListScrollVector = GUILayout.BeginScrollView(animationListScrollVector);
        {
            GUILayout.Label("Found animations:");
            for (int i = 0; i < animSO.frameAnimations.Count; i++)
            {
                if (i == currentAnimationNr)
                {
                    GUI.color = Color.grey;
                }

                if (GUILayout.Button(animSO.frameAnimations[i].animationName))
                {
                    currentAnimation = animSO.frameAnimations[i];
                    currentAnimationNr = i;
                }

                GUI.color = Color.white;
            }

            for (int i = 0; i < 50; i++)
            {
                GUILayout.Button("test " + i);
            }

        }
        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }

    private void DrawPreviewAnimation()
    {
        GUILayout.BeginArea(previewAnimationRect);


        EditorGUILayout.Space();


        if (animationIsPlaying)
            GUI.color = Color.green;

        if (GUILayout.Button("Play Animation"))
        {
            animationIsPlaying = !animationIsPlaying;
        }
        GUI.color = Color.white;


        EditorGUILayout.Space();

        Rect previewRect = new Rect(0, 0, 128, 128);


        if (currentFrame != null)
        {
            //DrawTexturePreview(new Vector2(previewRect.x + 10, previewRect.y + 10), currentFrame.sprite, 4);
        }


        EditorGUILayout.Space();


        GUI.color = Color.red;
        if (GUILayout.Button("Remove Animation"))
        {
            animSO.frameAnimations.Remove(animSO.frameAnimations[currentAnimationNr]);
        }
        GUI.color = Color.white;

        GUILayout.EndArea();
    }

    private void DrawFrameButtons()
    {

    }

    private void DrawGeneralFrameInfo()
    {

    }

    private void DrawSpecialFrameInfo()
    {

    }



    /*

    private Vector2 zero = Vector2.zero;
    private float pixelPerUnit = 16;

    private int frameCounter = 0;
    private float timer = 0;
    private bool playing = false;
    private bool showHurtboxes = false;

    private Vector2 animationListScrollVector;
    private Vector2 frameListSrollVector;


    void Update()
    {

    }

    private void OnGUI()
    {
        GameObject activeGO = Selection.activeGameObject;

        if (activeGO != null)
            anim = Selection.activeGameObject.GetComponent<FrameAnimator>();


        if (anim == null)
        {
            GUILayout.Label("No Frame Animator selected");
        }
        else
        {
            GUILayout.BeginHorizontal(GUILayout.MinWidth(180));
            {

                ShowFrameAnimationList();

                ShowFramesList();


                GUILayout.BeginVertical();
                {

                    var rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none,
                        GUILayout.MinWidth(148), GUILayout.MinHeight(148));

                    GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);

                    PreviewCurrentAnimation();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }
    }

    private void ShowFrameAnimationList()
    {
        animationListScrollVector = GUILayout.BeginScrollView(animationListScrollVector, GUILayout.MinWidth(200));
        {
            GUILayout.Label("Found animations:");

            for (int i = 0; i < anim.animations.Count; i++)
            {
                if (i == currentAnimationNr)
                {
                    GUI.color = Color.grey;
                }

                if (GUILayout.Button(anim.animations[i].animationName, GUILayout.MaxWidth(160)))
                {
                    currentAnimation = anim.animations[i];
                    currentAnimationNr = i;
                }

                GUI.color = Color.white;
            }

            GUILayout.Label("Add Animation:");
            addFrameAnimation = (FrameAnimation)EditorGUILayout.ObjectField(addFrameAnimation, typeof(FrameAnimation), false);

            if (addFrameAnimation != null)
            {
                anim.animations.Add(addFrameAnimation);
                addFrameAnimation = null;
            }

        }
        GUILayout.EndScrollView();
    }

    private void ShowFramesList()
    {
        GUILayout.BeginVertical(GUILayout.MinWidth(160));
        {
            EditorGUILayout.Space();

            if (currentAnimation != null)
            {
                if (currentAnimation.animationName == "")
                    currentAnimation.animationName = currentAnimation.name;

                currentAnimation.animationName = EditorGUILayout.TextField("Animation:", currentAnimation.animationName);
                //GUILayout.Label(currentAnimation.animationName);
            }

            if (playing)
                GUI.color = Color.green;

            if (GUILayout.Button("Play Animation"))
            {
                playing = !playing;
            }
            GUI.color = Color.white;

            EditorGUILayout.Space();

            GUI.color = Color.red;
            if (GUILayout.Button("Remove Animation"))
            {
                anim.animations.Remove(anim.animations[currentAnimationNr]);
            }
            GUI.color = Color.white;

            EditorGUILayout.Space();


            if (currentAnimation != null)
            {
                for (int i = 0; i < currentAnimation.frames.Count; i++)
                {
                    if (currentFrameNr == i)
                        GUI.color = Color.gray;

                    if (GUILayout.Button("Frame " + i))
                    {
                        currentFrame = currentAnimation.frames[i];
                        currentFrameNr = i;
                    }

                    GUI.color = Color.white;
                }
            }
        }
        GUILayout.EndVertical();
    }

    private void PreviewCurrentAnimation()
    {
        Rect rect = GUILayoutUtility.GetLastRect();

        if (currentFrame != null)
        {
            //DrawTexturePreview(new Vector2(rect.x + 10, rect.y + 10), currentFrame.sprite, 4);
        }
    }

    */

    private void DrawTexturePreview(Vector2 offset, Sprite sprite, float scale)
    {
        Vector2 fullSize = new Vector2(sprite.texture.width, sprite.texture.height);
        Vector2 size = sprite.rect.size;

        Rect coords = sprite.rect;

        coords.x /= fullSize.x;
        coords.width /= fullSize.x;
        coords.y /= fullSize.y;
        coords.height /= fullSize.y;

        Rect posRect = new Rect
        {
            size = size * scale,

            //x = zero.x - size.x / 2 * scale,
            //y = zero.y - size.y / 2 * scale

            x = -size.x / 2 * scale,
            y = -size.y / 2 * scale
        };


        //posRect.x += offset.x * pixelPerUnit * scale;
        //posRect.y += offset.y * pixelPerUnit * scale;


        posRect.x = offset.x;
        posRect.y = offset.y;

        GUI.DrawTextureWithTexCoords(posRect, sprite.texture, coords);
    }

}
