using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cinemachine.Editor;

[CustomEditor(typeof(LightSourceScript))]
[CanEditMultipleObjects]
public class LightManagerEditorScript : Editor
{
    private string timedGlowMusroom = "TimedGlowMushroomsScript";
    private string lightCrystals = "LightCrystalScript";
    private string staticSpiderLights = "SpiderLightsMechanic";
    private string prefabName = "TorchLight";
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
            SearchTimedMushrooms();
        }

        if (GUILayout.Button("This adds all the lightobject that is a LightCrystals"))
        {
            SearchLightCrystals();
        }

        if (GUILayout.Button("This adds all the lightobject that is a SpiderLights"))
        {
            SearchSpiderLights();
        }
        if (GUILayout.Button("This adds all the other lights that don't have scripts"))
        {
            SearchOtherLights();
        }
    }

    #region// SEARCHES THE LIGHTS THAT HAS SCRIPTS AND THEN ADDS THEM TO THE LIST
    //***********************************************************
    public void SearchLightCrystals()
    {
        MonoBehaviour[] scriptInstances = FindObjectsOfType<MonoBehaviour>();
        LightSourceScript lightSourceScript = (LightSourceScript)target;
        List<LightSourceScript.LightData> lightsArray = lightSourceScript.lightsArray;
        foreach (MonoBehaviour scriptInstance in scriptInstances)
        {
            if (scriptInstance.GetType().Name == lightCrystals)
            {
                Transform lightComponents;
                LightSourceScript.LightData newLightdata = new LightSourceScript.LightData();
                LightCrystalScript lightCrystalScript = scriptInstance.GetComponent<LightCrystalScript>();
                GameObject parentObj = scriptInstance.gameObject;

                if (parentObj.gameObject.transform.GetChild(0).TryGetComponent( out Light light))
                {
                    lightComponents = parentObj.gameObject.transform.GetChild(0);
                }
                else
                {
                    lightComponents = parentObj.gameObject.transform.GetChild(0).transform.GetChild(0);
                }
                

                newLightdata.lightSource = lightComponents.GetComponent<Light>();
                newLightdata.lightCollider = lightComponents.GetComponent<Collider>();
                newLightdata.isOn = true;

                if (newLightdata.lightCollider == null)
                {
                    Transform lightComponentsparent = parentObj.gameObject.transform.GetChild(0).transform.GetChild(1);
                    newLightdata.lightCollider = lightComponentsparent.GetComponent<Collider>();
                }
                lightCrystalScript.arrayIndex = lightsArray.Count;
                EditorUtility.SetDirty(lightCrystalScript);
                lightsArray.Add(newLightdata);
                Debug.Log("Found " + lightsArray.Count + " items with script '" + lightCrystals + "'.");
            }

        }
    }

    public void SearchTimedMushrooms()
    {
        MonoBehaviour[] scriptInstances = FindObjectsOfType<MonoBehaviour>();
        LightSourceScript lightSourceScript = (LightSourceScript)target;
        List<LightSourceScript.LightData> lightsArray = lightSourceScript.lightsArray;
        foreach (MonoBehaviour scriptInstance in scriptInstances)
        {
            if (scriptInstance.GetType().Name == timedGlowMusroom)
            {
                LightSourceScript.LightData newLightdata = new LightSourceScript.LightData();
                TimedGlowMushroomsScript timedGlowMushroomScript = scriptInstance.GetComponent<TimedGlowMushroomsScript>();
                GameObject parentObj = scriptInstance.gameObject;
                Transform lightComponents = parentObj.gameObject.transform.GetChild(0);
                newLightdata.lightSource = lightComponents.GetComponent<Light>();
                newLightdata.lightCollider = lightComponents.GetComponent<Collider>();
                newLightdata.isOn = true;
                timedGlowMushroomScript.arrayIndex = lightsArray.Count;
                EditorUtility.SetDirty(timedGlowMushroomScript);
                lightsArray.Add(newLightdata);
                Debug.Log("Found " + lightsArray.Count + " items with script '" + timedGlowMusroom + "'.");
            }

        }
    }

    public void SearchSpiderLights()
    {
        MonoBehaviour[] scriptInstances = FindObjectsOfType<MonoBehaviour>();
        LightSourceScript lightSourceScript = (LightSourceScript)target;
        List<LightSourceScript.LightData> lightsArray = lightSourceScript.lightsArray;
        foreach (MonoBehaviour scriptInstance in scriptInstances)
        {
            if (scriptInstance.GetType().Name == staticSpiderLights)
            {

                LightSourceScript.LightData newLightdata = new LightSourceScript.LightData();
                SpiderLightsMechanic spiderLightsMechanics = scriptInstance.GetComponent<SpiderLightsMechanic>();
                GameObject parentObj = scriptInstance.gameObject;
                Transform lightComponents = parentObj.gameObject.transform.GetChild(0);
                newLightdata.lightSource = lightComponents.GetComponent<Light>();
                newLightdata.lightCollider = lightComponents.GetComponent<Collider>();
                newLightdata.isOn = true;
                spiderLightsMechanics.arrayIndex = lightsArray.Count;
                EditorUtility.SetDirty(spiderLightsMechanics);
                lightsArray.Add(newLightdata);
                Debug.Log("Found " + lightsArray.Count + " items with script '" + staticSpiderLights + "'.");
            }

        }
    }

    public void SearchOtherLights()
    {
        GameObject[] otherLights = FindObjectsOfType<GameObject>();
        LightSourceScript lightSourceScript = (LightSourceScript)target;
        List<LightSourceScript.LightData> lightsArray = lightSourceScript.lightsArray;
        foreach (GameObject otherLight in otherLights)
        {
            if (otherLight.name == prefabName) 
            {
                LightSourceScript.LightData newLightdata = new LightSourceScript.LightData();
                newLightdata.lightSource = otherLight.GetComponent<Light>();
                newLightdata.lightCollider = otherLight.GetComponent<SphereCollider>();
                newLightdata.isOn = true;
                lightsArray.Add(newLightdata);
                Debug.Log("Found " + lightsArray.Count + " items with script '" + prefabName + "'.");
            }
        }
    }
    //***********************************************************
    #endregion
}
