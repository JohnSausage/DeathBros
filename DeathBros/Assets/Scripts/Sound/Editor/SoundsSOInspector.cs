using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SoundsSO))]
public class SouindsSOInspector : Editor
{
    bool loadNewFilesOnly = true;
    bool showSoundDetails = false;

    public override void OnInspectorGUI()
    {
        SoundsSO sc = (SoundsSO)target;

        EditorGUILayout.BeginVertical("box");

        sc.loadSoundsFromFolder = EditorGUILayout.TextField("Sound Folder", sc.loadSoundsFromFolder);

        loadNewFilesOnly = EditorGUILayout.Toggle("Load New Sounds Only", loadNewFilesOnly);

        if (GUILayout.Button("Load Sound Files From Resources") && sc.loadSoundsFromFolder != "")
        {
            Object[] objects = Resources.LoadAll(sc.loadSoundsFromFolder);

            foreach (var o in objects)
            {
                Sound s = new Sound
                {
                    name = o.name,
                    clip = (AudioClip)o
                };

                if (loadNewFilesOnly)
                {
                    if (!sc.sounds.Contains(sc.sounds.Find(x => x.name == s.name)))
                    {
                        sc.sounds.Add(s);
                    }
                }
                else
                {
                    sc.sounds.Add(s);
                }
            }
        }

        if (GUILayout.Button("Remove All Sounds"))
        {
            sc.sounds.Clear();
        }

        showSoundDetails = EditorGUILayout.Toggle("Show Details", showSoundDetails);

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical("box");

        if (sc.sounds == null)
        {
            sc.sounds = new List<Sound>();
        }

        for (int i = 0; i < sc.sounds.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");

            sc.sounds[i].name = EditorGUILayout.TextField("Name", sc.sounds[i].name);

            if (showSoundDetails)
            {
                sc.sounds[i].loop = EditorGUILayout.Toggle("Loop", sc.sounds[i].loop);
                sc.sounds[i].volume = EditorGUILayout.Slider("Volume", sc.sounds[i].volume, 0f, 1f);
                sc.sounds[i].clip = (AudioClip)EditorGUILayout.ObjectField("Clip", sc.sounds[i].clip, typeof(AudioClip), false);

                if (GUILayout.Button("Remove Sound"))
                {
                    sc.sounds.Remove(sc.sounds[i]);
                }

            }
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndVertical();


        EditorUtility.SetDirty(target);

        GUILayout.Space(100);

        base.OnInspectorGUI();
    }
}
