// 
// Copyright (c) 2023 Off The Beaten Track UG
// All rights reserved.
// 
// Maintainer: Jens Bahr
// 

#if !SPARROW_LIGHTS
#define SPARROW_LIGHTS
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using Sparrow.VolumetricLight.Helpers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Sparrow.VolumetricLight
{
    /*
     * Handles mesh creation and material modification
     */
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class LightVolume : MonoBehaviour
    {
        [SerializeField] public VolumetricLightProfile m_Profile;
        [SerializeField, HideInInspector] public Material m_DefaultMaterialAlpha = default;
        [SerializeField, HideInInspector] public Material m_DefaultMaterialAdditive = default;

        private static readonly int LightColor = Shader.PropertyToID("_Color");
        private static readonly int FogTexture = Shader.PropertyToID("_FogTexture");
        private static readonly int FadeTexture = Shader.PropertyToID("_FadeTexture");
        private static readonly int DepthFade = Shader.PropertyToID("_DepthFadeDistance");
        private static readonly int CameraFade = Shader.PropertyToID("_CameraFadeDistance");
        private static readonly int FresnelFade = Shader.PropertyToID("_FresnelFadeStrength");
        private static readonly int FogScrollSpeed = Shader.PropertyToID("_FogScrollSpeed");
        private static readonly int FogOpacity = Shader.PropertyToID("_FogOpacity");

        [HideInInspector, SerializeField] public VolumetricLight volumetricLight;
        MaterialPropertyBlock m_MaterialPropertyBlock;
        MaterialPropertyBlock propertyBlock => m_MaterialPropertyBlock ?? (m_MaterialPropertyBlock = new MaterialPropertyBlock());
        MeshRenderer m_Renderer;
        static readonly int FogTextureTiling = Shader.PropertyToID("_FogTextureTiling");
        MeshRenderer meshRenderer => m_Renderer ? m_Renderer : (m_Renderer = GetComponent<MeshRenderer>());

#if UNITY_EDITOR
        LightType m_CurrentMeshType = LightType.Directional;
        float m_CurrentRange = 0f;
        float m_CurrentAngle = 0f;
        Vector2 m_CurrentAreaSize = Vector2.zero;
#endif

        private void Awake() {
            if (volumetricLight == null) {
                if(gameObject.transform.parent != null)
                    volumetricLight = gameObject.transform.parent.GetComponent<VolumetricLight>();
                
                if (volumetricLight == null) 
                    volumetricLight = gameObject.GetComponentInChildren<VolumetricLight>();
            }
        }

        private void Start() => SendValuesToMaterial();

        public void Setup(VolumetricLight vol, VolumetricLightProfile prof)
        {
            m_Renderer = GetComponent<MeshRenderer>();

            volumetricLight = vol;
            m_Profile = prof;

            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            meshRenderer.lightProbeUsage = LightProbeUsage.Off;
            meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;

            UpdateMaterialType();
        }
           
        public void UpdateMaterialType()
        {
#if UNITY_EDITOR
            if (!m_Profile) return;
            if(m_DefaultMaterialAdditive == null || m_DefaultMaterialAlpha == null)
            {
                var guids = AssetDatabase.FindAssets("t:Material");

                var materials = new List<Material>();
                foreach (var g in guids)
                {
                    var m = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GUIDToAssetPath(g)).OfType<Material>();
                    foreach(Material mat in m)
                    {
                        if (mat.name.Equals("VolumetricLight_Additive"))
                            m_DefaultMaterialAdditive = mat;
                        if (mat.name.Equals("VolumetricLight_Alpha"))
                            m_DefaultMaterialAlpha = mat;
                    }
                }
            }
#endif
            if (m_DefaultMaterialAdditive != null && m_DefaultMaterialAlpha != null)
            {
                meshRenderer.material = null;
                meshRenderer.sharedMaterial = m_Profile.blendingMode == VolumetricLight.BlendingMode.Alpha
                            ? m_DefaultMaterialAlpha
                            : m_DefaultMaterialAdditive;
            }
        }

        public void SendValuesToMaterial()
        {
#if UNITY_EDITOR
            UpdateMaterialType();
#endif
            if (m_Profile == null) return;
            meshRenderer.GetPropertyBlock(propertyBlock);
            if (propertyBlock == null)
            {
                Debug.LogError("No Mesh PropertyBlock could be loaded!");
                return;
            }

            propertyBlock.Clear();

            propertyBlock.SetColor(LightColor, volumetricLight == null ? Color.white : volumetricLight.GetColor());
            if (m_Profile.useFogTexture && m_Profile.fogTexture != null)
            {
                propertyBlock.SetTexture(FogTexture, m_Profile.fogTexture);
                propertyBlock.SetFloat(FogTextureTiling, m_Profile.fogTextureTiling);
                propertyBlock.SetFloat(FogOpacity, m_Profile.fogOpacity);
                propertyBlock.SetVector(FogScrollSpeed, m_Profile.FogScrollingSpeed);
            }

            if (m_Profile.overrideFading)
            {
                propertyBlock.SetFloat(DepthFade, m_Profile.depthFadeDistance);
                propertyBlock.SetFloat(CameraFade, m_Profile.cameraFadeDistance);
                propertyBlock.SetFloat(FresnelFade, m_Profile.fresnelFadeStrength);
            }

            if (m_Profile.useCustomFade && m_Profile.FadeTexture != null)
            {
                propertyBlock.SetTexture(FadeTexture, m_Profile.FadeTexture);
            }

            meshRenderer.SetPropertyBlock(propertyBlock);
        }

#if UNITY_EDITOR
        public void RemoveCurrentMesh()
        {
            // destroy old mesh before creating a new one
            if (GetComponent<MeshFilter>().sharedMesh != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(GetComponent<MeshFilter>().sharedMesh);
                }
                else
                {
                    DestroyImmediate(GetComponent<MeshFilter>().sharedMesh);
                }

                GetComponent<MeshFilter>().sharedMesh = null;
            }
        }
