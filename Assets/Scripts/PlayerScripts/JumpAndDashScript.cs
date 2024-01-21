using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class JumpAndDashScript : MonoBehaviour
{
    [Header("References")]
    private MovementScript movementScript;
    private CharacterController characterController;

    [Header("Utility")]
    [SerializeField] private bool showDistanceGizmo = true;

    [Header("Jumping")]
    [SerializeField][Range(1, 10)] private float jumpForce = 3f;
    [SerializeField] [Tooltip("Allows the weaver to jump forever instead of dash")] private bool infiniteJump;

    [Header("Dashing")]
    [SerializeField] private bool canDash;
    [SerializeField] [Range(0.1f, 10f)] private float dashLength = 0.1f;
    [SerializeField] [Range(1f, 100f)] private float dashSpeed = 50f;
    [SerializeField] [Tooltip("Determines if gravity will affect weaver while dashing")] private bool zeroGravDash;

    void Start()
    {
        movementScript = GetComponent<MovementScript>();
        characterController = GetComponent<CharacterController>();
    }

    public void DoJumpDash()
    {
        if (characterController.isGrounded || infiniteJump)
        {
            movementScript.ChangeVelocity(new UnityEngine.Vector3(movementScript.GetVelocity().x, jumpForce, movementScript.GetVelocity().z));
        }
        if (canDash && !characterController.isGrounded && !infiniteJump)
        {
            if (zeroGravDash)
            {
                movementScript.ChangeVelocity(new UnityEngine.Vector3(movementScript.GetVelocity().x, 0f, movementScript.GetVelocity().z));
                movementScript.ChangeGravity(0f);
            }
            movementScript.active = false;
            canDash = false;
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        float startTime = Time.time;
        while (Time.time < startTime + dashLength)
        {
            characterController.Move(gameObject.transform.forward * dashSpeed * Time.deltaTime);
            yield return null;
        }
        if (zeroGravDash)
        {
            movementScript.ResetGravity();
        }

        StartCoroutine(GroundCheck());
        movementScript.active = true;
    }

    IEnumerator GroundCheck()
    {
        while (!characterController.isGrounded)
        {
            yield return null;
        }
        canDash = true;
    }

    void OnDrawGizmos()
    {
        if (showDistanceGizmo)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + gameObject.transform.forward * dashSpeed * dashLength);
        }
    }
}
