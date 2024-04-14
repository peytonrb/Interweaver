using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderCutsceneController : MonoBehaviour
{
    public GameObject spiderAnimStuff;

    public Animator anim;

    public float spiderSpeed = 0;

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("Speed", spiderSpeed * 0.7f);
        //Debug.Log(anim.GetFloat("Speed"));
    }

    public void DisableSpiderStuff()
    {
        //Debug.Log("Why");
        spiderAnimStuff.SetActive(false);
    }    
}
