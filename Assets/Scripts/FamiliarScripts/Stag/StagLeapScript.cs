using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class StagLeapScript : MonoBehaviour
{
    [Header("References")]
    private MovementScript movementScript;
    private CharacterController characterController;
    [Header("Variables")]
    private float currentJumpForce = 0f; 
    private float t = 0f;
    [HideInInspector] public bool chargingJump;
    [Header("Leap Variables")]
    [SerializeField] private float jumpChargeTime = 3f;
    [SerializeField] [Range (1f, 50f)] private float maxJumpForce = 10f;
    [Header("Slam Variables")]
    [SerializeField] private float inititalSlamForce = -10f;
    [SerializeField] private float slamGravity = -30;
    private float startingHeight;
    private bool canSlam;
    private bool displayGizmos;
    // Start is called before the first frame update
    void Start()
    {
        movementScript = GetComponent<MovementScript>();
        characterController = GetComponent<CharacterController>();
    }

    public IEnumerator ChargeJump()
    {
        float startTime = Time.time;

        if (characterController.isGrounded)
        {
            chargingJump = true;
            movementScript.canMove = false;
        }
        else
        {
            if (canSlam)
            {
                movementScript.ChangeVelocity(new Vector3(movementScript.GetVelocity().x, -inititalSlamForce, movementScript.GetVelocity().z));
                startingHeight = transform.position.y;
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
            canSlam = true;
            movementScript.canMove = true;
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
        RaycastHit hit;
        Physics.SphereCast(transform.position + characterController.center, 1f, Vector3.down, out hit);
        if (hit.collider != null)
        {
            Debug.Log("EEEEEEEEEEEE");
        }
        
        while (!characterController.isGrounded)
        {
            yield return null;
        }
        Debug.Log("Distance Fallen: " + (startingHeight - transform.position.y));
        //RaycastHit hit;
        displayGizmos = true;
        //Physics.SphereCast(transform.position + characterController.center, 5f, Vector3.zero, out hit);
        yield return new WaitForSeconds(1f);
        displayGizmos = false;
    }

    private void OnDrawGizmos()
    {
        if (displayGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 5f);
        }
    }
}
