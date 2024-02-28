using JetBrains.Annotations;
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
    public enum managerCall
    {
        correct,
        incorrect,
        complete
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Weaveable")
        {
            WeaveableObject weaveScript = other.gameObject.GetComponent<WeaveableObject>();
            // if (weaveScript.wovenObjects.Count == shapesCombined)
            // {
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
                                        Debug.Log("please");
                                        CallTheManager(managerCall.correct, weaveScript, 0);
                                        gotShape = true;
                                        other.gameObject.GetComponent<WeaveableObject>().weaveController.OnDrop();
                                        other.transform.position = new Vector3(99999f, -50, 0f);
                                        break;
                                    }
                                case 1:
                                    {
                                        CallTheManager(managerCall.incorrect, weaveScript, 0);
                                        other.gameObject.GetComponent<WeaveableObject>().weaveController.OnDrop();
                                        other.transform.position = new Vector3(99999f, -50, 0f);
                                        break;
                                    }
                                case 2:
                                    {
                                        CallTheManager(managerCall.incorrect, weaveScript, 0);
                                        other.gameObject.GetComponent<WeaveableObject>().weaveController.OnDrop();
                                        other.transform.position = new Vector3(99999f, -50, 0f);
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
                                        CallTheManager(managerCall.incorrect, weaveScript, 1);
                                        other.gameObject.GetComponent<WeaveableObject>().weaveController.OnDrop();
                                        other.transform.position = new Vector3(99999f, -50, 0f);
                                        break;
                                    }
                                case 1:
                                    {
                                        CallTheManager(managerCall.correct, weaveScript, 1);
                                        gotShape = true;
                                        other.gameObject.GetComponent<WeaveableObject>().weaveController.OnDrop();
                                        other.transform.position = new Vector3(99999f, -50, 0f);
                                        break;
                                    }
                                case 2:
                                    {
                                        CallTheManager(managerCall.incorrect, weaveScript, 1);
                                        other.gameObject.GetComponent<WeaveableObject>().weaveController.OnDrop();
                                        other.transform.position = new Vector3(99999f, -50, 0f);
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
                                        CallTheManager(managerCall.incorrect, weaveScript, 2);
                                        other.gameObject.GetComponent<WeaveableObject>().weaveController.OnDrop();
                                        other.transform.position = new Vector3(99999f, -50, 0f);
                                        break;
                                    }
                                case 1:
                                    {
                                        CallTheManager(managerCall.incorrect, weaveScript, 2);
                                        other.gameObject.GetComponent<WeaveableObject>().weaveController.OnDrop();
                                        other.transform.position = new Vector3(99999f, -50, 0f);
                                        break;
                                    }
                                case 2:
                                    {
                                        CallTheManager(managerCall.complete, weaveScript, 2);
                                        other.gameObject.GetComponent<WeaveableObject>().weaveController.OnDrop();
                                        other.transform.position = new Vector3(99999f, -50, 0f);
                                        break;
                                    }
                            }
                            break;
                        }
                }
            }
        }
        // }

    }

    public void CallTheManager(managerCall callType, WeaveableObject weaveScript, int correctKey = 0)
    {
        switch (callType)
        {
            case managerCall.correct:
                {
                    dpm.GotCombination(correctKey, weaveScript);
                    break;
                }
            case managerCall.incorrect:
                {
                    dpm.GotCombination(correctKey, weaveScript, false);
                    dpm.FailPuzzle(correctKey, weaveScript);
                    break;
                }
            case managerCall.complete:
                {

                    dpm.GotCombination(correctKey, weaveScript);
                    dpm.PuzzleComplete();
                    break;
                }
        }

    }

}
