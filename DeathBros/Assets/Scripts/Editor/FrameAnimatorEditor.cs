using UnityEngine;
using UnityEditor;
using System;

public class FrameAnimatorEditor : EditorWindow
{
    private FrameAnimator anim;
    private FrameAnimation currentAnimation;
    private int currentAnimationNr;
    private FrameAnimation addFrameAnimation;
    private Frame currentFrame;
    private int currentFrameNr;

    private Vector2 zero = Vector2.zero;
    private float pixelPerUnit = 16;

    private int frameCounter = 0;
    private float timer = 0;
    private bool playing = false;
    private bool showHurtboxes = false;

    private Vector2 animationListScrollVector;
    private Vector2 frameListSrollVector;

    void OnEnable() { EditorApplication.update += Update; }
    void OnDisable() { EditorApplication.update -= Update; }

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

            x = zero.x - size.x / 2 * scale,
            y = zero.y - size.y / 2 * scale
        };

        /*
        posRect.x += offset.x * pixelPerUnit * scale;
        posRect.y += offset.y * pixelPerUnit * scale;
        */

        posRect.x = offset.x;
        posRect.y = offset.y;

        GUI.DrawTextureWithTexCoords(posRect, sprite.texture, coords);
    }

    [MenuItem("Window/FrameAnimator")]
    public static void ShowWindow()
    {
        GetWindow<FrameAnimatorEditor>("Frame Animator");
    }
}
