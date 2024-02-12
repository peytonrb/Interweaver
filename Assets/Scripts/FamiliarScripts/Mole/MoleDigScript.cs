using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MoleDigScript : MonoBehaviour
{
    [Header("References")]
    private CharacterController characterController;
    private MovementScript movementScript;
    private float originalHeight;
    [Header("Variables")]
    [CannotBeNullObjectField] public GameObject familiar;
    public LayerMask digableLayer;
    [HideInInspector] public float castDistance;
    [HideInInspector] public bool startedToDig; // true when digging begins, false when digging has stopped  
    [HideInInspector] public bool borrowed; // different from startedToDig, this means the most is full submerged
    private Collider boxCollider;
    private RaycastHit hitLayer;
    private bool coolDown;
    private Vector3 targetPosition;
    public List<string> tagToIgnore = new List<string>();
    private float animLength;
    [Header("Animation")]
    [CannotBeNullObjectField] public CharacterAnimationHandler characterAnimationHandler;

    //[Header("Animation")]
    //[CannotBeNullObjectField] public CharacterAnimationHandler characterAnimationHandler;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        movementScript = familiar.GetComponent<MovementScript>();
        animLength = characterAnimationHandler.animator.GetCurrentAnimatorStateInfo(0).length;
        originalHeight = characterController.height;
        startedToDig = false;
        coolDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, -transform.up, Color.red);
        if (startedToDig)
        {
            if (Physics.Raycast(transform.position, -transform.up, out hitLayer, castDistance, digableLayer))
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5.0f);
            }
        }
    }



    public void DigPressed()
    {
        //casts a raycast from the player to the ground to check if they are on a digable layer and then ignores the collision
        //of that specific layer so then it can walk through the pillar of dirt
        if (Physics.Raycast(transform.position, -transform.up, out hitLayer, castDistance, digableLayer) && !startedToDig && !coolDown)
        {
            if (!hitLayer.collider.gameObject.CompareTag("Dirt Pillar")) // cannae allow diggin' when we're ontop a dirt pillar
            {
                ActualDigging();
            }
            else
            {
                MakePillarsDiggable();
                movementScript.active = false;
                Invoke("ResetCooldown", 3.0f);
                StartCoroutine(BurrowDownPillar());
            }
        }

        else if (Physics.Raycast(transform.position, -transform.up, out hitLayer, castDistance, digableLayer) && startedToDig && !coolDown)
        {
            Debug.Log("we got out bois");

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0f, digableLayer);
            foreach (Collider hitCollider in hitColliders)
            {
                transform.position = new Vector3(transform.position.x, hitCollider.bounds.max.y, transform.position.z);
            }

            //animation here
            characterAnimationHandler.ToggleSurfaceAnim();
            
            Debug.Log("Dug out " + animLength);
            
            startedToDig = false;
            StartCoroutine(DiggingOut(animLength));

            coolDown = true;
            ResetCollisionsWithTag(tagToIgnore);
            //add the digging sound here if it was added to the audio manager
            movementScript.ZeroCurrentSpeed(); // we do this to prevent sudden jarring movement after movement script is re-enabled
            movementScript.enabled = false;
            Invoke("ResetCooldown", 3.0f);
        }
    }

    public void ActualDigging()
    {
        Debug.Log("we're digging bois");
        startedToDig = true;
        
        targetPosition = new Vector3(hitLayer.point.x, hitLayer.point.y, hitLayer.point.z);

        //animation here
        characterAnimationHandler.ToggleBurrowAnim();

        Debug.Log("Dug in " + animLength);

        StartCoroutine(StartDigging(animLength));

        coolDown = true;
        MakePillarsDiggable();
        AudioManager.instance.footStepsChannel.Stop();
        //add the digging sound here if it was added to the audio manager
        movementScript.ZeroCurrentSpeed(); // we do this to prevent sudden jarring movement after movement script is re-enabled
        movementScript.enabled = false;
        Invoke("ResetCooldown", 3.0f);
    }

    public void MakePillarsDiggable()
    {
        IgnoreCollisionsWithTag(tagToIgnore);
    }


    IEnumerator StartDigging(float animLength)
    {
        yield return new WaitForSeconds(animLength);
        borrowed = true;
        Debug.Log("waited for " + animLength);
    }

    IEnumerator DiggingOut(float animLength)
    {
        borrowed = false;
        yield return new WaitForSeconds(animLength); 
        //animation here so then it can play after it's on top of a pillar
        Debug.Log("waited for " + animLength);
    }

    void IgnoreCollisionsWithTag(List<string> tag)
    {
        List<Collider> collidersToIgnore = new List<Collider>();

        // Find all GameObjects with the specified tags
        foreach (var t in tag)
        {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(t);

            // Add colliders from each object to the list
            foreach (var obj in objectsWithTag)
            {
                Collider collider = obj.GetComponent<Collider>();

                if (collider != null)
                {
                    collidersToIgnore.Add(collider);
                }
            }
        }

        // Ignore collisions with each collider in the list
        foreach (var colliderToIgnore in collidersToIgnore)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), colliderToIgnore, true);
        }
    }

    void ResetCollisionsWithTag(List<string> tag)
    {
        List<Collider> collidersToIgnore = new List<Collider>();

        // Find all GameObjects with the specified tags
        foreach (var t in tag)
        {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(t);

            // Add colliders from each object to the list
            foreach (var obj in objectsWithTag)
            {
                Collider collider = obj.GetComponent<Collider>();

                if (collider != null)
                {
                    collidersToIgnore.Add(collider);
                }
            }
        }

        // Ignore collisions with each collider in the list
        foreach (var colliderToIgnore in collidersToIgnore)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), colliderToIgnore, false);
        }
    }

    IEnumerator BurrowDownPillar()
    {
        yield return new WaitForFixedUpdate();
        while (!characterController.isGrounded)
        {
            yield return null;
        }
        movementScript.active = true;
        ActualDigging(); 

    }

    
    void ResetCooldown()
    {
        movementScript.enabled = true;
        coolDown = false;
    }
}
