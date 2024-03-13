using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public class MoleDigScript : MonoBehaviour
{
    [Header("References")]
    private CharacterController characterController;
    private MovementScript movementScript;
    private float originalHeight;
    [HideInInspector] public bool isOnDigableLayer;
    private bool digableLayerInvoke;

    [Header("Variables")]
    [CannotBeNullObjectField] public GameObject familiar;

    [Header("VFX")]
    [CannotBeNullObjectField] [SerializeField] private GameObject moundModel;
    [CannotBeNullObjectField] [SerializeField] private GameObject moleModel;
    [SerializeField] private float vfxDelay = 1f;
    private float randomRot;
    //private float elapsedTime;
    //private float storedTime;
    //private bool canPause;
    private VisualEffect dirtTrailVFX;
    private bool IsMoving;
    public LayerMask digableLayer;
    public float castDistance;
    [HideInInspector] public bool startedToDig; // true when digging begins, false when digging has stopped  
    [HideInInspector] public bool borrowed; // different from startedToDig, this means the most is full submerged
    private Collider boxCollider;
    private RaycastHit hitLayer;
    private bool coolDown;
    [SerializeField] private Vector3 targetPosition;
    public List<string> tagToIgnore = new List<string>();
    [SerializeField] private float animLength = 1.5f;

    [Header("Animation")]
    [CannotBeNullObjectField] public CharacterAnimationHandler characterAnimationHandler;

    //[Header("Animation")]
    //[CannotBeNullObjectField] public CharacterAnimationHandler characterAnimationHandler;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        movementScript = familiar.GetComponent<MovementScript>();
        transform.Find("VFX_Dirt").GetComponent<VisualEffect>().Stop();
        dirtTrailVFX = transform.Find("VFX_DirtTrail").GetComponent<VisualEffect>();
        dirtTrailVFX.gameObject.SetActive(false);
        //animLength = characterAnimationHandler.animator.GetCurrentAnimatorStateInfo(0).length;
        originalHeight = characterController.height;
        startedToDig = false;
        coolDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, -transform.up, Color.red);

        if(movementScript.currentSpeed > 0)
        {
            IsMoving = true;
            //canPause = true;
            ShaderSpeedControl(IsMoving);
        }
        else
        {
            /*if(canPause)
            {
                storedTime = Time.time;
                canPause = false;
            }*/
            IsMoving = false;
            ShaderSpeedControl(IsMoving);
        }

        moundModel.transform.eulerAngles = new Vector3(0, randomRot, 0);

        if ((startedToDig))
        {
            if (Physics.Raycast(transform.position, -transform.up, out hitLayer, castDistance, digableLayer))
            {
                boxCollider = hitLayer.collider;
            }

            if ((!boxCollider.gameObject.CompareTag("Dirt Pillar")))
            {

                if (Physics.Raycast(transform.position, -transform.up, out hitLayer, castDistance, digableLayer))
                {
                    //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);                                 
                    targetPosition = new Vector3(hitLayer.point.x, hitLayer.point.y, hitLayer.point.z);                    
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, (targetPosition - (transform.forward*2)), Time.deltaTime * 5.0f);
                }
            }
        }

        if (isOnDigableLayer && !dirtTrailVFX.gameObject.activeSelf && movementScript.currentSpeed > 0)
        {
            dirtTrailVFX.gameObject.SetActive(true); // inefficient mb
        }
        else if ((!isOnDigableLayer && dirtTrailVFX.gameObject.activeSelf) || movementScript.currentSpeed <= 0)
        {
            dirtTrailVFX.gameObject.SetActive(false);
        }


        //this is so fucking ass I'm so sorry
       //**********************************************
        if ((Physics.Raycast(transform.position, -transform.up, out hitLayer, castDistance, digableLayer)) && !digableLayerInvoke)
        {
            isOnDigableLayer = true;
            digableLayerInvoke = true;
        }

        else if ((!Physics.Raycast(transform.position, -transform.up, out hitLayer, castDistance, digableLayer)) && digableLayerInvoke)
        {
            isOnDigableLayer = false;
            digableLayerInvoke = false;
        }
        //**********************************************
    }



    public void DigPressed()
    {
        //casts a raycast from the player to the ground to check if they are on a digable layer and then ignores the collision,
        //this also includes the dirt pillar tag
        //of that specific layer so then it can walk through the pillar of dirt
        if (Physics.Raycast(transform.position, -transform.up, out hitLayer, castDistance, digableLayer) && !startedToDig && !coolDown)
        {
            boxCollider = hitLayer.collider;
            if (!hitLayer.collider.gameObject.CompareTag("Dirt Pillar")) // cannae allow diggin' when we're ontop a dirt pillar
            {
                ActualDigging();
                AnimationForDiggingDown();
            }
            else
            {
                MakePillarsDiggable();
                movementScript.active = false;
                Invoke("ResetCooldown", 1.5f);
                StartCoroutine(BurrowDownPillar());
            }
        }

        else if (Physics.Raycast(transform.position, -transform.up, out hitLayer, castDistance, digableLayer) && startedToDig && !coolDown)
        {
            //Debug.Log("we got out bois");
            //checking the mole in other colliders and if it's a dirt pillar tag, it goes up
            int weaveableLayer = LayerMask.NameToLayer("weaveObject"); 
            int weaveableLayerMask = 1 << weaveableLayer;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0f, digableLayer);
            Collider[] hitWeaveableColliders = Physics.OverlapSphere(transform.position, 0f, weaveableLayerMask);
            foreach (Collider hitCollider in hitColliders)
            {
                transform.position = new Vector3(transform.position.x, hitCollider.bounds.max.y + 0.5f, transform.position.z);
            }
            foreach (Collider hitWeaveableCollider in hitWeaveableColliders)
            {
                transform.position = new Vector3(transform.position.x, hitWeaveableCollider.bounds.max.y + 1, transform.position.z);
            }

            AnimationForDiggingUp();

            coolDown = true;
            ResetCollisionsWithTag(tagToIgnore);
            //add the digging sound here if it was added to the audio manager
            movementScript.ZeroCurrentSpeed(); // we do this to prevent sudden jarring movement after movement script is re-enabled
            movementScript.enabled = false;
            Invoke("ResetCooldown", 1.5f);
        }
    }


    public void MakePillarsDiggable()
    {
        IgnoreCollisionsWithTag(tagToIgnore);
    }

    #region//ANIMATION,ENUMS, AND METHOD FOR DIGGING
    //******************************************
    public void ActualDigging() //the actual digging and staying on the dirt layer
    {
        //Debug.Log("we're digging bois");
        startedToDig = true;
       

        coolDown = true;
        MakePillarsDiggable();
        AudioManager.instance.footStepsChannel.Stop();
        //add the digging sound here if it was added to the audio manager
        movementScript.ZeroCurrentSpeed(); // we do this to prevent sudden jarring movement after movement script is re-enabled
        movementScript.enabled = false;
        Invoke("ResetCooldown", 1.5f);
    }
    public void AnimationForDiggingDown()
    {
        //animation here
        characterAnimationHandler.ToggleBurrowAnim();

        //Debug.Log("Dug in " + animLength);

        StartCoroutine(StartDigging(animLength));
        StartCoroutine(VFXStart(vfxDelay));
    }
    public void AnimationForDiggingUp()
    {
        //animation here
        characterAnimationHandler.ToggleSurfaceAnim();

        //Debug.Log("Dug out " + animLength);

        startedToDig = false;
        
        moleModel.GetComponent<Renderer>().enabled = true;
        
        StartCoroutine(DiggingOut(animLength));

        transform.Find("VFX_Dirt").GetComponent<VisualEffect>().Play();
    }
    IEnumerator StartDigging(float animLength)
    {
        yield return new WaitForSeconds(animLength);
        borrowed = true;
        
        moleModel.GetComponent<Renderer>().enabled = false;
        
        //Debug.Log("waited for " + animLength);

        transform.Find("VFX_Dirt").GetComponent<VisualEffect>().Stop();
    }

    IEnumerator DiggingOut(float animLength)
    {
        borrowed = false;
        moundModel.GetComponent<Animator>().SetTrigger("Lower");
        yield return new WaitForSeconds(animLength/2);
        
        moundModel.GetComponent<Renderer>().enabled = false;
        //animation here so then it can play after it's on top of a pillar
        
        //Debug.Log("waited for " + animLength/2);

        transform.Find("VFX_Dirt").GetComponent<VisualEffect>().Stop();
    }

    IEnumerator VFXStart(float vfxDelay)
    {
        yield return new WaitForSeconds(vfxDelay);
        transform.Find("VFX_Dirt").GetComponent<VisualEffect>().Play();
        moundModel.GetComponent<Renderer>().enabled = true;
        moundModel.GetComponent<Animator>().SetTrigger("Raise");
        randomRot = Random.Range(0, 360);
    }
    //******************************************
    #endregion

    #region//METHOD FOR IGNORING COLLISION WITH TAG/ RESET COLLISION
    //************************************
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
    //************************************
    #endregion
    IEnumerator BurrowDownPillar()
    {
        AnimationForDiggingDown();
        yield return new WaitForFixedUpdate();
        while (!characterController.isGrounded)
        {
            yield return null;
        }
        movementScript.active = true;
        ActualDigging();

    }

    void ShaderSpeedControl(bool IsMoving)
    {
        if(IsMoving)
        {
            moundModel.GetComponent<Renderer>().material.SetFloat("_IsMoving", 1);
        }
        else
        {
            moundModel.GetComponent<Renderer>().material.SetFloat("_IsMoving", 0);
            /*moundModel.GetComponent<Renderer>().material.SetFloat("_StoredTime", storedTime);

            elapsedTime = Time.time - storedTime;
            moundModel.GetComponent<Renderer>().material.SetFloat("_ElapsedTime", elapsedTime);*/
        }
    }


    void ResetCooldown()
    {
        movementScript.enabled = true;
        coolDown = false;
    }
}
