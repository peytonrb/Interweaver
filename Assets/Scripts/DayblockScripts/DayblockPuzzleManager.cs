using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class DayblockPuzzleManager : MonoBehaviour
{
    private bool parentFound;
    public int combinationpart; //Which part of the combination has been done
    private GameObject[] weaveables;
    [Header ("Do Not Touch")]
    public WeaveableNew sunblockweaveparent;
    public WeaveableNew sunriseblockweaveparent;
    public WeaveableNew moonblockweaveparent;
    private WeaveableNew weaveableScript;
    private DayblockScript[] dayblockScripts;

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
        Debug.Log("Combination part" + combinationpart);
    }

    public void RestartPuzzle() {
        Debug.Log("Get Good!");
        dayblockScripts = GetComponentsInChildren<DayblockScript>();
        
        foreach (DayblockScript ds in dayblockScripts) {
            ds.gotShape = false;
        }
        combinationpart = 0;
    }

    public void PuzzleComplete() {
        Debug.Log("Ayy!");
    }
}
