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
        if (OnDigableLayer())
        {
            Debug.DrawRay(transform.position, -transform.up, Color.red);
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
        if (digThroughGround)
        {
            Debug.Log("we're digging bois");
            digThroughGround = false;
        }
        else if (!digThroughGround)
        {
            Debug.Log("we got out bois");
            digThroughGround = true;
        }
    }


}
