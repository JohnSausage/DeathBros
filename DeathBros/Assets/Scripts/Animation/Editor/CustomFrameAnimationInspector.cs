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
    bool playing = false;
    bool showHurtboxes = false;
    int currentDisplay = 0;

    Vector2 zero = new Vector2(150, 200);
    float scale = 4f;

    int pixelPerUnit = 16;

    void OnEnable() { EditorApplication.update += Update; }
    void OnDisable() { EditorApplication.update -= Update; }

    void Update()
    {
        Frame currentFrame = anim.frames[frameCounter];

        if (playing)
        {
            timer += Time.unscaledDeltaTime * 6 / 10; //Editor Updates Faster
            float duration = (currentFrame.duration);


            if (timer > duration / 60)
            {
                frameCounter++;
                timer = 0;
            }


            if (frameCounter >= anim.frames.Count)
                frameCounter = 0;
        }
    }

    public override void OnInspectorGUI()
    {
        anim = (FrameAnimation)target;
        anim.animationName = EditorGUILayout.TextField("Name:", anim.animationName);


        //mouse test
        Event e = Event.current;
        //Debug.Log(e.mousePosition - zero);


        EditorGUILayout.BeginVertical("box");
        {
            //Stop/Play Buttons
            if (playing)
            {
                if (GUILayout.Button("Stop"))
                    playing = false;
            }
            else
            {
                if (GUILayout.Button("Play"))
                    playing = true;
            }

        }
        showHurtboxes = EditorGUILayout.Toggle("Hurtboxes", showHurtboxes);

        EditorGUILayout.EndVertical();

        //Preview the Animation
        if (anim.frames.Count > 0)
        {
            Frame currentFrame = anim.frames[frameCounter];

            PreviewFrame(Vector2.zero, currentFrame);
        }

        EditorGUILayout.BeginHorizontal();
        {
            for (int i = 0; i < anim.frames.Count; i++)
            {
                if (GUILayout.Button((i + 1).ToString()))
                {
                    playing = false;

                    frameCounter = i;
                    currentDisplay = i;
                }
            }

            if (GUILayout.Button("+"))
            {
                anim.frames.Add(new Frame());
            }
        }
        EditorGUILayout.EndHorizontal();


        DisplayFrame(anim.frames[currentDisplay]);

        if (GUILayout.Button("Delete Frame"))
        {
            anim.frames.Remove(anim.frames[currentDisplay]);
        }

        /*
        //Display the Frames
        for (int i = 0; i < anim.frames.Count; i++)
        {
            DisplayFrame(anim.frames[i]);

            if (GUILayout.Button("Delete Frame"))
            {
                anim.frames.Remove(anim.frames[i]);
                break;
            }
        }

        if (GUILayout.Button("Add Frame"))
        {
            anim.frames.Add(new Frame());
        }
        */
        EditorGUILayout.Space();
        EditorUtility.SetDirty(target);

        base.OnInspectorGUI();
    }

    private void PreviewFrame(Vector2 position, Frame frame)
    {
        if (frame != null && frame.sprite != null)
        {
            Repaint();
            EditorGUILayout.BeginVertical();
            {
                DrawTexturePreview(position, Vector2.zero, frame.sprite, scale);

                if (showHurtboxes)
                {
                    foreach (Hurtbox h in frame.hurtBoxes)
                        DrawTexturePreview(position, h.position, Resources.Load<Sprite>("hurtbox"), 2 * h.radius * scale);
                }

                GUILayout.Space(200);
            }
            EditorGUILayout.EndVertical();
        }
    }

    //Displaying a single Frame
    private void DisplayFrame(Frame frame)
    {
        EditorGUILayout.BeginVertical("box");
        {
            frame.duration = EditorGUILayout.IntField("Duration:", frame.duration);

            frame.sprite = (Sprite)EditorGUILayout.ObjectField("Sprite:", frame.sprite, typeof(Sprite), false);

            if (frame.hurtBoxes == null) frame.hurtBoxes = new List<Hurtbox>();

            for (int i = 0; i < frame.hurtBoxes.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");
                {
                    frame.hurtBoxes[i].position = EditorGUILayout.Vector2Field("Position", frame.hurtBoxes[i].position);
                    frame.hurtBoxes[i].radius = EditorGUILayout.FloatField("Radius", frame.hurtBoxes[i].radius);

                    //DrawTexturePreview(CircleToRect(new Vector2(80, 80) + frame.hurtBoxes[i].position * 100, frame.hurtBoxes[i].radius * 25), Resources.Load<Sprite>("hurtbox"));

                    if (GUILayout.Button("Delete"))
                    {
                        frame.hurtBoxes.Remove(frame.hurtBoxes[i]);
                        break;
                    }

                }
                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("Add HurtBox"))
            {
                frame.hurtBoxes.Add(new Hurtbox());
            }
        }
        EditorGUILayout.EndVertical();
        //----------------------------------
        //Debug.Log(GUILayoutUtility.GetLastRect());

        //PreviewFrame(GUILayoutUtility.GetLastRect().position, frame);
    }

    private void DrawTexturePreview(Vector2 position, Vector2 offset, Sprite sprite, float scale)
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

        posRect.x += offset.x * pixelPerUnit * scale;
        posRect.y += offset.y * pixelPerUnit * scale;

        GUI.DrawTextureWithTexCoords(posRect, sprite.texture, coords);
    }

    /*
private Rect CircleToRect(Vector2 position, float radius)
{
    Rect returnValue = new Rect();

    returnValue.position = new Vector2(position.x - radius, position.y - radius);
    returnValue.size = new Vector2(radius * 2, radius * 2);

    return returnValue;
}

//Found somewhere
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
*/
}
