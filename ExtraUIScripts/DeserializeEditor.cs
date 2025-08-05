using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Deserialize))]
public class DeserializeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Deserialize s = (Deserialize)target;
        if (GUILayout.Button("Play", GUILayout.Width(70)))
        {
            s.Play();
        }
        if (GUILayout.Button("Pause", GUILayout.Width(70)))
        {
            s.Pause();
        }
        if (GUILayout.Button("Next", GUILayout.Width(70)))
        {
            s.NextFrame();
        }
        if (GUILayout.Button("Last", GUILayout.Width(70)))
        {
            s.LastFrame();
        }
    }
}
