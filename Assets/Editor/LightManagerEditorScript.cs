using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LightSourceScript))]
[CanEditMultipleObjects]
public class LightManagerEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LightSourceScript lightSourceScript = (LightSourceScript)target;

        if (GUILayout.Button("This clears the list"))
        {
            lightSourceScript.lightsArray.Clear();
        }

        if (GUILayout.Button("This adds all the lightobject that is a TimedGlowMushroom"))
        {
            lightSourceScript.SearchTimedMushrooms();
        }

        if (GUILayout.Button("This adds all the lightobject that is a LightCrystals"))
        {
            lightSourceScript.SearchLightCrystals();
        }

        if (GUILayout.Button("This adds all the lightobject that is a SpiderLights"))
        {
            lightSourceScript.SearchSpiderLights();
        }
        if (GUILayout.Button("This adds all the other lights that don't have scripts"))
        {
            lightSourceScript.SearchOtherLights();
        }
    }
}
