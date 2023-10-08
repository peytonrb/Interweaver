using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.VisualScripting;

public class FamiliarController : MonoBehaviour
{
    private MovementController movementController;// references the movement controller script
    public InputActionAsset inputs;
    private InputAction possessInput; //Input for depossessing
    private bool depossessButton; //Only used for reading if depossessing
    public bool depossessing;
    public GameObject weaver;

    void OnEnable()
    {
        inputs.Enable();

    }

    void OnDisable()
    {
        inputs.Disable();
    }
    
    void Awake()
    {
        movementController = GetComponent<MovementController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Section reserved for initiating inputs
        possessInput = inputs.FindAction("Player/Switch");
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 0)
        {
            if (movementController.active)
            {
                depossessButton = possessInput.WasPressedThisFrame();

                if (depossessButton)
                {
                    movementController.active = false;
                    depossessing = true;
                }
            }

        }

    }

    public IEnumerator ForcedDelay() 
    {
        yield return new WaitForNextFrameUnit();
        movementController.active = true;
        StopCoroutine(ForcedDelay());
    }
}
