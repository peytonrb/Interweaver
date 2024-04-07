using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CatapultScript : MonoBehaviour
{
    //[Header("References")]
    [Header("Variables")]
    [SerializeField] [Range (1f, 100f)] private float launchForce;
    [SerializeField] private float timeToLaunch = 2f;
    [SerializeField] private Vector3 direction;
    [SerializeField] private Transform stagSocketPosition;
    [SerializeField] private GameObject wyvern;

    [Header("Audio")]
    [SerializeField] private AudioClip launchSound;

    void Start()
    {
        
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Familiar"))
        {
            collider.gameObject.transform.position = stagSocketPosition.position; // move character to bowl position
            MovementScript movementScript = collider.GetComponent<MovementScript>(); 
            movementScript.ZeroCurrentSpeed();
            movementScript.ChangeVelocity(Vector3.zero);
            StartCoroutine(PrepareToLaunch(collider.gameObject));
        }
    }

    IEnumerator PrepareToLaunch(GameObject gameObject)
    {
        MovementScript movementScript = gameObject.GetComponent<MovementScript>();
        movementScript.enabled = false; // completely move motion of character
        CharacterController characterController = gameObject.GetComponent<CharacterController>();
        StagLeapScript stagLeapScript = characterController.gameObject.GetComponent<StagLeapScript>();
        stagLeapScript.canSlam = false;
        yield return new WaitForSeconds(timeToLaunch); 
        stagLeapScript.canSlam = true;
        Launch(movementScript, characterController);
    }

    private void Launch(MovementScript movementScript, CharacterController characterController)
    {
        movementScript.enabled = true; // unfreeze movement
        movementScript.ToggleCanMove(false); // prevent control of movements
        AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, launchSound, 1f);
        Vector3 launchVelocity = transform.rotation * direction.normalized * launchForce; // calculate initital launch velocity based on rotation of bowl, direction, and force
        movementScript.ChangeVelocity(launchVelocity); // apply velocity to the character
        StartCoroutine(RemoveLaunchForce(movementScript, characterController)); // prepare to remove this added velocity once the character hits the ground
        StagLeapScript stagLeapScript = characterController.gameObject.GetComponent<StagLeapScript>();
        WyvernBossManager wyvernboss = wyvern.GetComponent<WyvernBossManager>();
        wyvernboss.stagWasLaunched = true;
    }

    IEnumerator RemoveLaunchForce(MovementScript movementScript, CharacterController characterController)
    {
        StagLeapScript stagLeapScript = characterController.gameObject.GetComponent<StagLeapScript>(); // this is funky, but works!
        yield return new WaitForFixedUpdate(); // we wait a slight tick for
        while (!characterController.isGrounded)
        {
            yield return null;
        }

        yield return new WaitForFixedUpdate(); // we wait a slight tick for
        if (!stagLeapScript.isStaggered) // check this to make sure we don't overwrite the slam's stagger if present
        {
            movementScript.ToggleCanMove(true);
        }
        
        WyvernBossManager wyvernBoss = wyvern.GetComponent<WyvernBossManager>();
        wyvernBoss.stagWasLaunched = false;
        movementScript.ChangeVelocity(new Vector3(0f, 0f, 0f));
    }

    void OnDrawGizmos()
    {
        DrawArrow.ForGizmo(stagSocketPosition.position, transform.rotation * direction.normalized * launchForce * 0.5f, Color.blue); 
    }
}
