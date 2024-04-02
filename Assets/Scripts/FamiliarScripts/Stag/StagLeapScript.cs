using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class StagLeapScript : MonoBehaviour
{
    [Header("References")]
    private MovementScript movementScript;
    private CharacterController characterController;
    [CannotBeNullObjectField] public CharacterAnimationHandler characterAnimationHandler;
    private CameraMasterScript cameraMasterScript;
    [Header("Variables")]
    private float currentJumpForce = 0f; // the current jump force
    [HideInInspector] public bool chargingJump; // is a jump currently being charged?
    [Header("Leap Variables")]
    [SerializeField] private float jumpChargeTime = 3f; // how long is takes to charge a jump to max
    private Vector3 groundLocation; // marks the position of the ground directly below the stag while airborne
    [SerializeField] private Vector3 bonkBoxSize;
    [SerializeField] private float bonkBoxHeight = 0;
    [SerializeField] private LayerMask bonkBoxLayerMask;
    [SerializeField] private float headBonkStunLength = 0.2f;
    [SerializeField] [Range (1f, 75f)] private float maxJumpForce = 10f;
    [Header("Slam Variables")]
    [SerializeField] private float inititalSlamForce = -10f; // the initial slam force, if y velocity is already lower, just adds this number
    [SerializeField] private float slamGravity = -30; // gravity of slam
    private float startingHeight; // starting height of slam
    [SerializeField] private bool canOnlySlamAfterLeap; // determines if slams can only be done after a leap
    [HideInInspector] public bool canSlam; // can the stag slam at this moment?
    private bool displaySlamGizmos; // displays size of slam
    [SerializeField][Range (2f, 6f)] private float maxSlamRadius = 5f;
    [SerializeField] private float slamTime = 0.6f;
    private float currentSlamRadius = 0f;
    [SerializeField][Range(0f, 3f)] private float postSlamStaggerTime = 0.6f;
    [HideInInspector] public bool isStaggered;
    [HideInInspector] public bool wasLaunched;

    [Header("Screenshake")]
    [SerializeField][Range(0,5)] private float amplitude = 0.8f;
    [SerializeField][Range(0,5)] private float frequency = 0.8f;

    [Header("Utility")]
    private bool displayGroundGizmos; // displays gizmo 
    [SerializeField] private bool showSlamRadiusGizmo;
    [SerializeField] private bool showMaxJumpHeightGizmo;
    [SerializeField] private bool showBonkBoxGizmo;

    [Header("VFX")]
    private StagChargingVFXController stagChargingVFXScript;
    private StagLeapVFXController stagLeapVFXScript;
    private StagGroundPoundVFXController stagGroundPoundVFXScript;

    // Start is called before the first frame update
    void Start()
    {
        movementScript = GetComponent<MovementScript>();
        characterController = GetComponent<CharacterController>();
        cameraMasterScript = GameObject.FindGameObjectWithTag("CameraMaster").GetComponent<CameraMasterScript>();
        if (!canOnlySlamAfterLeap)
        {
            canSlam = true;
        }
        wasLaunched = false;

        stagChargingVFXScript = this.transform.Find("StagLeapChargingVFX").GetComponent<StagChargingVFXController>();
        stagLeapVFXScript = this.transform.Find("StagLeapVFX").GetComponent<StagLeapVFXController>();
        stagGroundPoundVFXScript = this.transform.Find("StagGroundPoundVFX").GetComponent<StagGroundPoundVFXController>();
    }

    void Update()
    {
        if (!characterController.isGrounded && movementScript.GetVelocity().y > 0 && !isStaggered)
        {
            Collider[] hitColliders = Physics.OverlapBox(transform.position + (transform.up * bonkBoxHeight), bonkBoxSize/2, transform.rotation, bonkBoxLayerMask);
            foreach (Collider hitCollider in hitColliders)
            {
                StartCoroutine(DoStagHeadBonkAndStun());
                break;
            }
        }
    }

    private IEnumerator DoStagHeadBonkAndStun()
    {
        float originalGravity = movementScript.GetGravity();

        isStaggered = true;
        cameraMasterScript.ShakeCurrentCamera(amplitude, frequency, headBonkStunLength);

        movementScript.ToggleCanMove(false);
        movementScript.ChangeVelocity(new Vector3(movementScript.GetVelocity().x, 0, movementScript.GetVelocity().z));
        movementScript.ChangeGravity(0f);

        yield return new WaitForSeconds(headBonkStunLength);

        movementScript.ChangeGravity(originalGravity);
        movementScript.ChangeVelocity(new Vector3(movementScript.GetVelocity().x, -15f, movementScript.GetVelocity().z));
        movementScript.ToggleCanMove(true);
        isStaggered = false;
    }

    public IEnumerator ChargeJump()
    {
        float startTime = Time.time;
        float t = 0f;

        if (characterController.isGrounded)
        {
            chargingJump = true;
            stagChargingVFXScript.StartEffect();
            characterAnimationHandler.ToggleCharging(chargingJump);
            movementScript.ZeroCurrentSpeed();
            movementScript.ToggleCanMove(false);
        }
        else
        {
            if (canSlam)
            {
                StartCoroutine(StartSlam());
            }
        }

        while (chargingJump && (Time.time < startTime + jumpChargeTime))
        {
            t += Time.deltaTime / jumpChargeTime;
            currentJumpForce = Mathf.Lerp(0, maxJumpForce, t);
            yield return null;
        }
        t = 0;
    }

    public void EndCharging()
    {
        if (chargingJump)
        {
            StartCoroutine(ShowGroundMarker());
            stagLeapVFXScript.StartVFX(stagLeapVFXScript.transform.position);
            canSlam = true;
            movementScript.ToggleCanMove(true);
            chargingJump = false;
            stagChargingVFXScript.EndEffect();
            characterAnimationHandler.ToggleCharging(chargingJump);
            if (movementScript.canMove && characterController.isGrounded) 
            {
                movementScript.ChangeVelocity(new Vector3(movementScript.GetVelocity().x, currentJumpForce, movementScript.GetVelocity().z));
            }
        }
    }

    private IEnumerator StartSlam()
    {
        canSlam = false;
        stagGroundPoundVFXScript.PlaySlammingVFX();

        characterAnimationHandler.ToggleSlam(true);

        if (movementScript.GetVelocity().y >= 0)
        {
            movementScript.ChangeVelocity(new Vector3(movementScript.GetVelocity().x, inititalSlamForce, movementScript.GetVelocity().z));
        }
        else
        {
            movementScript.ChangeVelocity(new Vector3(movementScript.GetVelocity().x, movementScript.GetVelocity().y + inititalSlamForce, movementScript.GetVelocity().z));
        }
        startingHeight = transform.position.y;
        
        while (!characterController.isGrounded)
        {
            yield return null;
        }
        // put all impact stuff past this point
        //Debug.Log("Distance Fallen: " + (startingHeight - transform.position.y));
        stagGroundPoundVFXScript.DisableSlammingVFX();
        stagGroundPoundVFXScript.PlayExplosionVFX();
        
        Vector3 slamSpot = transform.position;
        StartCoroutine(GrowSlamRadius(slamSpot));
        canSlam = true;
        characterAnimationHandler.ToggleSlam(false);
        movementScript.ToggleCanMove(false);
        isStaggered = true;
        yield return new WaitForSeconds(postSlamStaggerTime);
        isStaggered = false;
        if (!chargingJump)
        {
            movementScript.ToggleCanMove(true);
        }
    }

    private IEnumerator ShowGroundMarker()
    {
        yield return new WaitForFixedUpdate();
        RaycastHit hit;
        while (!characterController.isGrounded)
        {
            Physics.SphereCast(transform.position + characterController.center, 1f, Vector3.down, out hit);
            if (hit.collider != null)
            {
                displayGroundGizmos = true; // we'll replace this with some other kind of marker later on
                groundLocation = hit.point;
            }
            yield return null;
        }
        displayGroundGizmos = false;
    }

    private IEnumerator GrowSlamRadius(Vector3 slamSpot)
    {
        
        displaySlamGizmos = true;
        float startTime = Time.time;
        float t = 0f;
        while (Time.time < startTime + slamTime)
        {
            t += Time.deltaTime / slamTime;
            currentSlamRadius = Mathf.Lerp(0, maxSlamRadius, t);
            Collider[] hitColliders = Physics.OverlapSphere(slamSpot, currentSlamRadius);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.CompareTag("Breakable"))
                {
                    Destroy(hitCollider.gameObject); // call the thing being broken here!
                }
                if (hitCollider.gameObject.CompareTag("Boss")) {
                    WyvernBossManager wyvernBossManager = hitCollider.gameObject.GetComponent<WyvernBossManager>();
                    if (wyvernBossManager != null) {
                        if (wyvernBossManager.stagWasLaunched) {
                            wyvernBossManager.HurtWyvern();
                            wyvernBossManager.stagWasLaunched = false;
                        }
                        else {
                            FamiliarScript familiarScript = gameObject.GetComponent<FamiliarScript>();
                            familiarScript.Death();
                        }
                    }
                }
                
            }
            yield return null;
        }
        displaySlamGizmos = false;
    }

    private void OnDrawGizmos()
    {
        if (displayGroundGizmos)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(groundLocation, 1f);
        }
        if (displaySlamGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, currentSlamRadius);
        }
        if (showSlamRadiusGizmo)
        {
            Gizmos.DrawWireSphere(transform.position, maxSlamRadius);
        }
        if (showMaxJumpHeightGizmo)
        {
            DrawArrow.ForGizmo(transform.position, transform.up * maxJumpForce);
        }

        if (showBonkBoxGizmo)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawCube(transform.position + (transform.up * bonkBoxHeight), bonkBoxSize);
        }
    }
}
