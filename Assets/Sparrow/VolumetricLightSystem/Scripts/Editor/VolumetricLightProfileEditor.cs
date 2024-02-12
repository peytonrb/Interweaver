// 
// Copyright (c) 2023 Off The Beaten Track UG
// All rights reserved.
// 
// Maintainer: Jens Bahr
// 

using UnityEditor;

namespace Sparrow.VolumetricLight.Editor
{
    /*
     * formatting volumetric light profiles
     */
    [CustomEditor(typeof(VolumetricLightProfile))]
    public class VolumetricLightProfileEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            VolumetricLightProfile profile = target as VolumetricLightProfile;
            serializedObject.Update();

            serializedObject.DrawPropertyField("blendingMode");
            if (profile != null && profile.blendingMode == VolumetricLight.BlendingMode.Alpha)
            {
                EditorGUI.indentLevel++;
                serializedObject.DrawPropertyField("alpha");
                EditorGUI.indentLevel--;
            }

            EditorUtils.Separator();

            bool overrideColor = serializedObject.ToggleFoldout("overrideLightColor", "Override Light Color");
            EditorUtils.Space();
            EditorGUI.indentLevel++;

            if (overrideColor)
            {
                serializedObject.DrawPropertyField("newColor", "Color");
                serializedObject.DrawPropertyField("newIntensity", "Intensity");
            }
            else
            {
                serializedObject.DrawPropertyField("intensityMultiplier");
            }

            EditorGUI.indentLevel--;


            EditorUtils.Separator();

            if (serializedObject.ToggleFoldout("overrideFading", "Override Fade Distances"))
            {
                EditorUtils.Space();
                EditorGUI.indentLevel++;

                serializedObject.DrawPropertyField("depthFadeDistance", "Geometry Distance");
                serializedObject.DrawPropertyField("cameraFadeDistance", "Camera Distance");
                serializedObject.DrawPropertyField("fresnelFadeStrength", "Edge Fade Strength");

                EditorGUI.indentLevel--;
            }

            EditorUtils.Separator();

            if (serializedObject.ToggleFoldout("useCustomFade", "Custom Fading"))
            {
                EditorGUI.indentLevel++;
                serializedObject.DrawPropertyField("fadeGradient");
                EditorGUI.indentLevel--;
            }

            EditorUtils.Separator();


            if (serializedObject.ToggleFoldout("useFogTexture", "Use Fog Texture"))
            {
                EditorGUI.indentLevel++;
                serializedObject.DrawPropertyField("fogTexture");
                serializedObject.DrawPropertyField("fogTextureTiling");
                serializedObject.DrawPropertyField("fogOpacity");
                serializedObject.DrawPropertyField("verticalScrollingSpeed");
                serializedObject.DrawPropertyField("horizontalScrollingSpeed");
                EditorGUI.indentLevel--;
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
}