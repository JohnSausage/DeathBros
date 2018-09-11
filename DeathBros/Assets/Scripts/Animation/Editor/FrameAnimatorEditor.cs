﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

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

    private Hurtbox settingHurtbox, copyHurtbox;
    private FrameAnimation addFrameAnimation;

    private bool animationIsPlaying;
    private Vector2 animationListScrollVector;
    private int specialInfoTabNr = 0;
    private float timer = 0;

    private int pixelPerUnit = 16;
    private float scale = 4f;
    private Vector2 zero = Vector2.zero;

    private Texture2D grayTexture;
    private Texture2D blackTexture;
    private Texture2D frameButtonsTexture;
    private Texture2D whiteTexture;
    private Texture2D specialFrameInfoTexture;

    private Rect animationListRect = Rect.zero;
    private Rect previewAnimationRect = Rect.zero;
    private Rect frameButtonsRect = Rect.zero;
    private Rect generalFrameInfoRect = Rect.zero;
    private Rect specialFrameInfoRect = Rect.zero;


    void OnEnable()
    {
        InitTextures();
        //EditorApplication.update += Update;

        Repaint();
    }

    void OnDisable()
    {
        //EditorApplication.update -= Update;
    }

    private void OnLostFocus()
    {
    }

    private void Update()
    {
        if (animSO != null)
        {
            currentFrame = animSO.frameAnimations[currentAnimationNr].frames[currentFrameNr];


            if (animationIsPlaying)
            {
                timer += Time.deltaTime * 6 / 100; //Editor Updates Faster
                float duration = (currentFrame.duration);

                if (timer > duration / 60)
                {
                    currentFrameNr++;
                    timer = 0;
                }


                if (currentFrameNr >= animSO.frameAnimations[currentAnimationNr].frames.Count)
                    currentFrameNr = 0;

                Repaint();
            }
        }
    }

    private void OnGUI()
    {
        if (grayTexture == null)
        {
            InitTextures();
        }


        GameObject activeGO = Selection.activeGameObject;

        if (activeGO == null)
        {
            EditorGUILayout.HelpBox("No GameObject selected!", MessageType.Warning);
        }
        else
        {
            anim = Selection.activeGameObject.GetComponent<FrameAnimator>();
        }

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
            EditorGUI.BeginChangeCheck();

            animSO = anim.frameAnimationsSO;

            DrawLayouts();

            DrawAnimationList();
            DrawPreviewAnimation();
            DrawFrameButtons();
            DrawGeneralFrameInfo();
            DrawSpecialFrameInfo();

            //if (currentAnimation != null)
            //    Undo.RecordObject(currentAnimation, "Changed Animation");

            //if (currentAnimation != null)
            //    EditorUtility.SetDirty(currentAnimation);

            //if (animSO != null)
            //    EditorUtility.SetDirty(animSO);
        }
    }

    private void SaveChanges()
    {
        if (animSO != null)
        {
            EditorUtility.SetDirty(animSO);

            foreach (FrameAnimation animation in animSO.frameAnimations)
            {
                EditorUtility.SetDirty(animation);
            }
        }
    }

    private void InitTextures()
    {
        grayTexture = new Texture2D(1, 1);
        grayTexture.SetPixel(0, 0, Color.gray);
        grayTexture.Apply();

        blackTexture = new Texture2D(1, 1);
        blackTexture.SetPixel(0, 0, Color.black);
        blackTexture.Apply();

        frameButtonsTexture = new Texture2D(1, 1);
        frameButtonsTexture.SetPixel(0, 0, Color.gray);
        frameButtonsTexture.Apply();

        whiteTexture = new Texture2D(1, 1);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();

        specialFrameInfoTexture = new Texture2D(1, 1);
        specialFrameInfoTexture.SetPixel(0, 0, Color.black);
        specialFrameInfoTexture.Apply();

    }

    private void DrawLayouts()
    {
        animationListRect.x = 0;
        animationListRect.y = 0;
        animationListRect.width = 160;
        animationListRect.height = Screen.height - previewAnimationRect.height;

        previewAnimationRect.x = 0;
        previewAnimationRect.y = animationListRect.height;
        previewAnimationRect.width = animationListRect.width;
        previewAnimationRect.height = 200;

        frameButtonsRect.x = animationListRect.width;
        frameButtonsRect.y = 0;
        frameButtonsRect.width = Screen.width - animationListRect.width;
        frameButtonsRect.height = 30;

        generalFrameInfoRect.x = animationListRect.width;
        generalFrameInfoRect.y = frameButtonsRect.height;
        generalFrameInfoRect.width = 200;
        generalFrameInfoRect.height = Screen.height - frameButtonsRect.height;

        specialFrameInfoRect.x = generalFrameInfoRect.x + generalFrameInfoRect.width;
        specialFrameInfoRect.y = frameButtonsRect.height;
        specialFrameInfoRect.width = Screen.width - animationListRect.width - generalFrameInfoRect.width;
        specialFrameInfoRect.height = Screen.height - frameButtonsRect.height;

        animationListRect.size -= new Vector2(2, 2);
        previewAnimationRect.size -= new Vector2(2, 2);
        frameButtonsRect.size -= new Vector2(2, 2);
        generalFrameInfoRect.size -= new Vector2(2, 2);
        specialFrameInfoRect.size -= new Vector2(2, 2);

        GUI.DrawTexture(animationListRect, grayTexture);
        GUI.DrawTexture(previewAnimationRect, grayTexture);
        GUI.DrawTexture(frameButtonsRect, grayTexture);
        GUI.DrawTexture(generalFrameInfoRect, grayTexture);
        GUI.DrawTexture(specialFrameInfoRect, grayTexture);
    }

    private void DrawAnimationList()
    {
        GUILayout.BeginArea(animationListRect);

        GUILayout.Label("Add Animation:");

        EditorGUILayout.BeginVertical("box");
        addFrameAnimation = (FrameAnimation)EditorGUILayout.ObjectField(addFrameAnimation, typeof(FrameAnimation), false);
        EditorGUILayout.EndVertical();

        if (addFrameAnimation != null)
        {
            animSO.frameAnimations.Add(addFrameAnimation);
            addFrameAnimation = null;
        }

        GUILayout.Label("Found animations:");

        animationListScrollVector = EditorGUILayout.BeginScrollView(animationListScrollVector, "box");
        {
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
                    currentFrameNr = 0;

                    if (currentAnimation.frames.Count > 0)
                    {
                        currentFrame = currentAnimation.frames[currentFrameNr];
                    }
                }

                GUI.color = Color.white;
            }
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        GUI.color = Color.cyan;
        if (GUILayout.Button("Save Changes", GUILayout.MinHeight(40)))
        {
            SaveChanges();
        }
        GUI.color = Color.white;

        GUILayout.EndArea();
    }

    private void DrawPreviewAnimation()
    {
        GUILayout.BeginArea(previewAnimationRect);


        GUILayout.Space(5);


        if (animationIsPlaying)
            GUI.color = Color.green;

        if (GUILayout.Button("Play Animation"))
        {
            animationIsPlaying = !animationIsPlaying;
        }
        GUI.color = Color.white;


        Rect previewRect = Rect.zero;

        if (currentFrame != null)
        {
            Vector2 position = new Vector2(20, 25);
            previewRect = DrawTextureRect(position, currentFrame.sprite, 4, false);

            GUI.DrawTexture(previewRect, blackTexture);
            previewRect = DrawTextureRect(position, currentFrame.sprite, 4);
        }

        GUILayout.Space(previewRect.height);


        GUI.color = Color.red;
        if (GUILayout.Button("Remove Animation"))
        {
            currentAnimation = null;
            animSO.frameAnimations.Remove(animSO.frameAnimations[currentAnimationNr]);
            currentAnimationNr = 0;
            currentFrame = null;
            currentFrameNr = 0;
        }
        GUI.color = Color.white;

        GUILayout.EndArea();
    }

    private void DrawFrameButtons()
    {
        GUILayout.BeginArea(frameButtonsRect);

        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal("box");

        if (currentAnimation != null)
        {
            EditorGUILayout.LabelField("Frames:", GUILayout.MaxWidth(50));

            for (int i = 0; i < currentAnimation.frames.Count; i++)
            {
                if (currentAnimation.frames[i].sprite == null)
                {
                    GUI.color = Color.red;
                }

                if (currentFrameNr == i)
                {
                    GUI.color = Color.gray;
                }

                if (GUILayout.Button((i + 1).ToString(), GUILayout.MaxWidth(20)))
                {
                    currentFrameNr = i;
                    currentFrame = currentAnimation.frames[i];
                }

                GUI.color = Color.white;
            }
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    private void DrawGeneralFrameInfo()
    {
        GUILayout.BeginArea(generalFrameInfoRect);


        EditorGUILayout.LabelField("Insert Frame");

        EditorGUILayout.BeginHorizontal("box");

        if (GUILayout.Button("Before"))
        {
            currentAnimation.frames.Insert(currentFrameNr, new Frame());
            currentFrameNr++;
        }

        if (GUILayout.Button("After"))
        {
            currentAnimation.frames.Insert(currentFrameNr + 1, new Frame());
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.LabelField("General Frame Info:");

        EditorGUILayout.BeginVertical("box");
        if (currentFrame != null)
        {
            EditorGUILayout.LabelField("Sprite:");

            Rect previewRect = DrawTextureRect(new Vector2(35, 90), currentFrame.sprite, 4, false);

            GUI.DrawTexture(previewRect, blackTexture);

            previewRect = DrawTextureRect(new Vector2(35, 90), currentFrame.sprite, 4);

            GUILayout.Space(previewRect.height + 10);


            currentFrame.sprite = (Sprite)EditorGUILayout.ObjectField("Change Sprite:", currentFrame.sprite, typeof(Sprite), false);

            currentFrame.duration = EditorGUILayout.IntField("Duration:", currentFrame.duration);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUI.color = Color.red;
        if (GUILayout.Button("Remove Frame"))
        {
            currentAnimation.frames.Remove(currentFrame);
            currentFrame = null;
            currentFrameNr = 0;
        }
        GUI.color = Color.white;

        GUILayout.EndArea();
    }

    private void DrawSpecialFrameInfo()
    {
        GUILayout.BeginArea(specialFrameInfoRect);

        EditorGUILayout.LabelField("Special Frame Info:");


        EditorGUILayout.BeginHorizontal();

        if (specialInfoTabNr == 0)
            GUI.color = Color.gray;
        if (GUILayout.Button("Hurtboxes"))
        {
            specialInfoTabNr = 0;
        }
        GUI.color = Color.white;

        if (specialInfoTabNr == 1)
            GUI.color = Color.gray;
        if (GUILayout.Button("Hitboxes"))
        {
            specialInfoTabNr = 1;
        }
        GUI.color = Color.white;

        if (specialInfoTabNr == 2)
            GUI.color = Color.gray;
        if (GUILayout.Button("Projectiles"))
        {
            specialInfoTabNr = 2;
        }
        GUI.color = Color.white;

        EditorGUILayout.EndHorizontal();


        if (currentFrame != null)
        {
            if(specialInfoTabNr == 0)
            {
                DisplayHurtboxTab();
                SetHurtboxWithMouse();
            }
        }

        GUILayout.EndArea();
    }

    private void DisplayHurtboxTab()
    {
        if (currentFrame.hurtBoxes == null) currentFrame.hurtBoxes = new List<Hurtbox>();

        GUI.color = new Color32(180, 200, 180, 255);
        for (int i = 0; i < currentFrame.hurtBoxes.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");
            {
                currentFrame.hurtBoxes[i].position = EditorGUILayout.Vector2Field("Position", currentFrame.hurtBoxes[i].position);
                currentFrame.hurtBoxes[i].radius = EditorGUILayout.FloatField("Radius", currentFrame.hurtBoxes[i].radius);

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Set"))
                    {
                        settingHurtbox = currentFrame.hurtBoxes[i];
                    }

                    if (GUILayout.Button("Copy"))
                    {
                        copyHurtbox = currentFrame.hurtBoxes[i];
                    }

                    if (GUILayout.Button("Paste") && copyHurtbox != null)
                    {
                        currentFrame.hurtBoxes[i] = copyHurtbox.Clone();
                    }

                    if (GUILayout.Button("Delete"))
                    {
                        currentFrame.hurtBoxes.Remove(currentFrame.hurtBoxes[i]);
                        break;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        GUI.color = Color.white;

        if (GUILayout.Button("Add HurtBox"))
        {
            currentFrame.hurtBoxes.Add(new Hurtbox());
        }
    }

    private void SetHurtboxWithMouse()
    {
        if (settingHurtbox != null)
        {
            settingHurtbox.position.x = ((Event.current.mousePosition - zero) / pixelPerUnit / scale).x;
            settingHurtbox.position.y = -((Event.current.mousePosition - zero) / pixelPerUnit / scale).y;
        }

        if (settingHurtbox != null && Event.current.type == EventType.ScrollWheel)
        {
            settingHurtbox.radius += Event.current.delta.y * 0.01f;
        }

        if (settingHurtbox != null && Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            settingHurtbox = null;
        }
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

    private Rect DrawTextureRect(Vector2 offset, Sprite sprite, float scale, bool drawTexture = true)
    {
        Rect posRect = Rect.zero;

        if (sprite != null)
        {
            Vector2 fullSize = new Vector2(sprite.texture.width, sprite.texture.height);
            Vector2 size = sprite.rect.size;

            Rect coords = sprite.rect;

            coords.x /= fullSize.x;
            coords.width /= fullSize.x;
            coords.y /= fullSize.y;
            coords.height /= fullSize.y;

            posRect = new Rect
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

            if (drawTexture)
                GUI.DrawTextureWithTexCoords(posRect, sprite.texture, coords);
        }

        return posRect;
    }

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
