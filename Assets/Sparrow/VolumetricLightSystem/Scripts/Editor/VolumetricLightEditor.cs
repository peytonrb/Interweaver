// 
// Copyright (c) 2023 Off The Beaten Track UG
// All rights reserved.
// 
// Maintainer: Jens Bahr
// 

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Sparrow.VolumetricLight.Editor
{
    /*
     * Exposes settings of volumetric light and its profile
     */
    [CustomEditor(typeof(VolumetricLight)), CanEditMultipleObjects]
    public class VolumetricLightEditor : UnityEditor.Editor
    {
        private void SpotlightProperties()
        {
            EditorGUILayout.LabelField("Spotlight Settings", EditorStyles.boldLabel);

            bool overrideSpot = serializedObject.ToggleFoldout("overrideSpotAngle", "Override Spot Angle");
            EditorUtils.Space();
            EditorGUI.indentLevel++;
            if (overrideSpot)
            {
                SerializedProperty newSpotAngle = serializedObject.FindProperty("newSpotAngle");
                EditorGUILayout.PropertyField(newSpotAngle, new GUIContent("Spot Angle"));
            }
            else
            {
                SerializedProperty spotAngleWeight = serializedObject.FindProperty("spotAngleWeight");
                EditorGUILayout.PropertyField(spotAngleWeight, new GUIContent("Angle Inner/Outer"));
            }

            EditorGUI.indentLevel--;
            EditorUtils.Space();
        }

        public static bool HasShaderGraph(VolumetricLight light)
        {
            foreach (Renderer renderer in light.GetComponentsInChildren<Renderer>())
            {
                if (renderer != null)
                {
                    Material mat = renderer.sharedMaterial;
                    if (mat != null && mat.shader != null)
                    {
                        if (mat.shader.name == "Hidden/InternalErrorShader")
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public override void OnInspectorGUI()
        {
            VolumetricLight volumetricLight = target as VolumetricLight;
            serializedObject.Update();
            if (volumetricLight == null) return;

            EditorUtils.DrawLogoHeader("Volumetric Light", "https://wiki.beatentrack.games/s/585cd38e-10ce-4ec8-ae64-d7d2119065ea", true);

            if(!HasShaderGraph(volumetricLight))
                EditorGUILayout.HelpBox("Shader Graph is not currently installed in this project. Please install it to use this feature.", MessageType.Error);

            switch (volumetricLight.LightSource.type)
            {
                case LightType.Spot:
                    SpotlightProperties();
                    break;
                case LightType.Directional:
                    EditorGUILayout.HelpBox("Volumetric lights can not be used for directional lights.", MessageType.Warning);
                    if (GUILayout.Button("Fix now!")) volumetricLight.LightSource.type = LightType.Spot;
                    EditorUtils.Space();
                    break;
                default:
                    EditorGUILayout.HelpBox($"Change the Light type to see specific properties. Current Light Type: {volumetricLight.LightSource.type}", MessageType.Info);
                    break;
            }

            EditorUtils.Separator();

            using (new EditorGUILayout.HorizontalScope())
            {
                SerializedProperty profileProperty = serializedObject.FindProperty("profile");
                EditorGUILayout.PropertyField(profileProperty);

                if (GUILayout.Button("New"))
                {
                    VolumetricLightProfile profile =
                        VolumetricLightProfile.CreateProfile(volumetricLight.gameObject.scene, volumetricLight.name);

                    profile.name = volumetricLight.gameObject.name + "Profile";
                    profile.light = volumetricLight.LightSource;

                    profileProperty.objectReferenceValue = profile;
                }
            }

            serializedObject.ApplyModifiedProperties();

            if (volumetricLight.Profile == null)
            {
                EditorGUILayout.HelpBox("Assign a profile or create a new one to display all options.", MessageType
                    .Info);
                return;
            }

            EditorUtils.Separator();

            CreateEditor(volumetricLight.Profile).OnInspectorGUI();

            EditorUtils.Separator();

            if (GUILayout.Button("Toggle Volume visibility"))
            {
                if ((volumetricLight.Volume.gameObject.hideFlags & (HideFlags.HideInHierarchy | HideFlags.HideInInspector)) == (HideFlags.HideInHierarchy | HideFlags.HideInInspector))
                {
                    volumetricLight.Volume.gameObject.hideFlags &= ~(HideFlags.HideInHierarchy | HideFlags.HideInInspector);
                }
                else
                {
                    volumetricLight.Volume.gameObject.hideFlags |= HideFlags.HideInHierarchy | HideFlags.HideInInspector;
                }

            }
            if (GUILayout.Button("Recalculate LightVolume"))
            {
                volumetricLight.AddLightVolume();
            }
            

            serializedObject.ApplyModifiedProperties();
        }
    }
}