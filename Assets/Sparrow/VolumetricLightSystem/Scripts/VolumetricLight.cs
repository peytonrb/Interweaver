// 
// Copyright (c) 2023 Off The Beaten Track UG
// All rights reserved.
// 
// Maintainer: Jens Bahr
// 

using UnityEngine;

namespace Sparrow.VolumetricLight
{
    /*
     * Stores light source and profile, handles creation of supporting objects.
     */
    [RequireComponent(typeof(Light)), ExecuteAlways, SelectionBase, AddComponentMenu("Rendering/Volumetric Light")]
    public class VolumetricLight : MonoBehaviour
    {
        public enum BlendingMode
        {
            Alpha,
            Additive
        }

        // SPOTLIGHT SETTINGS
        [Tooltip("Adjust the influence of the spotlight's angle on the light's appearance. Ranges from 0 to 1.")]
        [Range(0f, 1f)] public float spotAngleWeight = 1f;
        [Tooltip("Enable this to override the light's spot angle with a custom value.")]
        public bool overrideSpotAngle;
        [Tooltip("Set a new spot angle for the light when override is enabled. Ranges from 1 to 179 degrees.")]
        [Range(1f, 179f)] public float newSpotAngle = 90;

        [SerializeField] VolumetricLightProfile profile;
        [SerializeField] LightVolume lightVolume;
        [SerializeField] Light lightSource;

        public LightVolume Volume
        {
            get
            {
                // reset light volume if its a child of another, e.g. if light was copied
                if (lightVolume != null && !lightVolume.transform.IsChildOf(transform))
                    lightVolume = null;
                if (lightVolume != null) return lightVolume;
                lightVolume = GetComponentInChildren<LightVolume>();
                if (lightVolume != null) return lightVolume;
                AddLightVolume();
                return lightVolume;
            }
        }

        public Light LightSource
        {
            get
            {
                if (lightSource == null)
                {
                    lightSource = GetComponent<Light>();
                }

                return lightSource;
            }
        }

        public VolumetricLightProfile Profile
        {
            get => profile;
            set => profile = value;
        }

        void Update()
        {
            if (profile == null) return;
#if UNITY_EDITOR
            SetUpVolume(); // calling this to react to changes in the light settings
#endif
            UpdateVolumetricLight();
        }

        public void SetUpVolume()
        {
            if (Profile == null) return;

            Profile.light = LightSource;
            Volume.m_Profile = Profile;
            Volume.RecalculateMesh();
        }
        public Color GetColor()
        {
            if (profile == null) return Color.white;
            Color col = profile.overrideLightColor
                ? profile.newColor * profile.newIntensity
                : lightSource.color * profile.intensityMultiplier * lightSource.intensity;
            col.a = profile.blendingMode == VolumetricLight.BlendingMode.Alpha ? profile.alpha : 1;
            return col;
        }
#if UNITY_EDITOR
        void OnEnable()
        {
            SetUpVolume();
        }
#endif
        void OnDestroy()
        {
#if UNITY_EDITOR
            DestroyImmediate(Volume.gameObject);
#else
            Destroy(Volume.gameObject);
#endif
        }

        public void UpdateVolumetricLight()
        {
            Volume.SendValuesToMaterial();
        }

        public void AddLightVolume()
        {
            // remove all child volumes
            var objs = transform.GetComponentsInChildren<LightVolume>();
            for (int i = objs.Length - 1; i >= 0; i--)
            {
#if UNITY_EDITOR
                DestroyImmediate(objs[i].gameObject);
#else
                Destroy(objs[i].gameObject);
#endif
            }

            // add new one
            GameObject obj = new GameObject("LightVolume");
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.localRotation = Quaternion.identity;

            obj.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;

            LightVolume vol = obj.AddComponent<LightVolume>();

            vol.Setup(this, Profile);

            lightVolume = vol;
        }
    }
}
