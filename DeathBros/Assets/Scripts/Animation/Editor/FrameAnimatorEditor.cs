using UnityEngine;
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

    private Hurtbox settingHurtbox;//, copyHurtbox;
    private FrameAnimation addFrameAnimation;

    private List<Hurtbox> copyHurtboxes = new List<Hurtbox>();
    //private int copyFromFrameNr;

    private bool animationIsPlaying;
    private Vector2 animationListScrollVector;
    private Vector2 generalFrameInfoScrollVector;
    private int specialInfoTabNr = 0;
    private float timer = 0;

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

    private Vector2 hurtboxSettingPosition;

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
        specialFrameInfoTexture.SetPixel(0, 0, Color.black);
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
        previewAnimationRect.height = 200;

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

        GUILayout.Label("Found animations:");

        animationListScrollVector = EditorGUILayout.BeginScrollView(animationListScrollVector, "box");

        for (int i = 0; i < animSO.frameAnimations.Count; i++)
        {
            if (i == currentAnimationNr)
            {
                GUI.color = Color.grey;
            }

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

            GUI.color = Color.white;
        }

        if (GUILayout.Button("Add New (wip)"))
        {

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
            previewRect = DrawSpriteRect(position, currentFrame.sprite, 4, blackTexture);
        }

        GUILayout.Space(previewRect.height);


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

        foreach (Sprite s in addSpritesAsFrames)
        {
            Frame spriteFrame = new Frame();
            spriteFrame.sprite = s;
            currentAnimation.frames.Add(spriteFrame);
        }

        EditorGUILayout.Space();

        addSpritesAsFrames.Clear();

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

                previewRect = DrawSpriteRect(previewPosition, currentFrame.sprite, scale, blackTexture);

                GUILayout.Space(previewRect.height + 10);

                if (settingHurtbox != null)
                {
                    Repaint();

                    hurtboxSettingPosition = Event.current.mousePosition - previewPosition;

                    if (previewRect.Contains(Event.current.mousePosition))
                    {
                        settingHurtbox.position.x = ((hurtboxSettingPosition.x - previewRect.width / 2) / pixelPerUnit / scale);
                        settingHurtbox.position.y = -((hurtboxSettingPosition.y - previewRect.height / 2) / pixelPerUnit / scale);

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


                bool showHurtboxes = true;

                Rect hurtboxRect = new Rect();

                if (showHurtboxes)
                {
                    foreach (Hurtbox h in currentFrame.hurtBoxes)
                    {
                        hurtboxRect.size = new Vector2(h.radius * 2, h.radius * 2) * scale * pixelPerUnit;
                        hurtboxRect.center = previewRect.center + new Vector2(h.position.x, -h.position.y) * scale * pixelPerUnit;

                        GUI.DrawTexture(hurtboxRect, hurtboxTexture);

                        //Check if hurtbox is clicked
                        if (hurtboxRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                        {
                            settingHurtbox = h;
                        }

                    }
                }

                currentFrame.sprite = (Sprite)EditorGUILayout.ObjectField("Change Sprite:", currentFrame.sprite, typeof(Sprite), false);

                currentFrame.duration = EditorGUILayout.IntField("Duration:", currentFrame.duration);
                if (editAllFrames)
                {
                    foreach (Frame f in currentAnimation.frames)
                    {
                        f.duration = currentFrame.duration;
                    }
                }
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
        if (GUILayout.Button("Projectiles"))
        {
            specialInfoTabNr = 2;
        }
        GUI.color = Color.white;

        EditorGUILayout.EndHorizontal();


        if (currentFrame != null)
        {
            if (specialInfoTabNr == 0)
            {
                DisplayHurtboxTab();

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
                        copyHurtboxes.Clear();
                        copyHurtboxes.Add(currentFrame.hurtBoxes[i].Clone());
                    }

                    if (GUILayout.Button("Paste"))
                    {
                        if (copyHurtboxes.Count > 0)
                            currentFrame.hurtBoxes[i] = copyHurtboxes[0].Clone();
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

        if (GUILayout.Button("Copy All"))
        {
            copyHurtboxes.Clear();

            for (int i = 0; i < currentFrame.hurtBoxes.Count; i++)
            {
                copyHurtboxes.Add(currentFrame.hurtBoxes[i].Clone());
            }
        }

        if (GUILayout.Button("Paste All"))
        {

            for (int i = 0; i < copyHurtboxes.Count; i++)
            {
                currentFrame.hurtBoxes.Add(copyHurtboxes[i].Clone());
            }
        }
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
