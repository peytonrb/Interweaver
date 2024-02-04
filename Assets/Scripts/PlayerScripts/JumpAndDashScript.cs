using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class JumpAndDashScript : MonoBehaviour
{
    [Header("References")]
    private MovementScript movementScript;
    private CharacterController characterController;
    [Header("Jumping")]
    [SerializeField][Range(1, 10)] private float jumpForce = 3f;
    [SerializeField] [Tooltip("Allows the weaver to jump forever instead of dash")] private bool infiniteJump;

    [Header("Dashing")]
    [SerializeField] private bool canDash;
    [SerializeField] [Range(0f, 10f)] private float dashCooldown = 0.4f;
    [SerializeField] [Range(0.1f, 10f)] private float dashLength = 0.1f;
    [SerializeField] [Range(1f, 100f)] private float dashSpeed = 50f;
    [SerializeField] [Tooltip("Determines if gravity will affect weaver while dashing")] private bool zeroGravDash;
    [SerializeField] [Tooltip("If in air at the end of a dash, increase affect of gravity by the heavyDashEffect variable when dash ends until touching ground")] private bool heavyDash;
    [SerializeField] [Range(1f, 25f)] private float heavyDashEffect = 3f;
    private float t = 0;
    [Header("Utility")]
    [SerializeField] private bool showDistanceGizmo = true;

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
    }

    public void DoDash()
    {
        if (canDash)
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
        float currentDashSpeed;
        while (Time.time < startTime + dashLength)
        {
            t += Time.deltaTime/dashLength;
            currentDashSpeed = Mathf.Lerp(dashSpeed, 0, t);
            characterController.Move(gameObject.transform.forward * currentDashSpeed * Time.deltaTime);

            yield return null;
        }
        t = 0;
        if (zeroGravDash)
        {
            movementScript.ResetGravity();
        }

        if (heavyDash && !characterController.isGrounded)
        {
            float gravityMod = movementScript.GetGravity() - heavyDashEffect;
            movementScript.ChangeGravity(gravityMod);
            StartCoroutine(HeavyDash());
        }

        StartCoroutine(DashCooldown());
        movementScript.active = true;
    }

    IEnumerator DashCooldown()
    {
        float time = dashCooldown;
        while (time > 0)
        {
            if (characterController.isGrounded) // pauses timer whilst in air
            {
                time -= Time.deltaTime;
            }
            yield return null;
        }
        canDash = true;
    }

    IEnumerator HeavyDash()
    {
        while (!characterController.isGrounded)
        {
            yield return null;
        }
        movementScript.ResetGravity();
    }

    void OnDrawGizmos()
    {
        if (showDistanceGizmo)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + gameObject.transform.forward * (dashSpeed/2 * dashLength));
        }
    }
}
