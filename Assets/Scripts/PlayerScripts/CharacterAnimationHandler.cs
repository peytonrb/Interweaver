using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationHandler : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ToggleMoveSpeedBlend(float speed)
    {
        animator.SetFloat("Speed", speed);
    }

    public void ToggleFallAnim(bool falling)
    {
        animator.SetBool("Falling", falling);
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
}
