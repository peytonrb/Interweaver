using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HawkFollowScript : MonoBehaviour
{
    [SerializeField] private Transform[] routes;

    private int routeToGo;
    private float tParam;
    private Vector3 objectPosition;

    [SerializeField] private float speedModifier = 0.1f;
    private bool coroutineAllowed;

    private bool glide = false;

    private Animator anim;

    public void Start()
    {
        routeToGo = 0;
        tParam = 0f;
        coroutineAllowed = true;
        
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (coroutineAllowed)
        {
            StartCoroutine(GoByTheRoute(routeToGo));
        }

        if(Vector3.SignedAngle(Vector3.up, transform.forward, transform.right) >= 90f)
        {
            anim.SetBool("Glide", true);
        }
        
        else
        {
            anim.SetBool("Glide", false);
        }
    }

    private IEnumerator GoByTheRoute(int routeNum)
    {
        coroutineAllowed = false;
        Vector3 p0 = routes[routeNum].GetChild(0).position;
        Vector3 p1 = routes[routeNum].GetChild(1).position;
        Vector3 p2 = routes[routeNum].GetChild(2).position;
        Vector3 p3 = routes[routeNum].GetChild(3).position;

        while (tParam < 1)
        {

            tParam += Time.deltaTime * speedModifier;
            objectPosition = Mathf.Pow(1 - tParam, 3) * p0 + 3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 + 3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 + Mathf.Pow(tParam, 3) * p3;
            transform.LookAt(objectPosition);
            transform.position = objectPosition;
            Debug.Log("called!");
            yield return new WaitForFixedUpdate();
        }

        tParam = 0;
        routeToGo += 1;

        if (routeToGo > routes.Length - 1)
        {
            routeToGo = 0;
        }

        coroutineAllowed = true;
    }
}
