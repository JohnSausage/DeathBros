﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

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

    private Hurtbox settingHurtbox;
    private List<Hurtbox> copyHurtboxes = new List<Hurtbox>();
    private Hitbox settingHitbox;
    private List<Hitbox> copyHitboxes = new List<Hitbox>();

    private Vector2 hboxSettingPosition;
    private Vector2 hitboxSettingPosition;

    private bool showHurtboxes = true;
    private bool showHitboxes = true;

    private FrameAnimation addFrameAnimation;

    private bool animationIsPlaying;
    private Vector2 animationListScrollVector;
    private Vector2 generalFrameInfoScrollVector;
    private Vector2 specialFrameInfoScrollVector;
    private int specialInfoTabNr = 0;
    private float timer = 0;
    private bool showDamageDetails = false;
    private string searchString = "";

    private string[] soundNames;
    private int selectSound = 0;
    private int pixelPerUnit = 16;
    private float scale = 4f;

    private bool editAllFrames;

    private Texture2D grayTexture;
    private Texture2D blackTexture;
    private Texture2D whiteTexture;
    private Texture2D frameButtonsTexture;
    private Texture2D specialFrameInfoTexture;

    private Texture2D hurtboxTexture;
    private Texture2D hitboxTexture;

    private Rect animationListRect = Rect.zero;
    private Rect previewAnimationRect = Rect.zero;
    private Rect frameButtonsRect = Rect.zero;
    private Rect generalFrameInfoRect = Rect.zero;
    private Rect specialFrameInfoRect = Rect.zero;

    private Rect previewRect = Rect.zero;

    public Sprite[] addSpritesArray;

    public List<Sprite> addSpritesAsFrames;

    void OnEnable()
    {
        InitTextures();
        //EditorApplication.update += Update;
        Repaint();
    }

    private void LoadSoundNames()
    {
        if (anim != null)
        {
            SoundsSO soundsSO = anim.GetComponentInChildren<SoundContainer>().soundsSO;

            soundNames = soundsSO.sounds.Select(x => x.name).ToArray();
        }
    }

    void OnDisable()
    {
        SaveChanges();
        //EditorApplication.update -= Update;
    }

    private void OnLostFocus()
    {
        SaveChanges();
    }

    private void Update()
    {
        if (animSO != null && currentAnimation != null && currentFrame != null)
        {
            if (currentAnimation.frames.Count > 0)
            {
                currentFrame = currentAnimation.frames[currentFrameNr];


                if (animationIsPlaying)
                {
                    timer += Time.deltaTime * 6 / 100; //Editor Updates Faster
                    float duration = (currentFrame.duration);

                    if (timer > duration / 60)
                    {
                        currentFrameNr++;
                        timer = 0;
                    }


                    if (currentFrameNr >= currentAnimation.frames.Count)
                        currentFrameNr = 0;

                    Repaint();
                }
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

            if (currentAnimation != null)
            {
                if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.LeftArrow)
                {
                    currentFrameNr--;
                    if (currentFrameNr < 0) currentFrameNr = currentAnimation.frames.Count - 1;
                }
                if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.RightArrow)
                {
                    currentFrameNr++;
                    if (currentFrameNr >= currentAnimation.frames.Count) currentFrameNr = 0;
                }
            }
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
        specialFrameInfoTexture.SetPixel(0, 0, Color.gray);
        specialFrameInfoTexture.Apply();

        hurtboxTexture = Resources.Load<Texture2D>("Editor/green_circle");
        hitboxTexture = Resources.Load<Texture2D>("Editor/red_circle");
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
        previewAnimationRect.height = 80;

        frameButtonsRect.x = animationListRect.width;
        frameButtonsRect.y = 0;
        frameButtonsRect.width = Screen.width - animationListRect.width;
        frameButtonsRect.height = 30;

        generalFrameInfoRect.x = animationListRect.width;
        generalFrameInfoRect.y = frameButtonsRect.height;
        generalFrameInfoRect.width = (Screen.width - animationListRect.width) / 2;
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

        searchString = EditorGUILayout.TextField(searchString);

        GUILayout.Label("Found animations:");

        animationListScrollVector = EditorGUILayout.BeginScrollView(animationListScrollVector, "box");

        if (animSO.frameAnimations == null)
        {
            animSO.frameAnimations = new List<FrameAnimation>();
        }

        for (int i = 0; i < animSO.frameAnimations.Count; i++)
        {
            if (i == currentAnimationNr)
            {
                GUI.color = Color.grey;
            }

            if (animSO.frameAnimations[i].name.Contains(searchString))
            {
                if (GUILayout.Button(animSO.frameAnimations[i].name))
                {
                    currentAnimation = animSO.frameAnimations[i];
                    currentAnimationNr = i;
                    currentFrameNr = 0;
                    currentFrame = null;

                    if (currentAnimation.frames == null)
                    {
                        currentAnimation.frames = new List<Frame>();
                    }

                    if (currentAnimation.frames.Count > 0)
                    {
                        currentFrame = currentAnimation.frames[currentFrameNr];
                    }

                }
            }

            GUI.color = Color.white;
        }

        if (GUILayout.Button("Add New (wip)"))
        {

        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        //object field with th current animation
        if (currentAnimation != null)
        {
            currentAnimation = (FrameAnimation)EditorGUILayout.ObjectField(currentAnimation, typeof(FrameAnimation), false);
        }

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


        /*
        Rect previewRect = Rect.zero;

        if (currentFrame != null)
        {
            Vector2 position = new Vector2(20, 25);
            previewRect = DrawSpriteRect(position, currentFrame.sprite, 4, blackTexture);
        }

        GUILayout.Space(previewRect.height);
        */

        GUI.color = Color.red;
        if (GUILayout.Button("Remove Animation"))
        {
            if (EditorUtility.DisplayDialog("Remove Animation", "Remove current animation?", "Remove", "Cancel"))
            {
                currentAnimation = null;
                animSO.frameAnimations.Remove(animSO.frameAnimations[currentAnimationNr]);
                currentAnimationNr = 0;
                currentFrame = null;
                currentFrameNr = 0;
            }
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

        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty array = so.FindProperty("addSpritesAsFrames");

        EditorGUILayout.PropertyField(array);
        so.ApplyModifiedProperties();
        if (addSpritesAsFrames != null)
        {
            foreach (Sprite s in addSpritesAsFrames)
            {
                Frame spriteFrame = new Frame();
                spriteFrame.sprite = s;
                currentAnimation.frames.Add(spriteFrame);
            }

            EditorGUILayout.Space();

            addSpritesAsFrames.Clear();
        }
        GUILayout.EndArea();
    }

    private void DrawGeneralFrameInfo()
    {
        GUILayout.BeginArea(generalFrameInfoRect);

        if (currentFrame == null)
        {
            EditorGUILayout.BeginHorizontal("box");

            if (GUILayout.Button("Add Frame"))
            {
                currentAnimation.frames.Insert(currentFrameNr, new Frame());
                currentFrameNr = 0;
                currentFrame = currentAnimation.frames[currentFrameNr];
            }

            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.LabelField("Add Or Remove Frames");

            EditorGUILayout.BeginHorizontal("box");

            if (GUILayout.Button("Before"))
            {
                currentAnimation.frames.Insert(currentFrameNr, new Frame());
                currentFrameNr++;
            }

            GUI.color = Color.red;
            if (GUILayout.Button("Remove"))
            {
                if (EditorUtility.DisplayDialog("Remove Frame", "Remove current frame?", "Remove", "Cancel"))
                {
                    currentAnimation.frames.Remove(currentFrame);
                    currentFrame = null;
                    currentFrameNr = 0;
                }
            }
            GUI.color = Color.white;

            if (GUILayout.Button("After"))
            {
                currentAnimation.frames.Insert(currentFrameNr + 1, new Frame());
            }
            EditorGUILayout.EndHorizontal();

            editAllFrames = EditorGUILayout.Toggle("Edit All Frames", editAllFrames);

            EditorGUILayout.LabelField("General Frame Info:");

            generalFrameInfoScrollVector = EditorGUILayout.BeginScrollView(generalFrameInfoScrollVector, "box");
            if (currentFrame != null)
            {
                scale = EditorGUILayout.Slider("Scale:", scale, 1f, 16f);


                EditorGUILayout.LabelField("Sprite:");

                Vector2 previewPosition = new Vector2(35, 40);

                previewRect = DrawSpriteRect(previewPosition, currentFrame.sprite, scale, grayTexture);

                GUILayout.Space(previewRect.height + 10);

                if (settingHurtbox != null)
                {
                    Repaint();

                    hboxSettingPosition = Event.current.mousePosition - previewPosition;

                    if (previewRect.Contains(Event.current.mousePosition))
                    {
                        settingHurtbox.position.x = ((hboxSettingPosition.x - previewRect.width / 2) / pixelPerUnit / scale);
                        settingHurtbox.position.y = -((hboxSettingPosition.y - previewRect.height / 2) / pixelPerUnit / scale);

                        if (Event.current.type == EventType.ScrollWheel)
                        {
                            settingHurtbox.radius += Event.current.delta.y * 0.01f;
                        }

                        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
                        {
                            settingHurtbox = null;
                        }
                    }
                }

                if (settingHitbox != null)
                {
                    Repaint();

                    hboxSettingPosition = Event.current.mousePosition - previewPosition;

                    if (previewRect.Contains(Event.current.mousePosition))
                    {
                        settingHitbox.position.x = ((hboxSettingPosition.x - previewRect.width / 2) / pixelPerUnit / scale);
                        settingHitbox.position.y = -((hboxSettingPosition.y - previewRect.height / 2) / pixelPerUnit / scale);

                        if (Event.current.type == EventType.ScrollWheel)
                        {
                            settingHitbox.radius += Event.current.delta.y * 0.01f;
                        }

                        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
                        {
                            settingHitbox = null;
                        }
                    }
                }




                Rect hboxRect = new Rect();

                if (showHurtboxes)
                {
                    if (currentFrame.hurtboxes != null)
                    {
                        foreach (Hurtbox h in currentFrame.hurtboxes)
                        {

                            hboxRect.size = new Vector2(h.radius * 2, h.radius * 2) * scale * pixelPerUnit;
                            hboxRect.center = previewRect.center + new Vector2(h.position.x, -h.position.y) * scale * pixelPerUnit;

                            GUI.DrawTexture(hboxRect, hurtboxTexture);

                            //Check if hurtbox is clicked
                            if (hboxRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                            {
                                settingHurtbox = h;
                            }

                        }
                    }
                }

                if (showHitboxes)
                {
                    if (currentFrame.hitboxes != null)
                    {
                        foreach (Hitbox h in currentFrame.hitboxes)
                        {

                            var fillColorArray = hitboxTexture.GetPixels();
                            if (h.damage == null) h.damage = new Damage();
                            Color color = h.damage.editorColor;
                            if (color == new Color(0, 0, 0, 0)) color = Color.red;
                            color.a = 0.25f;

                            for (int i = 0; i < fillColorArray.Length; i++)
                            {
                                if (fillColorArray[i].a > 0f)
                                    fillColorArray[i] = color;
                            }

                            hitboxTexture.SetPixels(fillColorArray);
                            hitboxTexture.Apply();

                            hboxRect.size = new Vector2(h.radius * 2, h.radius * 2) * scale * pixelPerUnit;
                            hboxRect.center = previewRect.center + new Vector2(h.position.x, -h.position.y) * scale * pixelPerUnit;

                            GUI.DrawTexture(hboxRect, hitboxTexture);

                            //Check if hurtbox is clicked
                            if (hboxRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                            {
                                settingHitbox = h;
                            }

                        }
                    }
                }

                showHurtboxes = EditorGUILayout.Toggle("Show Hurtboxes", showHurtboxes);
                showHitboxes = EditorGUILayout.Toggle("Show Hitboxes", showHitboxes);

                currentFrame.sprite = (Sprite)EditorGUILayout.ObjectField("Change Sprite:", currentFrame.sprite, typeof(Sprite), false);

                currentFrame.spawnProjectile = EditorGUILayout.Vector2Field("Projectile", currentFrame.spawnProjectile);
                currentFrame.spawnHoldItem = (GameObject)EditorGUILayout.ObjectField("Hold Item", currentFrame.spawnHoldItem, typeof(GameObject), false);

                currentFrame.duration = EditorGUILayout.IntField("Duration:", currentFrame.duration);
                if (editAllFrames)
                {
                    foreach (Frame f in currentAnimation.frames)
                    {
                        f.duration = currentFrame.duration;
                    }
                }

                currentFrame.newHitID = EditorGUILayout.Toggle("New Hit ID:", currentFrame.newHitID);
            }
            EditorGUILayout.EndScrollView();

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

        }

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
        if (GUILayout.Button("Sounds"))
        {
            specialInfoTabNr = 2;
        }
        GUI.color = Color.white;

        if (specialInfoTabNr == 3)
            GUI.color = Color.gray;
        if (GUILayout.Button("Movement"))
        {
            specialInfoTabNr = 3;
        }
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();


        if (currentFrame != null)
        {
            if (specialInfoTabNr == 0)
            {
                DisplayHurtboxTab();
            }
            else if (specialInfoTabNr == 1)
            {
                DisplayHitboxTab();
            }
            else if (specialInfoTabNr == 2)
            {
                DisplaySoundTab();
            }
            else if (specialInfoTabNr == 3)
            {
                DisplayMovementTab();
            }
        }

        GUILayout.EndArea();
    }


    private void DisplayHurtboxTab()
    {
        if (currentFrame.hurtboxes == null) currentFrame.hurtboxes = new List<Hurtbox>();

        GUI.color = new Color32(180, 200, 180, 255);
        for (int i = 0; i < currentFrame.hurtboxes.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");
            {
                currentFrame.hurtboxes[i].position = EditorGUILayout.Vector2Field("Position", currentFrame.hurtboxes[i].position);
                currentFrame.hurtboxes[i].radius = EditorGUILayout.FloatField("Radius", currentFrame.hurtboxes[i].radius);

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Set"))
                    {
                        settingHurtbox = currentFrame.hurtboxes[i];
                    }

                    if (GUILayout.Button("Copy"))
                    {
                        copyHurtboxes.Clear();
                        copyHurtboxes.Add(currentFrame.hurtboxes[i].Clone());
                    }

                    if (GUILayout.Button("Paste"))
                    {
                        if (copyHurtboxes.Count > 0)
                            currentFrame.hurtboxes[i] = copyHurtboxes[0].Clone();
                    }

                    if (GUILayout.Button("Delete"))
                    {
                        currentFrame.hurtboxes.Remove(currentFrame.hurtboxes[i]);
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
            currentFrame.hurtboxes.Add(new Hurtbox());
        }

        if (GUILayout.Button("Copy All"))
        {
            copyHurtboxes.Clear();

            for (int i = 0; i < currentFrame.hurtboxes.Count; i++)
            {
                copyHurtboxes.Add(currentFrame.hurtboxes[i].Clone());
            }
        }

        if (GUILayout.Button("Paste All"))
        {

            for (int i = 0; i < copyHurtboxes.Count; i++)
            {
                currentFrame.hurtboxes.Add(copyHurtboxes[i].Clone());
            }
        }

        if (GUILayout.Button("Paste All To All Frames"))
        {

            foreach (Frame frame in currentAnimation.frames)
            {
                for (int i = 0; i < copyHurtboxes.Count; i++)
                {
                    frame.hurtboxes.Add(copyHurtboxes[i].Clone());
                }
            }
        }
    }

    private void DisplayHitboxTab()
    {
        specialFrameInfoScrollVector = EditorGUILayout.BeginScrollView(specialFrameInfoScrollVector);
        if (currentFrame.hitboxes == null) currentFrame.hitboxes = new List<Hitbox>();

        GUI.color = new Color32(200, 180, 180, 255);

        if (GUILayout.Button("Add Damage Type"))
        {
            if (currentAnimation.damages == null) currentAnimation.damages = new List<Damage>();

            currentAnimation.damages.Add(new Damage());
        }

        if (currentAnimation.damages != null)
        {
            for (int i = 0; i < currentAnimation.damages.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Nr " + (i + 1));
                currentAnimation.damages[i].damageNumber = EditorGUILayout.FloatField("Damage", currentAnimation.damages[i].damageNumber);
                currentAnimation.damages[i].knockBackDirection = EditorGUILayout.Vector2Field("Knockback Direction", currentAnimation.damages[i].knockBackDirection);
                currentAnimation.damages[i].positionalInfluence = EditorGUILayout.Vector2Field("Positional Influence", currentAnimation.damages[i].positionalInfluence);
                currentAnimation.damages[i].baseKnockback = EditorGUILayout.FloatField("Base Knockback", currentAnimation.damages[i].baseKnockback);
                currentAnimation.damages[i].knockbackGrowth = EditorGUILayout.FloatField("Knockback Growth", currentAnimation.damages[i].knockbackGrowth);
                currentAnimation.damages[i].hitStunFrames = EditorGUILayout.IntField("Hitstun Frames", currentAnimation.damages[i].hitStunFrames);
                currentAnimation.damages[i].damageType = (EDamageType)EditorGUILayout.EnumPopup("Damage Type", currentAnimation.damages[i].damageType);

                currentAnimation.damages[i].StatusEffect = (StatusEffect)EditorGUILayout.ObjectField("Status Effect:", currentAnimation.damages[i].StatusEffect, typeof(StatusEffect), false);

                currentAnimation.damages[i].editorColor = EditorGUILayout.ColorField("Color", currentAnimation.damages[i].editorColor);

                if (GUILayout.Button("Remove")) currentAnimation.damages.Remove(currentAnimation.damages[i]);
                if (GUILayout.Button("Set Hitbox"))
                {
                    foreach (Frame frame in currentAnimation.frames)
                    {
                        foreach (Hitbox hitbox in frame.hitboxes)
                        {
                            if (hitbox.damage.damageType == currentAnimation.damages[i].damageType)
                            {
                                hitbox.damage = currentAnimation.damages[i].Clone();
                            }
                        }
                    }
                }

                EditorGUILayout.EndVertical();
            }

            if (currentAnimation.damages.Count == 0) currentAnimation.damages = null;
        }

        showDamageDetails = EditorGUILayout.Toggle("Show Damage Details", showDamageDetails);

        for (int i = 0; i < currentFrame.hitboxes.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");
            {
                currentFrame.hitboxes[i].position = EditorGUILayout.Vector2Field("Position", currentFrame.hitboxes[i].position);
                currentFrame.hitboxes[i].radius = EditorGUILayout.FloatField("Radius", currentFrame.hitboxes[i].radius);

                if (currentFrame.hitboxes[i].damage == null)
                {
                    currentFrame.hitboxes[i].damage = new Damage();
                }

                currentFrame.hitboxes[i].damage.damageType = (EDamageType)EditorGUILayout.EnumPopup("Damage Type", currentFrame.hitboxes[i].damage.damageType);

                EditorGUILayout.BeginHorizontal();
                if (currentAnimation.damages != null)
                {
                    for (int j = 0; j < currentAnimation.damages.Count; j++)
                    {
                        if (GUILayout.Button((j + 1).ToString()))
                        {
                            currentFrame.hitboxes[i].damage = currentAnimation.damages[j];
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (showDamageDetails)
                {
                    EditorGUILayout.BeginVertical("box");
                    {
                        if (currentFrame.hitboxes[i].damage == null)
                        {
                            currentFrame.hitboxes[i].damage = new Damage();
                        }
                        EditorGUILayout.LabelField("Damage Options:");

                        currentFrame.hitboxes[i].damage.damageNumber = EditorGUILayout.FloatField("Damage", currentFrame.hitboxes[i].damage.damageNumber);
                        currentFrame.hitboxes[i].damage.knockBackDirection = EditorGUILayout.Vector2Field("Knockback Direction", currentFrame.hitboxes[i].damage.knockBackDirection);
                        currentFrame.hitboxes[i].damage.positionalInfluence = EditorGUILayout.Vector2Field("Positional Influence", currentFrame.hitboxes[i].damage.positionalInfluence);
                        currentFrame.hitboxes[i].damage.baseKnockback = EditorGUILayout.FloatField("Base Knockback", currentFrame.hitboxes[i].damage.baseKnockback);
                        currentFrame.hitboxes[i].damage.knockbackGrowth = EditorGUILayout.FloatField("Knockback Growth", currentFrame.hitboxes[i].damage.knockbackGrowth);
                        currentFrame.hitboxes[i].damage.hitStunFrames = EditorGUILayout.IntField("Hitstun Frames", currentFrame.hitboxes[i].damage.hitStunFrames);

                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Set"))
                        {
                            settingHitbox = currentFrame.hitboxes[i];
                        }

                        if (GUILayout.Button("Copy"))
                        {
                            copyHitboxes.Clear();
                            copyHitboxes.Add(currentFrame.hitboxes[i].Clone());
                        }

                        if (GUILayout.Button("Paste"))
                        {
                            if (copyHitboxes.Count > 0)
                                currentFrame.hitboxes[i] = copyHitboxes[0].Clone();
                        }

                        if (GUILayout.Button("Delete"))
                        {
                            currentFrame.hitboxes.Remove(currentFrame.hitboxes[i]);
                            break;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }
        GUI.color = Color.white;




        if (GUILayout.Button("Add HitBox"))
        {
            currentFrame.hitboxes.Add(new Hitbox());
        }

        if (GUILayout.Button("Copy All"))
        {
            copyHitboxes.Clear();

            for (int i = 0; i < currentFrame.hitboxes.Count; i++)
            {
                copyHitboxes.Add(currentFrame.hitboxes[i].Clone());
            }
        }

        if (GUILayout.Button("Paste All"))
        {

            for (int i = 0; i < copyHitboxes.Count; i++)
            {
                currentFrame.hitboxes.Add(copyHitboxes[i].Clone());
            }
        }

        EditorGUILayout.EndScrollView();
    }



    private void DisplaySoundTab()
    {
        EditorGUILayout.BeginVertical("box");

        if (GUILayout.Button("Load Sound Names"))
        {
            LoadSoundNames();
        }

        if (soundNames != null)
        {
            selectSound = EditorGUILayout.Popup(selectSound, soundNames);

            if (GUILayout.Button("Use Sound"))
            {
                currentFrame.soundName = soundNames[selectSound];
            }
        }

        currentFrame.soundName = EditorGUILayout.TextField("Sound", currentFrame.soundName);

        EditorGUILayout.EndVertical();
    }

    private void DisplayMovementTab()
    {
        EditorGUILayout.BeginVertical("box");

        currentFrame.forceMovement = EditorGUILayout.Vector2Field("Force Movement", currentFrame.forceMovement);
        currentFrame.addMovment = EditorGUILayout.Vector2Field("Add Movement", currentFrame.addMovment);

        currentFrame.resetVelocity = EditorGUILayout.Toggle("Reset Veloctiy", currentFrame.resetVelocity);

        EditorGUILayout.EndVertical();
    }


    private Rect DrawSpriteRect(Vector2 position, Sprite sprite, float scale, Texture2D background = null)
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

                x = -size.x / 2 * scale,
                y = -size.y / 2 * scale
            };

            posRect.x = position.x;
            posRect.y = position.y;

            if (background != null)
            {
                GUI.DrawTexture(posRect, background);
            }

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
