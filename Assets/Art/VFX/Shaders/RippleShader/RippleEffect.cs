using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RippleEffect : MonoBehaviour
{
    public int TextureSize = 512;
    public RenderTexture ObjectsRT;
    private RenderTexture CurrRT, PrevRT, TempRT;
    public Shader RippleShader, AddShader;
    public GameObject psFollowPlayer;
    private Material RippleMat, AddMat;

    void Start()
    {
        //Creating render textures and materials
        CurrRT = new RenderTexture(TextureSize, TextureSize, 0, RenderTextureFormat.RFloat);
        PrevRT = new RenderTexture(TextureSize, TextureSize, 0, RenderTextureFormat.RFloat);
        TempRT = new RenderTexture(TextureSize, TextureSize, 0, RenderTextureFormat.RFloat);
        RippleMat = new Material(RippleShader);
        AddMat = new Material(AddShader);
        psFollowPlayer.gameObject.SetActive(false);

        //Change the texture in the material of this object to the render texture calculated by the ripple shader.
        GetComponent<Renderer>().material.SetTexture("_RippleTex", CurrRT);

        StartCoroutine(ripples());
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            psFollowPlayer.gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        { 
            if (collider.GetComponent<MovementScript>().currentSpeed > 0)
            {
                if (!psFollowPlayer.gameObject.activeSelf)
                {
                    psFollowPlayer.gameObject.SetActive(true);
                }

                psFollowPlayer.transform.position = new Vector3(collider.transform.position.x, collider.transform.position.y - 2f, collider.transform.position.z);
            }
            else
            {
                psFollowPlayer.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            psFollowPlayer.gameObject.SetActive(false);
        }
    }

    IEnumerator ripples()
    {
        //Copy the result of blending the render textures to TempRT.
        AddMat.SetTexture("_ObjectsRT", ObjectsRT);
        AddMat.SetTexture("_CurrentRT", CurrRT);
        Graphics.Blit(null, TempRT, AddMat);

        RenderTexture rt0 = TempRT;
        TempRT = CurrRT;
        CurrRT = rt0;

        //Calculate the ripple animation using ripple shader.
        RippleMat.SetTexture("_PrevRT", PrevRT);
        RippleMat.SetTexture("_CurrentRT", CurrRT);
        Graphics.Blit(null, TempRT, RippleMat);
        Graphics.Blit(TempRT, PrevRT);

        //Swap PrevRT and CurrentRT to calculate the result for the next frame.
        RenderTexture rt = PrevRT;
        PrevRT = CurrRT;
        CurrRT = rt;

        //Wait for one frame and then execute again.
        yield return null;
        StartCoroutine(ripples());
    }
}
