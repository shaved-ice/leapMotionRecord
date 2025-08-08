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
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("First", GUILayout.Width(70)))
        {
            s.FirstFrame();
        }
        if (GUILayout.Button("<<Prev", GUILayout.Width(70)))
        {
            s.PrevFrameLarge();
        }
        if (GUILayout.Button("Prev", GUILayout.Width(70)))
        {
            s.PrevFrame();
        }
        if (GUILayout.Button("Play/Pause", GUILayout.Width(90)))
        {
            s.PausePlay();
        }
        if (GUILayout.Button("Next", GUILayout.Width(70)))
        {
            s.NextFrame();
        }
        if (GUILayout.Button("Next>>", GUILayout.Width(70)))
        {
            s.NextFrameLarge();
        }
        if (GUILayout.Button("Last", GUILayout.Width(70)))
        {
            s.LastFrame();
        }
        GUILayout.EndHorizontal();
    }
}
