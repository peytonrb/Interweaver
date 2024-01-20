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

        if (OnDigableLayer())
        {          
            Debug.Log("yooo this is digable?");
        }
    }

    public bool OnDigableLayer()
    {
        if (Physics.Raycast(transform.position, -transform.up, castDistance, digableLyer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DigPressed()
    {
        if (!digThroughGround && OnDigableLayer())
        {
            Debug.Log("we're digging bois");
            digThroughGround = true;
        }

        else if (digThroughGround && OnDigableLayer())
        {
            Debug.Log("we got out bois");
            digThroughGround = false;
        }
    }


}