#endif

        public void RecalculateMesh(bool forceReset = false)
        {
#if UNITY_EDITOR

            Light lightShape = transform.parent.GetComponent<Light>();// profile.light;
            float angle = volumetricLight.overrideSpotAngle
                            ? volumetricLight.newSpotAngle / 2f
                            : Mathf.Lerp(lightShape.innerSpotAngle, lightShape.spotAngle, volumetricLight.spotAngleWeight) /
                              2f;

            // first, check if we need to recalculate the mesh!
            if (m_CurrentMeshType == lightShape.type && GetComponent<MeshFilter>().sharedMesh != null)
            {
                switch (lightShape.type)
                {
                    case LightType.Spot:
                        if (Mathf.Approximately(m_CurrentRange, lightShape.range) && Mathf.Approximately(m_CurrentAngle, angle))
                            return;
                        break;
                    case LightType.Point:
                        if (Mathf.Approximately(m_CurrentRange, lightShape.range))
                            return;
                        break;
                    case LightType.Rectangle:
                    case LightType.Disc:
                        if (Mathf.Approximately(m_CurrentRange, lightShape.range)
                            && Mathf.Approximately(m_CurrentAreaSize.x, lightShape.areaSize.x)
                            && Mathf.Approximately(m_CurrentAreaSize.y, lightShape.areaSize.y))
                            return;
                        break;
                }
            }
            RemoveCurrentMesh();

            Mesh mesh = new Mesh();
            switch (lightShape.type)
            {
                case LightType.Spot:
                    mesh = MeshBuilder.HollowCone(lightShape.range, angle);
                    m_CurrentAngle = angle;
                    break;
                case LightType.Point:
                    mesh = MeshBuilder.Sphere(lightShape.range, 32);
                    break;
                case LightType.Disc:
                    mesh = MeshBuilder.HollowCylinder(lightShape.range, lightShape.areaSize.x, lightShape.areaSize.x,
                        32, 2);
                    m_CurrentAreaSize = lightShape.areaSize;
                    break;
                case LightType.Rectangle:
                    Vector2 size = lightShape.areaSize;
                    m_CurrentAreaSize = size;
                    mesh = MeshBuilder.Box(lightShape.range, size.x, size.y);
                    break;
                case LightType.Directional:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (mesh == null)
            {
                Debug.LogWarning("Light Type can not be used to create Volumetric Lights!");
                return;
            }

            mesh.RecalculateBounds();
            GetComponent<MeshFilter>().sharedMesh = mesh;
            m_CurrentRange = lightShape.range;
            m_CurrentMeshType = lightShape.type;
#endif
        }
    }
}