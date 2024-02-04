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
    [HideInInspector] public bool digThroughGround;
    [HideInInspector] public bool borrowed; // different from dig through ground, this means the most is full submerged
    private Collider boxCollider;
    private RaycastHit hitLayer;
    private bool coolDown;
    private float initialYPosition;
    private Vector3 targetPosition;
    public List<string> tagToIgnore = new List<string>(); 
    [CannotBeNullObjectField] public GameObject moleWalkingHolder;
    [CannotBeNullObjectField] public GameObject moleDiggingHolder;

    //[Header("Animation")]
    //[CannotBeNullObjectField] public CharacterAnimationHandler characterAnimationHandler;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        movementScript = familiar.GetComponent<MovementScript>();

        originalHeight = characterController.height;
        digThroughGround = false;
        coolDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, -transform.up, Color.red);
        if (digThroughGround)
        {
            if (Physics.Raycast(transform.position, -transform.up, out hitLayer, castDistance, digableLayer))
            {
                transform.position = new Vector3(transform.position.x, initialYPosition, transform.position.z);
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
        if (Physics.Raycast(transform.position, -transform.up, out hitLayer, castDistance, digableLayer) && !digThroughGround && !coolDown)
        {
            if (!hitLayer.collider.gameObject.CompareTag("Dirt Pillar")) // cannae allow diggin' when we're ontop a dirt pillar
            {
                Debug.Log("we're digging bois");
                digThroughGround = true;
                characterController.height = originalHeight/2;
                initialYPosition = transform.position.y - (originalHeight/4);
                targetPosition = new Vector3(hitLayer.point.x, initialYPosition, hitLayer.point.z);
                StartCoroutine(StartDigging());
                //animation here
                coolDown = true;
                MakePillarsDiggable();
                AudioManager.instance.footStepsChannel.Stop();
                //add the digging sound here if it was added to the audio manager
                movementScript.enabled = false;
                Invoke("ResetCooldown", 2.0f);
            }
        }

        else if (Physics.Raycast(transform.position, -transform.up, out hitLayer, castDistance, digableLayer) && digThroughGround && !coolDown)
        {
            Debug.Log("we got out bois");

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0f, digableLayer);
            foreach (Collider hitCollider in hitColliders)
            {
                transform.position = new Vector3(transform.position.x, hitCollider.bounds.max.y, transform.position.z);
            }

            digThroughGround = false;
            StartCoroutine(DiggingOut());
            //animation here
            coolDown = true;
            ResetCollisionsWithTag(tagToIgnore);
            //add the digging sound here if it was added to the audio manager
            movementScript.enabled = false;
            Invoke("ResetCooldown", 2.0f);
        }
    }

    public void MakePillarsDiggable()
    {
        IgnoreCollisionsWithTag(tagToIgnore);
    }


    IEnumerator StartDigging()
    {
        yield return new WaitForSeconds(2);
        borrowed = true;
        //characterController.height = originalHeight/2;
        moleWalkingHolder.SetActive(false);
        moleDiggingHolder.SetActive(true);
        Debug.Log("waited for 2 seconds");
    }

    IEnumerator DiggingOut()
    {
        borrowed = false;
        yield return new WaitForSeconds(2);
        characterController.height = originalHeight;
        moleWalkingHolder.SetActive(true);
        moleDiggingHolder.SetActive(false);
        Debug.Log("waited for 2 more seconds");
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

    
    void ResetCooldown()
    {
        movementScript.enabled = true;
        coolDown = false;
    }
}
