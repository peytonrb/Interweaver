using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class BlackboardScript : MonoBehaviour
{
    [SerializeField] private GameObject interactiveBlackboardUI;
    [SerializeField] private CinemachineVirtualCamera blackboardCamera;
    [SerializeField] private CinemachineBrain mainCamera;
    [Tooltip ("If true, then you are currently staring at the blackboard, and blackboard functionality is on.")] public bool onBlackboard;

    // Start is called before the first frame update
    void Start()
    {
        interactiveBlackboardUI.SetActive(false);
        blackboardCamera.Priority = 0;
        onBlackboard = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GoToFromBlackboard(MovementScript movementScript) {
        if (onBlackboard == false) {
            movementScript.ToggleCanLook(false);
            movementScript.ToggleCanMove(false);
            blackboardCamera.Priority = 2;
            StartCoroutine(WaitForBlendToFinish());
            onBlackboard = true;
        }
        else {
            interactiveBlackboardUI.SetActive(false);
            movementScript.ToggleCanLook(true);
            movementScript.ToggleCanMove(true);
            blackboardCamera.Priority = 0;
            onBlackboard = false;
        }
    }

    IEnumerator WaitForBlendToFinish() {
        yield return null;
        while (mainCamera.IsBlending) {
            yield return null;
        }
        Debug.Log("Hit");
        interactiveBlackboardUI.SetActive(true);
        yield break;
    }
}
