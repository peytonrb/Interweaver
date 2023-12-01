using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class DayblockPuzzleManager : MonoBehaviour
{
    //private bool parentFound;
    public int combinationpart; //Which part of the combination has been done
    private GameObject[] weaveables;
    [HideInInspector] public WeaveableNew sunblockweaveparent;
    [HideInInspector] public WeaveableNew sunriseblockweaveparent;
    [HideInInspector] public WeaveableNew moonblockweaveparent;

    public List<GameObject> setKeyObjects = new List<GameObject>();
    private List<GameObject> fakeObjects = new List<GameObject>();
    public Transform[] failSpitPoint;

    private WeaveableNew weaveableScript;
    private DayblockScript[] dayblockScripts;

    public GameObject vfxObject;

    public GameObject fakeKeyObject;

    public GameObject orbObject;

    public GameObject riseSuccObj;
    public GameObject sunSuccObj;
    public GameObject moonSuccObj;

    public GameObject riseDefObj;
    public GameObject sunDefObj;
    public GameObject moonDefObj;

    public Transform sunVFX;
    public Transform riseVFX;
    public Transform moonVFX;

    public Transform forceTransform;
    // Start is called before the first frame update
    void Start()
    {
        weaveables = GameObject.FindGameObjectsWithTag("Weaveable");
        dayblockScripts = GetComponentsInChildren<DayblockScript>();

        foreach (DayblockScript ds in dayblockScripts) {
            ds.gotShape = false;
        }
    }

    public void FoundParent() 
    {
        foreach (GameObject weaveable in weaveables) {
            weaveableScript = weaveable.GetComponent<WeaveableNew>();
            //Finds the parent script
            switch (weaveableScript.ID) {
                //Sunblock
                case 0:
                    if (weaveableScript.isParent) {
                        sunblockweaveparent = weaveableScript;
                    }
                break;

                //Sunsetblock
                case 1:
                    if (weaveableScript.isParent) {
                        sunriseblockweaveparent = weaveableScript;
                    }
                break;

                //Moonblock
                case 2:
                    if (weaveableScript.isParent) {
                        moonblockweaveparent = weaveableScript;
                    }
                break;
            }
            
        }
    }

    public void GotCombination(int combination, WeaveableNew weaveableScript, bool isCorrect = true) {
        combinationpart += 1;

        GameObject obj = Instantiate(fakeKeyObject, transform.GetChild(combination));
        fakeObjects.Add(obj);

        setKeyObjects.Add(weaveableScript.gameObject);

        if (isCorrect)
        {
            StartCoroutine(SetGlowIndicator(combinationpart));
        }
    }

    public IEnumerator SetGlowIndicator(int num)
    {

        yield return new WaitForSeconds(1f);
        //Set glowing bit
        switch (num)
        {
            case 1:
                {
                    Instantiate(vfxObject, riseVFX);
                    riseSuccObj.SetActive(true);
                    riseDefObj.SetActive(false);
                    break;
                }
            case 2:
                {
                    Instantiate(vfxObject, sunVFX);
                    sunSuccObj.SetActive(true);
                    sunDefObj.SetActive(false);
                    break;
                }
            case 3:
                {
                    Instantiate(vfxObject, moonVFX);
                    moonSuccObj.SetActive(true);
                    moonDefObj.SetActive(false);
                    break;
                }
        }

        yield break;
    }


    public void FailPuzzle(int correctKey, WeaveableNew weaveable)
    {
        StartCoroutine(RestartPuzzle(correctKey, weaveable));
    }

    public IEnumerator RestartPuzzle(int correctKey, WeaveableNew weaveable) {

        yield return new WaitForSeconds(1f);

        riseDefObj.SetActive(true);
        sunDefObj.SetActive(true);
        moonDefObj.SetActive(true);

        riseSuccObj.SetActive(false);
        sunSuccObj.SetActive(false);
        moonSuccObj.SetActive(false);

        //return original object
        weaveable.transform.position = failSpitPoint[correctKey].position;
        weaveable.Uninteract();

        //reset other objects
        dayblockScripts = GetComponentsInChildren<DayblockScript>();

        int count = 0;
        foreach (DayblockScript ds in dayblockScripts) 
        {
            ds.gotShape = false;
            count++;
        }

        int count2 = 0;
        foreach (GameObject obj in setKeyObjects)
        {
            
            obj.transform.position = failSpitPoint[count2].position;
            
            Vector3 forceDirection = forceTransform.position - failSpitPoint[count2].position;

            obj.GetComponent<Rigidbody>().AddForce(forceDirection * 5, ForceMode.Impulse);

            count2++;
        }
        combinationpart = 0;
        setKeyObjects.Clear();

        foreach(GameObject obj in fakeObjects)
        {
            Destroy(obj);
        }

        fakeObjects.Clear();

        yield break;
    }

    public void PuzzleComplete() 
    {
        StartCoroutine(FinishPuzzle());
    }

    public IEnumerator FinishPuzzle()
    {
        yield return new WaitForSeconds(3);

        GameObject effect = Instantiate(vfxObject, transform.position, transform.rotation);
        effect.transform.localScale = new Vector3(2, 2, 2);
        Instantiate(orbObject, transform.position, transform.rotation);

        yield break;
    }
}
