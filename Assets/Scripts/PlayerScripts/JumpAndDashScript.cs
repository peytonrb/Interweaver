using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class JumpAndDashScript : MonoBehaviour
{
    [Header("Animation")]
    [CannotBeNullObjectField] public CharacterAnimationHandler characterAnimationHandler;

    [Header("References")]
    private MovementScript movementScript;
    private CharacterController characterController;
    [Header("Jumping")]
    [SerializeField][Range(1, 10)] private float jumpForce = 3f;
    [SerializeField][Tooltip("Allows the weaver to jump forever instead of dash")] private bool infiniteJump;
    [SerializeField] private float coyoteTimeLength;
    private float coyoteTime;
    [Header("Dashing")]
    [HideInInspector] public bool dashLock; // use this in situations where you're TEMPORARILY preventing the use of dash 
    public bool canDash; // accessed by WeaveableObject
    [SerializeField][Range(0f, 10f)] private float dashCooldown = 0.4f;
    [SerializeField][Range(0.1f, 10f)] private float dashLength = 0.1f;
    [SerializeField][Range(1f, 100f)] private float dashSpeed = 50f;
    [SerializeField][Tooltip("Determines if gravity will affect weaver while dashing")] private bool zeroGravDash;
    [SerializeField][Tooltip("If in air at the end of a dash, increase affect of gravity by the heavyDashEffect variable when dash ends until touching ground")] private bool heavyDash;
    [SerializeField][Range(1f, 25f)] private float heavyDashEffect = 3f;
    [Tooltip("Resets dash cooldown when you jump")] public bool freeJumpDash;
    [SerializeField] private bool canTurnWhileDashing;
    [SerializeField][Range(0.1f, 2f)] private float dashTimeToTurn = 1.0f;
    private float t = 0;

    [Header("Utility")]
    [SerializeField] private bool showDistanceGizmo = true;

    [Header("VFX")]
    private TrailRenderer dashTrail;
    public Material glowMaterial;
    private GameObject skinnedMeshWeaver;
    private Material originalMaterial;

    [Header("Audio")]

    [SerializeField] private AudioClip dashAudioClip;

    void Start()
    {
        movementScript = GetComponent<MovementScript>();
        characterController = GetComponent<CharacterController>();
        coyoteTime = coyoteTimeLength;

        // vfx
        if (gameObject.CompareTag("Player"))
        {
            this.transform.Find("DashTrail").TryGetComponent<TrailRenderer>(out dashTrail);
            skinnedMeshWeaver = this.transform.Find("SK_Weaver/SM_Weaver").gameObject;
            originalMaterial = skinnedMeshWeaver.GetComponent<Renderer>().material;
            DisableDashVFX();
        }
        
    }

    private void Update()
    {
        if (!characterController.isGrounded && !infiniteJump && coyoteTime > 0)
        {
            coyoteTime -= Time.deltaTime;
        }
        else if (characterController.isGrounded && coyoteTime < coyoteTimeLength)
        {
            coyoteTime = coyoteTimeLength;
        }
    }

    public void DoJump()
    {
        if (characterController.isGrounded || infiniteJump || coyoteTime > 0)
        {
            if (movementScript.canMove) {
                StartCoroutine(JumpBuffer());
                characterAnimationHandler.ToggleJumpAnim();
                movementScript.ChangeVelocity(new UnityEngine.Vector3(movementScript.GetVelocity().x, jumpForce, movementScript.GetVelocity().z));
                if (freeJumpDash)
                {
                    canDash = true;
                    StopCoroutine(DashCooldown());
                }
            }
        }
    }

    IEnumerator JumpBuffer() // there's a very brief period where a character has JUST jumped, but it still grounded, this coroutine simply waits till their officially off the ground to avoid double jumps
    {
        while (characterController.isGrounded)
        {
            yield return null;
        }
        coyoteTime = 0;
    }

    public void DoDash()
    {
        if (canDash && movementScript.canMove && !dashLock)
        {
            if (zeroGravDash)
            {
                movementScript.ChangeVelocity(new UnityEngine.Vector3(movementScript.GetVelocity().x, 0f, movementScript.GetVelocity().z));
                movementScript.ChangeGravity(0f);
            }
            if (!canTurnWhileDashing)
            {
                movementScript.canLook = false;
            }
            movementScript.ToggleCanMove(false);
            movementScript.ChangeTimeToTurn(dashTimeToTurn);
            canDash = false;
            characterAnimationHandler.ToggleDashAnim(true);
            
            EnableDashVFX();
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        float startTime = Time.time;
        float currentDashSpeed;
        while (Time.time < startTime + dashLength)
        {
            t += Time.deltaTime / dashLength;
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
        movementScript.ResetTimeToTurn();
        if (!canTurnWhileDashing)
        {
            movementScript.canLook = true;
        }
        movementScript.ToggleCanMove(true);
        characterAnimationHandler.ToggleDashAnim(false);
        DisableDashVFX();
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

    private void EnableDashVFX()
    {
        // trail
        dashTrail.emitting = true;
        dashTrail.transform.GetChild(0).GetComponent<TrailRenderer>().emitting = true;
        dashTrail.transform.GetChild(1).GetComponent<TrailRenderer>().emitting = true;

        dashTrail.transform.GetChild(2).GetComponent<ParticleSystem>().Play();

        // material
        StartCoroutine(FadeGlowIn());

        //Audio
        AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, dashAudioClip);
    }

    private void DisableDashVFX()
    {
        // material
        StartCoroutine(FadeGlowOut());

        dashTrail.emitting = false;
        StartCoroutine(TrailLinger());
        dashTrail.transform.GetChild(1).GetComponent<TrailRenderer>().emitting = false;
    }

    IEnumerator FadeGlowIn()
    {
        skinnedMeshWeaver.GetComponent<Renderer>().material = glowMaterial;
        skinnedMeshWeaver.GetComponent<Renderer>().material.SetFloat("GlowTotal", 1f);
        Color color = skinnedMeshWeaver.GetComponent<Renderer>().material.color;
        color.a = 0f;
        float t = 0;

        while (t < 0.5f)
        {
            t += Time.deltaTime;
            float blend = Mathf.Clamp01(t / 0.5f);
            color.a = Mathf.Lerp(0f, 100f, blend);
            skinnedMeshWeaver.GetComponent<Renderer>().material.color = color;
            yield return null;
        }
    }

    IEnumerator FadeGlowOut()
    {
        if (skinnedMeshWeaver.GetComponent<Renderer>().material != originalMaterial)
        {
            float glowAmount = skinnedMeshWeaver.GetComponent<Renderer>().material.GetFloat("GlowTotal");
            float t = 0;

            while (t < dashCooldown)
            {
                t += Time.deltaTime;
                float blend = Mathf.Clamp01(t / dashCooldown);
                float blendNum = Mathf.Lerp(1f, 0f, blend);
                skinnedMeshWeaver.GetComponent<Renderer>().material.SetFloat("GlowTotal", blendNum);
                yield return null;
            }
        }

        skinnedMeshWeaver.GetComponent<Renderer>().material = originalMaterial;
    }

    IEnumerator TrailLinger()
    {
        yield return new WaitForSeconds(dashCooldown);
        dashTrail.transform.GetChild(0).GetComponent<TrailRenderer>().emitting = false;
    }

    void OnDrawGizmos()
    {
        if (showDistanceGizmo)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + gameObject.transform.forward * (dashSpeed / 2 * dashLength));
        }
    }
}
