// 
// Copyright (c) 2023 Off The Beaten Track UG
// All rights reserved.
// 
// Maintainer: Jens Bahr
// 

using System;
using UnityEngine;
using UnityEditor;

namespace Sparrow.VolumetricLight.Editor
{
    public static class EditorUtils
    {
        private const int SpaceHeight = 6;
        private const int LineHeight = 2;
        private const string LogoDark = "sparrow_logos/volumetric_light_dark";
        private const string LogoLight = "sparrow_logos/volumetric_light_logo";

        public static void Separator(int height = LineHeight)
        {
            Space();
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            rect.height = height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
            Space();
        }

        public static void Space(int height = SpaceHeight)
        {
            EditorGUILayout.Space(height);
        }

        public static void DrawLogoHeader(string title, bool small = false)
        {
            DrawLogoHeader(small);
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.fontSize = (int)(style.fontSize * (small ? 1.5f : 2f));
            style.fontStyle = FontStyle.Bold;
            GUI.Label(small
                ? new Rect(45, 20, EditorGUILayout.GetControlRect().width - 70, 25)
                : new Rect(100, 40, EditorGUILayout.GetControlRect().width - 70, 25), title, style);
        }

        public static void DrawLogoHeader(string title, string wikiURL, bool small = false)
        {
            DrawLogoHeader(title, small);
            DrawWikiLinkButton(wikiURL);
        }

        public static void DrawWikiLinkButton(string url)
        {
            if (GUI.Button(new Rect(EditorGUILayout.GetControlRect().width - 30, 10, 30, 25), "?"))
            {
                Application.OpenURL(url);
            }
        }

        public static void DrawLogoHeader(bool small = false)
        {
            var logo = Resources.Load<Texture>(EditorGUIUtility.isProSkin ? LogoLight : LogoDark);
            if (logo != null)
            {
                GUI.DrawTexture(small ? new Rect(15, 15, 423f * (30f / 516f), 30) : new Rect(10, 10, 423f * (60f / 516f), 60), logo);
                GUILayout.Space(small ? 30 : 80);
            }
            else
            {
                GUILayout.Label("Sparrow", EditorStyles.boldLabel);
            }
        }

        public static Texture2D TextureFromColor(Texture2D texture, Color color)
        {
            Color[] colors = texture.GetPixels();

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = colors[i].a > 0 ? color : colors[i];
            }

            texture.SetPixels(colors);
            texture.Apply();

            return texture;
        }

        public static Texture2D TextureFromColor(Color color)
        {
            Color[] colors = new Color[1];
            colors[0] = color;

            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixels(colors);
            tex.Apply();
            return tex;
        }

        public static GameObject FindInChildren(Transform parent, string name, string parentName, bool strict = false)
        {
            foreach (Transform child in parent)
            {
                if (strict
                    ? (child.name == (name) && parent.name == (parentName))
                    : (child.name.ToLower().Contains(name.ToLower()) &&
                       parent.name.ToLower().Contains(parentName.ToLower())))
                {
                    return child.gameObject;
                }

                GameObject childFound = FindInChildren(child, name, parentName, strict);
                if (childFound != null)
                    return childFound;
            }

            return null;
        }

        public static bool ToggleFoldout(this SerializedObject serializedObject, string propertyName, string caption)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);

            if (property.propertyType != SerializedPropertyType.Boolean)
                throw new ArgumentException("Serialized Property of Type Boolean expected.");

            GUIStyle pushButton = new GUIStyle(EditorStyles.miniButtonRight);
            if (property.boolValue)
            {
                pushButton.fontStyle = FontStyle.Bold;
            }

            DrawPropertyField(serializedObject, propertyName, caption);

            return property.boolValue;
        }

        public static void DrawPropertyField(this SerializedObject serializedObject, string propertyName)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            EditorGUILayout.PropertyField(property);
        }

        public static void DrawPropertyField(this SerializedObject serializedObject, string propertyName, string label)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            EditorGUILayout.PropertyField(property, new GUIContent(label));
        }
    }
}