using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{
    private Image myImage;

    public static FadeToBlack instance;

    public float fadeDuration = 1f;

    private bool fadeActive = false;

    private Color baseImageColor;

    float imageAlpha = 0;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        myImage = GetComponent<Image>();
        baseImageColor = myImage.color;
        //Debug.Log(myImage.gameObject.name);
    }

    // Gets called by death methods to start the UI fader process
    public void StartFadeToBlack()
    {
        if (!fadeActive)
        {
            StartCoroutine(FadeImage());
            fadeActive = true;
        }
    }

    // Fades the UI Image in... then out...
    public IEnumerator FadeImage()
    {
        yield return new WaitForSeconds(1f);

        float elapsedTime = fadeDuration;

        while (elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime;
            
            imageAlpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);

            Color newColor = new Color(baseImageColor.r, baseImageColor.g, baseImageColor.b, imageAlpha);
           
            myImage.color = newColor;

           // Debug.Log(myImage.color.a);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        ////Reverse the Fade
        elapsedTime = fadeDuration;

        while (elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime;

            imageAlpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);

            Color newColor = new Color(baseImageColor.r, baseImageColor.g, baseImageColor.b, imageAlpha);

            myImage.color = newColor;

            yield return null;
        }

        fadeActive = false;
        yield break;
    }
}
