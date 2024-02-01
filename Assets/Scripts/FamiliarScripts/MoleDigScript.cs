using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MoleDigScript : MonoBehaviour
{
    [Header("References")]
    MolePillarScript molePillarScript;
    [Header("variables")]
    [CannotBeNullObjectField] public GameObject familiar;
    public LayerMask digableLayer;
    [HideInInspector] public float castDistance;
    [HideInInspector] public bool digThroughGround;
    private Collider boxCollider;
    private RaycastHit hitLayer;
    private bool coolDown;
    private float initialYPosition;
    private Vector3 targetPosition;
    public string tagToIgnore; 
    private CharacterController characterController;
    private MovementScript movementScript;
    [CannotBeNullObjectField] public GameObject moleWalkingHolder;
    [CannotBeNullObjectField] public GameObject moleDiggingHolder;
    private bool canBuildPillar; // a bool that flags as true when mole is moving up a dirt pillar

    //[Header("Animation")]
    //[CannotBeNullObjectField] public CharacterAnimationHandler characterAnimationHandler;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        movementScript = familiar.GetComponent<MovementScript>();
        molePillarScript = GetComponent<MolePillarScript>();
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
                initialYPosition = transform.position.y;
                targetPosition = new Vector3(hitLayer.point.x, initialYPosition, hitLayer.point.z);
                StartCoroutine(StartDigging());
                //animation here
                coolDown = true;
                IgnoreCollisionsWithTag(tagToIgnore);
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
                canBuildPillar = true;
                Debug.Log("we're in the dirt pillar baby WOOOOOOOOO" + hitCollider.bounds);
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
    IEnumerator StartDigging()
    {
        yield return new WaitForSeconds(2);
        moleWalkingHolder.SetActive(false);
        moleDiggingHolder.SetActive(true);
        Debug.Log("waited for 2 seconds");
    }

    IEnumerator DiggingOut()
    {
        if (!canBuildPillar)
        {
            molePillarScript.DeployPillar();
        }

        yield return new WaitForSeconds(2);
        moleWalkingHolder.SetActive(true);
        moleDiggingHolder.SetActive(false);
        if (!canBuildPillar)
        {
            molePillarScript.build = true;
        }

        canBuildPillar = false;
        Debug.Log("waited for 2 more seconds");
    }

    void IgnoreCollisionsWithTag(string tag)
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);

        foreach (var obj in objectsWithTag)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), obj.GetComponent<Collider>(), true);
        }
    }

    void ResetCollisionsWithTag(string tag)
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);

        foreach (var obj in objectsWithTag)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), obj.GetComponent<Collider>(), false);
        }
    }
    void ResetCooldown()
    {
        movementScript.enabled = true;
        coolDown = false;
    }
}
