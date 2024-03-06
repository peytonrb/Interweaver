using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriageOpener : MonoBehaviour
{
    public Animator animA;
    public Animator animB;

    private bool opening = false;
    // Start is called before the first frame update
    void Start()
    {

    }



    public void OpenCarriage()
    {
        if (!opening)
        {
            animA.SetTrigger("Open");
            animB.SetTrigger("Open");

            StartCoroutine(OpenCooldown());
            opening = true;
        }
        
    }

    public IEnumerator OpenCooldown()
    {
        yield return new WaitForSeconds(3f);

        opening = false;

        yield break;
    }
}
