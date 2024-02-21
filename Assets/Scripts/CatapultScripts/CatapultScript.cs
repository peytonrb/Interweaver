using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultScript : MonoBehaviour
{
    [Header("References")]
    public bool active;
    [Header("Variables")]
    [SerializeField] [Range (1f, 100f)] private float launchForce;
    [SerializeField] private Vector3 direction;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Familiar"))
        {
            collider.gameObject.transform.position = transform.position; // move character to bowl position
            MovementScript movementScript = collider.GetComponent<MovementScript>(); 
            movementScript.enabled = false; // completely move motion of character
            StartCoroutine(PrepareToLaunch(collider.gameObject));
        }
    }

    IEnumerator PrepareToLaunch(GameObject gameObject)
    {
        yield return new WaitForSeconds(2f); 
        MovementScript movementScript = gameObject.GetComponent<MovementScript>();
        CharacterController characterController = gameObject.GetComponent<CharacterController>();
        Launch(movementScript, characterController);
    }

    private void Launch(MovementScript movementScript, CharacterController characterController)
    {
        movementScript.enabled = true; // unfreeze movement
        movementScript.ToggleCanMove(false); // prevent control of movements
        Vector3 launchVelocity = transform.rotation * direction.normalized * launchForce; // calculate initital launch velocity based on rotation of bowl, direction, and force
        movementScript.ChangeVelocity(launchVelocity); // apply velocity to the character
        StartCoroutine(RemoveLaunchForce(movementScript, characterController)); // prepare to remove this added velocity once the character hits the ground
    }

    IEnumerator RemoveLaunchForce(MovementScript movementScript, CharacterController characterController)
    {
        StagLeapScript stagLeapScript = characterController.gameObject.GetComponent<StagLeapScript>(); // this is funky, but works!
        while (!characterController.isGrounded)
        {
            yield return null;
        }

        yield return new WaitForFixedUpdate(); // we wait a slight tick for
        if (!stagLeapScript.isStaggered) // check this to make sure we don't overwrite the slam's stagger if present
        {
            movementScript.ToggleCanMove(true);
        }
        
        movementScript.ChangeVelocity(new Vector3(0f, 0f, 0f));
    }

    void OnDrawGizmos()
    {
        DrawArrow.ForGizmo(transform.position, transform.rotation * direction.normalized * launchForce * 0.5f); 
    }
}
