using UnityEngine;
using UnityEditor;
using System;

public class FrameAnimatorEditor : EditorWindow
{
    private FrameAnimator anim;
    private FrameAnimation currentAnimation;

    private Vector2 zero = Vector2.zero;
    private float pixelPerUnit = 16;

    private void OnGUI()
    {
            anim = Selection.activeGameObject.GetComponent<FrameAnimator>();


        if (anim == null)
        {
            GUILayout.Label("No Frame Animator selected");
        }
        else
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Label("Found animations:");

                    ShowFrameAnimationList();
                }
                GUILayout.EndVertical();
            }

            {
                var rect = GUILayoutUtility.GetRect(100, 100, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);

                if(currentAnimation != null)
                {
                    DrawTexturePreview(new Vector2(rect.x + 10, rect.y +10), currentAnimation.frames[0].sprite, 4);
                }
            }
            GUILayout.EndHorizontal();
        }
    }

    private void ShowFrameAnimationList()
    {
        foreach (FrameAnimation animation in anim.animations)
        {
            if(GUILayout.Button(animation.animationName))
            {
                currentAnimation = animation;
            }
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
