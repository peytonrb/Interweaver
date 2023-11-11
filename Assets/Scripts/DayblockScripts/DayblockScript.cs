using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayblockScript : MonoBehaviour
{
    //Script will act as a trigger for objects in dayblock.
    public int shapeNeeded; //Identifies the shape of the blocks
    private int combinationpart; //Which part of the combination has been done
    public GameObject dayblockPuzzleManager;

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
                            //Sun block
                            if (weaveScript.wovenObjects.Count == 3) {
                                //Finds if the puzzle has been done in the proper order.
                                DayblockPuzzleManager dpm = dayblockPuzzleManager.GetComponent<DayblockPuzzleManager>();
                                switch (combinationpart) {
                                    case 0:
                                        dpm.RestartPuzzle();
                                    break;
                                    case 1:
                                        combinationpart = 2;
                                    break;
                                    case 2:
                                        dpm.RestartPuzzle();
                                    break;
                                }
                            }
                        break;

                        case 1:
                            //Sunrise block
                            if (weaveScript.wovenObjects.Count == 3) {
                                DayblockPuzzleManager dpm = dayblockPuzzleManager.GetComponent<DayblockPuzzleManager>();
                                switch (combinationpart) {
                                    case 0:
                                        combinationpart = 1;
                                    break;
                                    case 1:
                                        dpm.RestartPuzzle();  
                                    break;
                                    case 2:
                                        dpm.RestartPuzzle();
                                    break;
                                }
                            }

                        break;

                        case 2:
                            //Moon block
                            if (weaveScript.wovenObjects.Count == 2) {
                                DayblockPuzzleManager dpm = dayblockPuzzleManager.GetComponent<DayblockPuzzleManager>();
                                switch (combinationpart) {
                                    case 0:
                                        dpm.RestartPuzzle();
                                    break;
                                    case 1:
                                        dpm.RestartPuzzle();
                                    break;
                                    case 2:
                                        dpm.PuzzleComplete();
                                    break;
                                }
                            }
                        break;
                    }
                }
            }
        }
    }

    void FindParent(int blockID) {
        DayblockPuzzleManager dpm = dayblockPuzzleManager.GetComponent<DayblockPuzzleManager>();

        switch (blockID) {
            case 0:
                //Found the sun
                WeaveableNew weaveScript = dpm.sunblockweaveparent;
            break;

            case 1:
                //Found the sunrise
            break;

            case 2:
                //Found the moon
            break;
        }
    }

}
