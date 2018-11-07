using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StatsSO))]
public class StatsSOInspector : Editor
{
    private StatsSO statsSO;

    public override void OnInspectorGUI()
    {
        statsSO = (StatsSO)target;

        for (int i = 0; i < statsSO.stats.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();



            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(50));

            if (GUILayout.Button("Up", GUILayout.MaxWidth(50), GUILayout.MaxHeight(20)))
            {
                if (i != 0)
                {
                    Stat temp = statsSO.stats[i];

                    statsSO.stats[i] = statsSO.stats[i - 1];
                    statsSO.stats[i - 1] = temp;
                }
            }

            if (GUILayout.Button("Down", GUILayout.MaxWidth(50), GUILayout.MaxHeight(20)))
            {
                if (i != statsSO.stats.Count - 1)
                {
                    Stat temp = statsSO.stats[i];

                    statsSO.stats[i] = statsSO.stats[i + 1];
                    statsSO.stats[i + 1] = temp;
                }
            }

            EditorGUILayout.EndVertical();




            EditorGUILayout.BeginVertical("box");

            statsSO.stats[i].statName = EditorGUILayout.TextField("Stat Name", statsSO.stats[i].statName);
            statsSO.stats[i].baseValue = EditorGUILayout.FloatField("Base Value", statsSO.stats[i].baseValue);

            EditorGUILayout.EndVertical();



            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(40));

            if (GUILayout.Button("X", GUILayout.MaxWidth(40), GUILayout.MaxHeight(40)))
            {
                statsSO.stats.Remove(statsSO.stats[i]);
            }

            EditorGUILayout.EndVertical();


            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add New Stat")) statsSO.stats.Add(new Stat());

        EditorUtility.SetDirty(target);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();


        base.OnInspectorGUI();
    }
}
