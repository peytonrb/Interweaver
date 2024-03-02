using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WeaveFXScript : MonoBehaviour
{
    [CannotBeNullObjectField] public LineRenderer weaveRenderer;
    [CannotBeNullObjectField] public ParticleSystem weaveActivation;
    [CannotBeNullObjectField] public ParticleSystem objectSelectPS;
    [CannotBeNullObjectField] public GameObject particleScattering;
    [CannotBeNullObjectField] public Material emissiveMat;

    [Header("Post Processing")]
    [CannotBeNullObjectField] public GameObject postProcessing;
    private Volume postProcessingVolume;
    [CannotBeNullObjectField] public VolumeProfile defaultProfile;
    [CannotBeNullObjectField] public VolumeProfile weavingProfile;
    [CannotBeNullObjectField] public GameObject weaveEffect;

    public int needleVel = 60;
    void Start()
    {
        postProcessingVolume = postProcessing.GetComponent<Volume>();
        weaveRenderer.gameObject.SetActive(false);
        weaveActivation.Stop();
    }

    public void DrawWeave(Vector3 playerPos, Vector3 weaveablePos)
    {
        weaveRenderer.gameObject.SetActive(true);
        weaveRenderer.positionCount = 2;
        weaveRenderer.SetPosition(0, playerPos);
        weaveRenderer.SetPosition(1, weaveablePos);
        // postProcessingVolume.profile = defaultProfile;
    }

    public void DisableWeave()
    {
        if (weaveRenderer != null)
            weaveRenderer.gameObject.SetActive(false);
    }

    public void ActivateWeave(Transform weaveablePos)
    {
        weaveActivation.Play();
        GameObject spawnedEffect = Instantiate(weaveEffect, transform.position,
                                               Quaternion.LookRotation(Vector3.up, weaveablePos.position - transform.position));
        spawnedEffect.GetComponent<NeedleVFXScript>().SetDestroyTimer(Vector3.Distance(weaveablePos.position, transform.position) / needleVel, weaveablePos.gameObject);
    }

    public void WeaveableSelected(GameObject weaveable)
    {
        if (weaveable.gameObject.tag != "FloatingIsland")
        {
            // particles upon select
            var psShape = objectSelectPS.shape; // i hate var but unity requires it to be like this. idk man.

            //Try get box collider or sphere collider
            if (weaveable.TryGetComponent<BoxCollider>(out BoxCollider box))
            {
                psShape.radius = box.bounds.size.x; // will break if object doesn't have box collider
            }
            else if (weaveable.TryGetComponent<SphereCollider>(out SphereCollider sphere))
            {
                psShape.radius = sphere.bounds.size.x;
            }
            
            Instantiate(objectSelectPS, weaveable.transform.position, Quaternion.Euler(-90f, 0f, 0f));

            if (weaveable.transform.GetChild(0).GetComponent<Renderer>() != null)
            {
                weaveable.transform.GetChild(0).GetComponent<Renderer>().material = emissiveMat;
            }
            else
            {
                weaveable.transform.GetChild(0).GetComponent<Renderer>().material = emissiveMat;
            }

            StartCoroutine(StartAura(weaveable));
        }
    }
    IEnumerator StartAura(GameObject weaveable)
    {
        yield return new WaitForSeconds(0.4f);
        Instantiate(particleScattering, weaveable.transform.position, Quaternion.identity, weaveable.transform);
    }

    public void StopAura(GameObject weaveable)
    {
        // postProcessingVolume.profile = weavingProfile;

        if (weaveable.gameObject.tag != "FloatingIsland")
        {
            weaveable.transform.GetChild(0).GetComponent<Renderer>().material = weaveable.GetComponent<WeaveableObject>().originalMat;

            // kinda inefficient if we end up having hella children per GameObject
            for (int i = 0; i < weaveable.transform.childCount; i++)
            {
                if (weaveable.transform.GetChild(i).name == "WeaveableObjectAura(Clone)")
                {
                    Transform child = weaveable.transform.GetChild(i);
                    Destroy(child.gameObject);
                }
            }
        }
    }
}
