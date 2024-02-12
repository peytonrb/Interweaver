using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WeaveController : MonoBehaviour
{
    [Header("Camera")]
    private Camera mainCamera;

    [Header("UI")]
    [CannotBeNullObjectField][SerializeField] private GameObject targetingArrow;

    [Header("Weave Functionality")]
    public float weaveDistance = 20f;
    [SerializeField] private LayerMask targetingLayerMask; // should be WeaveObject, Terrain, and Attachable Weave Object
    public LayerMask weaveableLayerMask;
    private Vector3 worldPosition;

    [Header("Audio")]
    [SerializeField] private AudioClip startWeaveClip;
    [SerializeField] private AudioClip weavingLoopClip;
    [SerializeField] private AudioClip weavingIntroClip;
    [SerializeField] private AudioClip weavingOutroClip;

    [Header("For Input Manager - DO NOT MODIFY")]
    public bool isWeaving;
    public WeaveableObject currentWeaveable;
    public WeaveableManager weaveableManager;

    void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        weaveableManager = GameObject.FindWithTag("WeaveableManager").GetComponent<WeaveableManager>();
    }

    // adjusts targeting arrow based on gamepad
    // <param> the direction that the player is looking in
    public void GamepadTargetingArrow(Vector2 lookDir)
    {
        if (lookDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(lookDir.x, lookDir.y) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            targetingArrow.transform.rotation = Quaternion.Euler(0, targetAngle, 0);
            if (isWeaving)
            {
                targetingArrow.SetActive(false);
            }
            else
            {
                targetingArrow.SetActive(true);
            }
        }
        else
        {
            targetingArrow.SetActive(false);
        }
    }

    // adjusts targeting arrow based on mouse position
    // <param> the direction that the player is looking in
    public void MouseTargetingArrow(Vector2 lookDir)
    {
        targetingArrow.SetActive(true);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData, 1000, targetingLayerMask))
        {
            worldPosition = hitData.point;
        }
        Vector3 AdjustedVector = new Vector3(worldPosition.x, transform.position.y, worldPosition.z);
        targetingArrow.transform.LookAt(AdjustedVector);
    }

    // no other objects are being woven. weave this object. 
    // <param> is the player using k&m or a controller
    public void WeaveObject(bool isGamepad)
    {
        bool isValidWeaveable = false;

        if (isGamepad)
        {
            // boxcast in controller targeted direction
            RaycastHit hitInfo;

            // check for Weaveable object within range of BoxCast - adjust width of boxcast if necessary
            if (Physics.BoxCast(transform.position, transform.localScale, targetingArrow.transform.forward, out hitInfo,
                                transform.rotation, weaveDistance, weaveableLayerMask))
            {
                currentWeaveable = hitInfo.collider.GetComponent<WeaveableObject>();
                isValidWeaveable = true;
            }
        }
        else
        {
            // screen to point raycast using mouse position
            Ray rayPlayer = new Ray(transform.position, targetingArrow.transform.forward);
            RaycastHit hitInfo;
            // checks for a Weavable object within distance of Ray
            if (Physics.Raycast(rayPlayer, out hitInfo, weaveDistance, weaveableLayerMask))
            {
                currentWeaveable = hitInfo.collider.GetComponent<WeaveableObject>();
                isValidWeaveable = true;
            }
        }

        // up to here is working for both k&m and controller
        // if weaveable is within range and can be woven...
        if (isValidWeaveable)
        {
            Debug.Log("here");
            // trigger vfx
            // trigger audio
            // disable targeting arrow
            // set weave variables
        }
    }

    // drop objects. does not uncombine objects.
    public void OnDrop()
    {

    }
}