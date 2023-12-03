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

    void Start()
    {
        routeToGo = 0;
        tParam = 0f;
        coroutineAllowed = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (coroutineAllowed)
        {
            StartCoroutine(GoByTheRoute(routeToGo));
        }

        //if (tParam < 0.25f)
        //{
        //    transform.LookAt(routes[routeToGo].GetChild(0).position);
        //}
        //else if (tParam < 0.5f)
        //{
        //    transform.LookAt(routes[routeToGo].GetChild(1).position);
        //}
        //else if (tParam < 0.75f)
        //{
        //    transform.LookAt(routes[routeToGo].GetChild(2).position);
        //}
        //else
        //{
        //    transform.LookAt(routes[routeToGo].GetChild(3).position);
        //}
        


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
            
            yield return new WaitForEndOfFrame();
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
