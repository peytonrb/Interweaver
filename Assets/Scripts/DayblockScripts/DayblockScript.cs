using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayblockScript : MonoBehaviour
{
    //Script will act as a trigger for objects in dayblock.
    public int shapeNeeded; //Identifies the shape of the blocks
    public int shapesCombined;
    
    public GameObject dayblockPuzzleManager;
    public bool gotShape;

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Weaveable") {
            WeaveableNew weaveScript = other.gameObject.GetComponent<WeaveableNew>();

            if (!weaveScript.isParent) {
                FindParent(weaveScript.ID);
            }

            //If this is the parent object
            else {
                if (weaveScript.ID == shapeNeeded) {
                    switch (weaveScript.ID) {
                        //Finds amount of blocks in the shape.
                        case 0:
                            //Sunrise block
                            if (weaveScript.wovenObjects.Count == shapesCombined) {
                                //Finds if the puzzle has been done in the proper order.
                                DayblockPuzzleManager dpm = dayblockPuzzleManager.GetComponent<DayblockPuzzleManager>();
                                if (!gotShape) {
                                    //Finds if the part of the puzzle has recieved its shape yet.
                                    switch (dpm.combinationpart) {
                                        case 0:
                                            dpm.GotCombination(1,weaveScript);
                                            gotShape = true;
                                        break;
                                        case 1:
                                            dpm.RestartPuzzle();
                                        break;
                                        case 2:
                                            dpm.RestartPuzzle();
                                        break;
                                    }
                                }
                                
                            }
                        break;

                        case 1:
                            //Sun block
                            if (weaveScript.wovenObjects.Count == shapesCombined) {
                                DayblockPuzzleManager dpm = dayblockPuzzleManager.GetComponent<DayblockPuzzleManager>();
                                if (!gotShape) {
                                    switch (dpm.combinationpart) {
                                        case 0:
                                            dpm.RestartPuzzle();
                                        break;
                                        case 1:
                                            dpm.GotCombination(2,weaveScript);
                                            gotShape = true;
                                        break;
                                        case 2:
                                            dpm.RestartPuzzle();
                                        break;
                                    }
                                }
                            }

                        break;

                        case 2:
                            //Moon block
                            if (weaveScript.wovenObjects.Count == shapesCombined) {
                                DayblockPuzzleManager dpm = dayblockPuzzleManager.GetComponent<DayblockPuzzleManager>();
                                if (!gotShape) {
                                    switch (dpm.combinationpart) {
                                        case 0:
                                            dpm.RestartPuzzle();
                                        break;
                                        case 1:
                                            dpm.RestartPuzzle();
                                        break;
                                        case 2:
                                            dpm.PuzzleComplete();
                                            gotShape = true;
                                        break;
                                    }
                                }    
                            }
                        break;
                    }
                }
                else {
                    DayblockPuzzleManager dpm = dayblockPuzzleManager.GetComponent<DayblockPuzzleManager>();
                    dpm.RestartPuzzle();
                }
            }
        }
    }

    void FindParent(int blockID) {
        DayblockPuzzleManager dpm = dayblockPuzzleManager.GetComponent<DayblockPuzzleManager>();

        switch (blockID) {
            case 0:
                //Found the sun
                DoTheThing(dpm.sunblockweaveparent);
            break;

            case 1:
                //Found the sunrise
                DoTheThing(dpm.sunriseblockweaveparent);
            break;

            case 2:
                //Found the moon
                DoTheThing(dpm.moonblockweaveparent);
            break;
        }
    }

    void DoTheThing(WeaveableNew weaveableScript) {
        if (weaveableScript.ID == shapeNeeded) {
                switch (weaveableScript.ID) {
                    //Finds amount of blocks in the shape.
                    case 0:
                        //Sunrise block
                        if (weaveableScript.wovenObjects.Count == shapesCombined) {
                            //Finds if the puzzle has been done in the proper order.
                            DayblockPuzzleManager dpm = dayblockPuzzleManager.GetComponent<DayblockPuzzleManager>();
                            if (!gotShape) {
                                switch (dpm.combinationpart) {
                                    case 0:
                                        dpm.GotCombination(1,weaveableScript);
                                        gotShape = true;
                                    break;
                                    case 1:
                                        dpm.RestartPuzzle();
                                    break;
                                    case 2:
                                        dpm.RestartPuzzle();
                                    break;
                                }
                            }  
                        }
                    break;

                    case 1:
                        //Sun block
                        if (weaveableScript.wovenObjects.Count == shapesCombined) {
                            DayblockPuzzleManager dpm = dayblockPuzzleManager.GetComponent<DayblockPuzzleManager>();
                            if (!gotShape) {
                                switch (dpm.combinationpart) {
                                    case 0:
                                        dpm.RestartPuzzle();
                                    break;
                                    case 1:
                                        dpm.GotCombination(2,weaveableScript);
                                        gotShape = true;
                                        
                                    break;
                                    case 2:
                                        dpm.RestartPuzzle();
                                    break;
                                }
                            }
                            
                        }
                    break;

                    case 2:
                        //Moon block
                        if (weaveableScript.wovenObjects.Count == shapesCombined) {
                            DayblockPuzzleManager dpm = dayblockPuzzleManager.GetComponent<DayblockPuzzleManager>();
                            if (!gotShape) {
                                switch (dpm.combinationpart) {
                                    case 0:
                                        dpm.RestartPuzzle();
                                    break;
                                    case 1:
                                        dpm.RestartPuzzle();
                                    break;
                                    case 2:
                                        dpm.PuzzleComplete();
                                        gotShape = true;
                                    break;
                                }
                            }
                            
                        }
                    break;
                }
        }
        else {
            DayblockPuzzleManager dpm = dayblockPuzzleManager.GetComponent<DayblockPuzzleManager>();
            dpm.RestartPuzzle();
        }
    }

}
