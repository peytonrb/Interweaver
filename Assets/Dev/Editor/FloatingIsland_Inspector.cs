using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FloatingIslandScript))]
public class FloatingIsland_Inspector : Editor
{
    public override void OnInspectorGUI()
    {
        FloatingIslandScript myFloatingIslandScript = (FloatingIslandScript)target;

        GUILayout.Label("Spawning Required Objects:", EditorStyles.boldLabel);

        if (GUILayout.Button("Spawn Transforms"))
        {
            myFloatingIslandScript.SpawnTransforms();
        }

        if (GUILayout.Button("Spawn Crystal"))
        {
            myFloatingIslandScript.SpawnCrystal();
        }

        if (GUILayout.Button("Spawn Camera"))
        {
            myFloatingIslandScript.SpawnCamera();
        }

        DrawDefaultInspector();

        

        EditorGUILayout.HelpBox("Assign the following: sit and float transforms, isFloating bool, and myFloatCamera VCAM", MessageType.Info);
    }
}
