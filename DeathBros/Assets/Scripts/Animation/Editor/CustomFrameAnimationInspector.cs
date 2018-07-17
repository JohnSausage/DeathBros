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
    Hurtbox settingHurtbox, copyHurtbox;

    Vector2 zero = new Vector2(150, 200);
    float scale = 4f;

    int pixelPerUnit = 16;

    void OnEnable() { EditorApplication.update += Update; }
    void OnDisable() { EditorApplication.update -= Update; }

    private void Reset()
    {
        frameCounter = 0;
        timer = 0;
        playing = false;
        showHurtboxes = false;
        currentDisplay = 0;
    }

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
                if (frameCounter == i)
                {
                    GUI.color = Color.grey;
                }

                if (GUILayout.Button((i + 1).ToString()))
                {
                    playing = false;

                    frameCounter = i;
                    currentDisplay = i;
                }

                GUI.color = Color.white;
            }

            if (GUILayout.Button("+"))
            {
                anim.frames.Add(new Frame());
            }
        }
        EditorGUILayout.EndHorizontal();

        DisplayFrame(anim.frames[currentDisplay]);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUI.color = new Color32(200, 180, 180, 255);
        if (GUILayout.Button("Delete Frame"))
        {
            anim.frames.Remove(anim.frames[currentDisplay]);
            Reset();
        }
        GUI.color = Color.white;

        EditorGUILayout.Space();

        EditorUtility.SetDirty(target);

        base.OnInspectorGUI();
    }

    private void PreviewFrame(Vector2 position, Frame frame)
    {
        if (frame != null && frame.sprite != null)
        {
            Repaint();

            DrawTexturePreview(position, Vector2.zero, frame.sprite, scale);

            if (showHurtboxes)
            {
                foreach (Hurtbox h in frame.hurtBoxes)
                    DrawTexturePreview(position, new Vector2(h.position.x, -h.position.y), Resources.Load<Sprite>("hurtbox"), 2 * h.radius * scale);
            }
        }

        GUILayout.Space(200);
    }

    //Displaying a single Frame
    private void DisplayFrame(Frame frame)
    {
        EditorGUILayout.BeginVertical("box");
        {
            frame.sprite = (Sprite)EditorGUILayout.ObjectField("Sprite:", frame.sprite, typeof(Sprite), false);

            frame.duration = EditorGUILayout.IntField("Duration:", frame.duration);

            if (frame.hurtBoxes == null) frame.hurtBoxes = new List<Hurtbox>();

            for (int i = 0; i < frame.hurtBoxes.Count; i++)
            {
                //GUI.color = Color.green;
                GUI.color = new Color32(180, 200, 180, 255);
                EditorGUILayout.BeginVertical("box");
                {
                    frame.hurtBoxes[i].position = EditorGUILayout.Vector2Field("Position", frame.hurtBoxes[i].position);
                    frame.hurtBoxes[i].radius = EditorGUILayout.FloatField("Radius", frame.hurtBoxes[i].radius);

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Set"))
                        {
                            settingHurtbox = frame.hurtBoxes[i];
                        }

                        if (GUILayout.Button("Copy"))
                        {
                            copyHurtbox = frame.hurtBoxes[i];
                        }

                        if (GUILayout.Button("Paste") && copyHurtbox != null)
                        {
                            frame.hurtBoxes[i] = copyHurtbox.Clone();
                        }

                        if (GUILayout.Button("Delete"))
                        {
                            frame.hurtBoxes.Remove(frame.hurtBoxes[i]);
                            break;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                GUI.color = Color.white;
            }

            if (GUILayout.Button("Add HurtBox"))
            {
                frame.hurtBoxes.Add(new Hurtbox());
            }
        }
        EditorGUILayout.EndVertical();
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
