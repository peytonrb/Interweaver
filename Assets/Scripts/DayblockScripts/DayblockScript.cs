using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayblockScript : MonoBehaviour
{
    //Script will act as a trigger for objects in dayblock.
    public int shapeNeeded; //Identifies the shape of the blocks
    public int shapesCombined;

    public bool gotShape;

    public DayblockPuzzleManager dpm;
    private enum managerCall{ 
        correct,
        incorrect,
        complete
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Weaveable")
        {
            WeaveableNew weaveScript = other.gameObject.GetComponent<WeaveableNew>();
            if (weaveScript.wovenObjects.Count == shapesCombined)
            {
                //Finds if the part of the puzzle has recieved its shape yet.
                if (!gotShape)
                {
                    switch (shapeNeeded)
                    {
                        case 0:
                            {
                                //Check sunrise
                                switch (dpm.combinationpart)
                                {
                                    case 0:
                                        {
                                            StartCoroutine(CallTheManager(managerCall.correct, weaveScript, 0));
                                            gotShape = true;
                                            other.gameObject.GetComponent<WeaveableNew>().Uninteract();
                                            other.transform.position = new Vector3(99999f, -50, 0f);
                                            break;
                                        }
                                    case 1:
                                        {
                                            StartCoroutine(CallTheManager(managerCall.incorrect, weaveScript, 0));
                                            break;
                                        }
                                    case 2:
                                        {
                                            StartCoroutine(CallTheManager(managerCall.incorrect, weaveScript, 0));
                                            break;
                                        }
                                }
                                break;
                            }
                        case 1:
                            {
                                //Check sun
                                switch (dpm.combinationpart)
                                {
                                    case 0:
                                        {
                                            StartCoroutine(CallTheManager(managerCall.incorrect, weaveScript, 1));
                                            break;
                                        }
                                    case 1:
                                        {
                                            StartCoroutine(CallTheManager(managerCall.correct, weaveScript, 1));
                                            gotShape = true;
                                            other.gameObject.GetComponent<WeaveableNew>().Uninteract();
                                            other.transform.position = new Vector3(99999f, -50, 0f);
                                            break;
                                        }
                                    case 2:
                                        {
                                            StartCoroutine(CallTheManager(managerCall.incorrect, weaveScript, 1));
                                            break;
                                        }
                                }
                                break;
                            }
                        case 2:
                            {
                                //Check moon
                                switch (dpm.combinationpart)
                                {
                                    case 0:
                                        {
                                            StartCoroutine(CallTheManager(managerCall.incorrect, weaveScript, 2));
                                            break;
                                        }
                                    case 1:
                                        {
                                            StartCoroutine(CallTheManager(managerCall.incorrect, weaveScript, 2));
                                            break;
                                        }
                                    case 2:
                                        {
                                            StartCoroutine(CallTheManager(managerCall.complete, weaveScript, 2));
                                            other.gameObject.GetComponent<WeaveableNew>().Uninteract();
                                            other.transform.position = new Vector3(99999f, -50, 0f);
                                            break;
                                        }
                                }
                                break;
                            }
                    }
                }
            }
        }


        IEnumerator CallTheManager(managerCall callType, WeaveableNew weaveScript, int correctKey = 0)
        {
            //yield return new WaitForSeconds(1.5f);

            switch(callType)
            {
                case managerCall.correct:
                    {
                        dpm.GotCombination(correctKey, weaveScript);
                        break;
                    }
                case managerCall.incorrect: 
                    {
                        weaveScript.transform.position = dpm.failSpitPoint[correctKey].position;
                        weaveScript.Uninteract();
                        dpm.RestartPuzzle();
                        break;
                    }
                case managerCall.complete:
                    {
                        dpm.GotCombination(correctKey, weaveScript);
                        dpm.PuzzleComplete();
                        break;
                    }
            }
            yield break;
        }
        

    }
}
