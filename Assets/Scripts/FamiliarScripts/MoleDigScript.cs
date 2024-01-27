using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MoleDigScript : MonoBehaviour
{
    [Header("variables")]
    public LayerMask digableLayer;
    [HideInInspector] public float castDistance;
    [HideInInspector] public bool digThroughGround;
    private Collider boxCollider;
    private bool coolDown;
    [CannotBeNullObjectField] public GameObject moleWalkingHolder;
    [CannotBeNullObjectField] public GameObject moleDiggingHolder;

    [Header("Animation")]
    [CannotBeNullObjectField] public CharacterAnimationHandler characterAnimationHandler;
    void Start()
    {
        digThroughGround = false;
        coolDown = false;

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, -transform.up, Color.red);
       if (digThroughGround)
       {
            ClampPosition(boxCollider.bounds);
       }
    }



    public void DigPressed()
    {
        RaycastHit hitLayer;

        if (Physics.Raycast(transform.position, -transform.up, out hitLayer, castDistance, digableLayer) && !digThroughGround && !coolDown)
        {
            Debug.Log("we're digging bois");
            digThroughGround = true;
            DigAction(hitLayer.collider);
           StartCoroutine(StartDigging());
            //animation here
            coolDown = true;
            Invoke("ResetCooldown", 2.0f);         
        }

        else if (Physics.Raycast(transform.position, -transform.up, out hitLayer, castDistance, digableLayer) && digThroughGround && !coolDown)
        {
            Debug.Log("we got out bois");
            digThroughGround = false;
            StartCoroutine(DiggingOut());
            //animation here
            coolDown = true;
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
        yield return new WaitForSeconds(2);
        moleWalkingHolder.SetActive(true);
        moleDiggingHolder.SetActive(false);
        Debug.Log("waited for 2 more seconds");
    }
    public void DigAction(Collider funnyBox)
    {
        boxCollider = funnyBox;
        ClampPosition(boxCollider.bounds);
    }

    public void ClampPosition(Bounds clampBounds)
    {
        transform.position = new Vector3(
        Mathf.Clamp(transform.position.x,clampBounds.min.x, clampBounds.max.x),
        transform.position.y,
        Mathf.Clamp(transform.position.z, clampBounds.min.z, clampBounds.max.z));
    }

    void ResetCooldown()
    {
        coolDown = false;
    }
}
