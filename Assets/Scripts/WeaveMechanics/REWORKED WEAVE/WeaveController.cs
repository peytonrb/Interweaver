using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WeaveController : MonoBehaviour
{
    [Header("General")]
    public bool usingAudio = true;
    public LayerMask weaveableLayers;

    [Header("Camera")]
    private Camera mainCamera;

    [Header("UI")]
    [CannotBeNullObjectField] public GameObject targetingArrow;

    [Header("Weave Functionality")]
    public float weaveDistance = 20f;
    [SerializeField] private LayerMask targetingLayerMask; // should be WeaveObject, Terrain, and Attachable Weave Object
    public LayerMask weaveableLayerMask;
    private Vector3 worldPosition;
    [HideInInspector] public WeaveFXScript weaveFXScript;
    private GameObject weaveSpawn;
    public int maxCombinedObjects = 4;
    [SerializeField] private float combineRange = 10;

    [Header("Audio")]
    [SerializeField] private AudioClip startWeaveClip;
    [SerializeField] private AudioClip weavingLoopClip;
    [SerializeField] private AudioClip weavingIntroClip;
    [SerializeField] private AudioClip weavingOutroClip;

    [Header("Other Script References - DO NOT MODIFY")]
    [HideInInspector] public bool isWeaving;
    [HideInInspector] public WeaveableObject currentWeaveable;
    [HideInInspector] public WeaveableObject selectedWeaveable;
    [HideInInspector] public Vector2 lookDirection;
    private MovementScript movementScript;
    private PlayerControllerNew playerControllerNew;
    [SerializeField] public Transform targetSphere;
    [HideInInspector] public bool isHoveringObject;

    void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        weaveFXScript = this.GetComponent<WeaveFXScript>();
        weaveSpawn = this.transform.Find("WeaveSpawn").gameObject;
        movementScript = this.GetComponent<MovementScript>();
        targetingArrow.SetActive(true);
        playerControllerNew = this.GetComponent<PlayerControllerNew>();
    }

    void Update()
    {
        // draws weave line. is in update due to dynamic positions of weaver and weaveable
        if (isWeaving)
        {
            weaveFXScript.DrawWeave(weaveSpawn.transform.position, currentWeaveable.transform.position);
        }

        //unsure if this would cause problems but I'm pretty sure it won't
        if (!InputManagerScript.instance.isGamepad && movementScript.active)
        {
            MouseTargetingArrow();
        }



        if (this.GetComponent<PlayerControllerNew>().isDead)
        {
            OnDrop();
        }

        if (!movementScript.canMove && targetSphere.gameObject.activeSelf)
        {
            targetSphere.gameObject.SetActive(false);
        }
        else if (movementScript.canMove && !targetSphere.gameObject.activeSelf)
        {
            targetSphere.gameObject.SetActive(true);
        }
    }

    // adjusts targeting arrow based on gamepad
    // <param> the direction that the player is looking in
    public void GamepadTargetingArrow(Vector2 lookDir)
    {
        lookDirection = lookDir;
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
    // public void MouseTargetingArrow(Vector2 lookDir) <---this is what it was before for the method
    public void MouseTargetingArrow()
    {
        targetingArrow.SetActive(true);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;

        if (Physics.Raycast(ray, out hitData, 1000, targetingLayerMask))
        {
            worldPosition = hitData.point;
        }

        Vector3 adjustedVector = new Vector3(worldPosition.x, transform.position.y, worldPosition.z);
        targetingArrow.transform.LookAt(adjustedVector);

        //adjust target sphere location
        targetSphere.position = hitData.point;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, weaveableLayers)) // this is.... expensive
        {
            if (hit.collider.GetComponent<WeaveableObject>() != null)
            {
                if (!hit.collider.GetComponent<WeaveableObject>().materialIsOn && Vector3.Distance(this.transform.position, hit.collider.transform.position) < 20f)
                {
                    Renderer rend = hit.collider.transform.GetChild(0).GetComponent<Renderer>();
                    Material[] mats = rend.materials;
                    Material existingMat = mats[0];
                    Material[] newMats = new Material[2];
                    newMats[0] = existingMat;
                    newMats[1] = weaveFXScript.emissiveMat;
                    rend.materials = newMats;
                    hit.collider.GetComponent<WeaveableObject>().materialIsOn = true;
                    isHoveringObject = true;
                }
            }
        }
        else
        {
            isHoveringObject = false;
        }
    }

    // no other objects are being woven. weave this object. 
    // <param> is the player using k&m or a controller
    public void WeaveObject(bool isGamepad)
    {
        bool isValidWeaveable = false;
        //probably decrease the distance or somehow make it where it doesn't ignore walls
        //holy fuck it worked?
        Vector3 distanceBetweenPlayerWeaveable = worldPosition - transform.position;
        float direction = distanceBetweenPlayerWeaveable.magnitude;

        if (isGamepad)
        {
            // boxcast in controller targeted direction
            RaycastHit hitInfo;



            // check for Weaveable object within range of BoxCast - adjust width of boxcast if necessary
            if (Physics.BoxCast(transform.position, transform.localScale * 1.2f, targetingArrow.transform.forward, out hitInfo,
                                transform.rotation, weaveDistance, weaveableLayerMask))
            {
                if (!hitInfo.collider.GetComponent<WeaveableObject>().isFloatingIsland)
                {
                    currentWeaveable = hitInfo.collider.GetComponent<WeaveableObject>();
                    isValidWeaveable = true;

                }

            }
        }
        else
        {
            // screen to point raycast using mouse position
            Ray rayPlayer = new Ray(transform.position, targetingArrow.transform.forward);
            RaycastHit hitInfo;

            // checks for a Weavable object within distance of Ray

            if (Physics.SphereCast(transform.position, 1f, rayPlayer.direction, out hitInfo, direction, weaveableLayerMask)) // changed to spherecast so horizontal objects are easier to pick up
            {
                if (!hitInfo.collider.GetComponent<WeaveableObject>().isFloatingIsland)
                {
                    currentWeaveable = hitInfo.collider.GetComponent<WeaveableObject>();
                    isValidWeaveable = true;
                }
            }
        }

        // if weaveable is within range and can be woven...
        if (isValidWeaveable)
        {
            currentWeaveable.isBeingWoven = true;
            if (currentWeaveable.TryGetComponent<Rigidbody>(out Rigidbody rb))
                rb.useGravity = false;

            movementScript.freeMove = true; // lets weaver move even when not facing where they're moving
            StartCoroutine(PlayWeaveVFX());
            StartCoroutine(WaitForVFX()); // sets isWeaving to true. is in Coroutine for aesthetic purposes.
            currentWeaveable.AddToWovenObjects();
            this.GetComponent<JumpAndDashScript>().dashLock = true;
            // toggle on animation here
        }
    }

    // plays VFX and Audio at proper timings
    // accessed by WeaveableObject
    public IEnumerator PlayWeaveVFX()
    {
        // VFX
        weaveFXScript.ActivateWeave(currentWeaveable.transform);
        CameraMasterScript.instance.ShakeCurrentCamera(Random.Range(.3f,.6f), 1f, .32f);
        // Audio
        if (usingAudio)
        {
            AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, startWeaveClip);
            yield return new WaitForSeconds(.1f);
            AudioManager.instance.PlaySound(AudioManagerChannels.weaveLoopingChannel, weavingIntroClip);
            yield return new WaitForSeconds(.732f);
            AudioManager.instance.PlaySound(AudioManagerChannels.weaveLoopingChannel, weavingLoopClip);
        }

        yield break;
    }

    // drop objects. does not uncombine objects.
    public void OnDrop()
    {
        isWeaving = false;

        //Disables the Weaving bool in the animator for the animation blending
        playerControllerNew.WeaveAnimation(false);

        if (currentWeaveable != null) // in case of floating islands
            currentWeaveable.GetComponent<Rigidbody>().velocity = Vector3.down * 2f;

        movementScript.freeMove = false; // returns weaver to needing to be facing where they're moving
        weaveFXScript.DisableWeave();

        // if this is the only weaveable in the list (weaveables are added on combine)
        if (currentWeaveable != null && !currentWeaveable.hasBeenCombined)
        {
            StartCoroutine(EndWeaveAudio());
            currentWeaveable.ResetWeaveable();
            WeaveableManager.Instance.RemoveWeaveableFromList(0, 0);
        }
        else if (currentWeaveable != null)
        {
            StartCoroutine(EndWeaveAudio());
            if (WeaveableManager.Instance.combinedWeaveables.Count > 0 && WeaveableManager.Instance.combinedWeaveables[currentWeaveable.listIndex] != null)
            {
                for (int i = 0; i < WeaveableManager.Instance.combinedWeaveables[currentWeaveable.listIndex].weaveableObjectGroup.Count; i++)
                {
                    WeaveableManager.Instance.combinedWeaveables[currentWeaveable.listIndex].weaveableObjectGroup[i].ResetWeaveable();
                }
            }
        }

        currentWeaveable = null;
        selectedWeaveable = null;
        this.GetComponent<JumpAndDashScript>().dashLock = false;
        // toggle off animation here
    }

    // stops all audio if audio is enabled
    IEnumerator EndWeaveAudio()
    {
        if (usingAudio)
        {
            AudioManager.instance.PlaySound(AudioManagerChannels.weaveLoopingChannel, weavingOutroClip);
            yield return new WaitForSeconds(.732f);
            AudioManager.instance.StopSound(AudioManagerChannels.weaveLoopingChannel);
        }

        yield break;
    }

    // helper function that allows seconds to be dynamic to be used easily with VFX/Audio calls
    private IEnumerator WaitForVFX()
    {
        yield return new WaitForSeconds(0.2f);
        isWeaving = true;

        //Enables the Weaving bool in the animator for the animation blending
        playerControllerNew.WeaveAnimation(true);
    }

    // is called by InputManager, is used to determine the weaveable actively being selected for COMBINING
    // !! limit calls to this function
    // <param> is player using a controller or k&m
    public void CheckIfWeaveable(bool isGamepad)
    {
        //somehow the the box cast and the raycasts are ignoring the walls?????
        // prevents you from combining more than the max amount of weaveables per group
        if (WeaveableManager.Instance.combinedWeaveables[currentWeaveable.listIndex].weaveableObjectGroup.Count < maxCombinedObjects)
        {
            if (isGamepad)
            {
                // boxcast in controller targeted direction
                RaycastHit hitInfo;
                ExtDebug.DrawBoxCastBox(currentWeaveable.transform.position - new Vector3(0, currentWeaveable.hoverHeight, 0), transform.localScale * 1.2f, transform.rotation, currentWeaveable.targetingArrow.transform.forward, weaveDistance, Color.green);

                // check for Weaveable object within range of BoxCast sent from CURRENTWEAVEABLE
                if (Physics.BoxCast(currentWeaveable.transform.position - new Vector3(0, currentWeaveable.hoverHeight, 0), transform.localScale * 1.2f,
                                    currentWeaveable.targetingArrow.transform.forward, out hitInfo,
                                    currentWeaveable.transform.rotation, weaveDistance, weaveableLayerMask))
                {
                    if (hitInfo.collider.GetComponent<WeaveableObject>() != currentWeaveable)
                    {
                        if (!hitInfo.collider.GetComponent<WeaveableObject>().isFloatingIsland)
                        {
                            selectedWeaveable = hitInfo.collider.GetComponent<WeaveableObject>();
                            currentWeaveable.CombineObject();
                        }
                        else
                        {
                            if (currentWeaveable.TryGetComponent<CrystalScript>(out CrystalScript script))
                            {
                                selectedWeaveable = hitInfo.collider.GetComponent<WeaveableObject>();
                                currentWeaveable.CombineObject();
                            }
                        }
                    }
                }
            }
            else
            {
                // screen to point raycast using mouse position
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;

                // checks for a Weavable object within distance of Ray
                //Physics.SphereCast(transform.position, 4f, ray.direction, out hitInfo, 100f, weaveableLayerMask);
                if (Physics.Raycast(ray, out hitInfo, 100f, weaveableLayerMask))
                {
                    if (hitInfo.collider.GetComponent<WeaveableObject>() != currentWeaveable)
                    {
                        if (!hitInfo.collider.GetComponent<WeaveableObject>().isFloatingIsland)
                        {
                            selectedWeaveable = hitInfo.collider.GetComponent<WeaveableObject>();

                            if (Vector3.Distance(selectedWeaveable.transform.position, currentWeaveable.transform.position) <= combineRange)
                                currentWeaveable.CombineObject();
                        }
                        else
                        {
                            if (currentWeaveable.TryGetComponent<CrystalScript>(out CrystalScript script))
                            {
                                Debug.Log("this is going off");
                                selectedWeaveable = hitInfo.collider.GetComponent<WeaveableObject>();

                                if (Vector3.Distance(selectedWeaveable.transform.position, currentWeaveable.transform.position) <= combineRange)
                                    currentWeaveable.CombineObject();
                            }
                        }

                    }
                }
            }
        }
    }

}