using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StagLeapScript : MonoBehaviour
{
    [Header("References")]
    private MovementScript movementScript;
    private CharacterController characterController;
    [Header("Variables")]
    private float currentJumpForce = 0f; // the current jump force
    //private float t = 0f;
    [HideInInspector] public bool chargingJump; // is a jump currently being charged?
    [Header("Leap Variables")]
    [SerializeField] private float jumpChargeTime = 3f; // how long is takes to charge a jump to max
    private Vector3 groundLocation; // marks the position of the ground directly below the stag while airborne
    private bool displayGroundGizmos; // displays gizmo 
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

    [Header("Utility")]
    [SerializeField] private bool showSlamRadiusGizmo;
    [SerializeField] private bool showMaxJumpHeightGizmo;
    

    // Start is called before the first frame update
    void Start()
    {
        movementScript = GetComponent<MovementScript>();
        characterController = GetComponent<CharacterController>();
        if (!canOnlySlamAfterLeap)
        {
            canSlam = true;
        }
        wasLaunched = false;
    }

    public IEnumerator ChargeJump()
    {
        float startTime = Time.time;
        float t = 0f;

        if (characterController.isGrounded)
        {
            chargingJump = true;
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
            canSlam = true;
            movementScript.ToggleCanMove(true);
            chargingJump = false;
            if (movementScript.canMove && characterController.isGrounded) 
            {
                movementScript.ChangeVelocity(new Vector3(movementScript.GetVelocity().x, currentJumpForce, movementScript.GetVelocity().z));
            }
        }
    }

    private IEnumerator StartSlam()
    {
        canSlam = false;

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
        Vector3 slamSpot = transform.position;
        StartCoroutine(GrowSlamRadius(slamSpot));
        canSlam = true;
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
                //Put in reference to boss if and only if stag was launched out of a cannon
                if (hitCollider.gameObject.CompareTag("Boss")) {
                    if (wasLaunched == true) {
                        WyvernBossManager wyvern = hitCollider.gameObject.GetComponent<WyvernBossManager>();
                        wyvern.HurtWyvern();
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
    }
}
