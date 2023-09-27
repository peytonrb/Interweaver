using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaverAnimationHandler : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ToggleMoveSpeedBlend(float speed) // called from PlayerScript
    {
        animator.SetFloat("Speed", speed);
    }

    public void ToggleFallAnim(bool falling) // called from PlayerScript when character controller is not grounded
    {
        animator.SetBool("Falling", falling);
    }

    public void ToggleWeaveAnim(bool weaving) 
    {
        animator.SetBool("Weaving", weaving); // called from Weavable
    }
}
