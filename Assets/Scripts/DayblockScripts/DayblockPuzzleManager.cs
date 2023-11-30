using System.Collections;
using System.Collections.Generic;
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

    public Transform[] failSpitPoint;

    private WeaveableNew weaveableScript;
    private DayblockScript[] dayblockScripts;

    public GameObject vfxObject;

    public GameObject fakeKeyObject;

    public GameObject orbObject;

    // Start is called before the first frame update
    void Start()
    {
        weaveables = GameObject.FindGameObjectsWithTag("Weaveable");
        dayblockScripts = GetComponentsInChildren<DayblockScript>();

        foreach (DayblockScript ds in dayblockScripts) {
            ds.gotShape = false;
        }
    }

    public void FoundParent() {
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
    
    public void GotCombination(int combination, WeaveableNew weaveableScript) {
        combinationpart = combination;
        Instantiate(fakeKeyObject, transform.GetChild(combination));
        Debug.Log("Combination part" + combinationpart);

        setKeyObjects.Add(weaveableScript.gameObject);
    }

    public void RestartPuzzle() {
        Debug.Log("Get Good!");
        dayblockScripts = GetComponentsInChildren<DayblockScript>();

        int count = 0;
        foreach (DayblockScript ds in dayblockScripts) {

            ds.transform.position = failSpitPoint[count].position;
            ds.gotShape = false;

            count++;
        }
        combinationpart = 0;
        setKeyObjects.Clear();
    }

    public void PuzzleComplete() {

        Debug.Log("Ayy!");
        Instantiate(vfxObject, transform.position, transform.rotation);
        Instantiate(orbObject, transform.position, transform.rotation);
    }
}
