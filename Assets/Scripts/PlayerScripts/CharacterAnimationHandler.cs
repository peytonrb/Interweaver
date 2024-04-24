using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class CharacterAnimationHandler : MonoBehaviour
{
    [HideInInspector] public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ToggleMoveSpeedBlend(float speed)
    {
        animator.SetFloat("Speed", speed);
    }

    public void ToggleJumpAnim()
    {
        animator.SetTrigger("Jump");
    }

    public void ToggleFallAnim(bool falling)
    {
        animator.SetBool("Falling", falling);
    }

    public void ToggleDashAnim(bool dashing)
    {
        animator.SetBool("Dash", dashing);
    }

    public void ToggleWeaveAnim(bool weaving) 
    {
        animator.SetBool("Weaving", weaving);
    }

    public void ToggleDiveAnim(bool diving)
    {
        animator.SetBool("Diving", diving);
    }

    public void ToggleBounceAnim()
    {
        animator.SetTrigger("Crashing");
        animator.SetBool("Diving", false);
    }

    public void ToggleBurrowAnim()
    {
        animator.SetTrigger("Burrow");
    }

    public void ToggleSurfaceAnim()
    {
        animator.SetTrigger("Surface");
    }

    public void ToggleCharging(bool charging)
    {
        animator.SetBool("Charging", charging);
    }

    public void ToggleSlam(bool slaming)
    {
        animator.SetBool("Slam", slaming);
    }

    public void ToggleDeathAnim()
    {
        animator.SetTrigger("Death");
    }

    public void ToggleRespawnAnim()
    {
        animator.SetTrigger("Respawn");
    }

    public void ToggleFireballAnim()
    {
        animator.SetTrigger("Fireball");
    }

    public void ToggleFlamethrowerAnim(bool flamethrower)
    {
        animator.SetBool("Flamethrower", flamethrower);
    }

    public void ToggleHurtAnim()
    {
        animator.SetTrigger("Hurt");
    }

}
