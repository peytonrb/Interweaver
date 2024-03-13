using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessionUIAnimationHandler : MonoBehaviour
{
    [HideInInspector] public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SwitchingToFamiliar()
    {
        animator.SetBool("SwitchToFamiliar", true);
        animator.SetBool("SwitchToWeaver", false);
    }

    public void SwitchingToWeaver()
    {
        animator.SetBool("SwitchToFamiliar", false);
        animator.SetBool("SwitchToWeaver", true);
    }
}
