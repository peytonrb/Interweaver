using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleDigScript : MonoBehaviour
{
    [Header("variables")]
    public LayerMask digableLyer;
    public float castDistance;
    public bool digThroughGround;
    
    void Start()
    {
        digThroughGround = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, -transform.up, Color.red);
    }

  

    public void DigPressed()
    {
        RaycastHit hitLayer;

        if (Physics.Raycast(transform.position, -transform.up, out hitLayer, castDistance, digableLyer) && !digThroughGround)
        {
            Debug.Log("we're digging bois");
            digThroughGround = true;
            DigAction(hitLayer.collider);
            //Debug.Log(hitLayer.collider.gameObject.name);
        }
             
        else if (Physics.Raycast(transform.position, -transform.up, out hitLayer, castDistance, digableLyer) && digThroughGround)
        {
            Debug.Log("we got out bois");
            digThroughGround = false;
        }

    }

    public void DigAction(Collider funnyBox)
    {        
        //Debug.Log("we actually digging this bois " + funnyBox.gameObject.name);
        Debug.Log("the bounds of the box " + funnyBox.bounds);
    }
}
