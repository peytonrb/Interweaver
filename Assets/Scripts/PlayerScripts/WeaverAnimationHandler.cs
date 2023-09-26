using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaverAnimationController : MonoBehaviour
{
    private Animator animator;
    private PlayerScript playerScript;
    private CharacterController characterController;
    [SerializeField] private float speed; 
    [SerializeField] private bool weaving;
    [SerializeField] private bool falling;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        //playerScript = GetComponent<PlayerScript>();
        //characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (true) // [CHANGER LATER] if character controller is grounded
        {
            animator.SetBool("Weaving", weaving); // [CHANGE LATER] determines if player character is in the weaving animation set or not
            animator.SetFloat("Speed", speed); // [CHANGE LATER] sets animator's speed param to player's current speed
        }

        else // character controller is NOT grounded and is therefore falling
        {
            falling = true;
        }

        animator.SetBool("Falling", falling); 
        if (falling)
        {
            weaving = false;
        }
    }
}
