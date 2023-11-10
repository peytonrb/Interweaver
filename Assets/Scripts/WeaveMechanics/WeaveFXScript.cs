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
        postProcessingVolume.profile = defaultProfile;
    }

    public void DisableWeave()
    {
        weaveRenderer.gameObject.SetActive(false);
    }

    public void ActivateWeave()
    {
        weaveActivation.Play();
    }

    public void WeaveableSelected(GameObject weaveable)
    {
        if (weaveable.gameObject.tag != "FloatingIsland")
        {
            // particles upon select
            var psShape = objectSelectPS.shape; // i hate var but unity requires it to be like this. idk man.
            psShape.radius = weaveable.GetComponent<BoxCollider>().bounds.size.x; // will break if object doesn't have box collider. will fix when issue arises
            Instantiate(objectSelectPS, weaveable.transform.position, Quaternion.Euler(-90f, 0f, 0f));

            // aura effect
            weaveable.GetComponent<Renderer>().material = emissiveMat;
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
        postProcessingVolume.profile = weavingProfile;

        if (weaveable.gameObject.tag != "FloatingIsland")
        {
            weaveable.GetComponent<Renderer>().material = weaveable.GetComponent<WeaveableNew>().originalMat;

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
