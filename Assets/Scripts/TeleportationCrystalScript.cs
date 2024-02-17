using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportationCrystalScript : MonoBehaviour
{
    public bool isWeaveable = true;
    public GameObject linkedCrystal;
    public bool isTeleportable = true;
    void Start()
    {
        if (!isWeaveable)
        {
            this.GetComponent<WeaveableNew>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }

     public void GetOtherCrystals()
    {
        this.GetComponent<WeaveableNew>().Uncombine();
        Debug.Log("This is the linked crystal's name " + linkedCrystal.name + " and this is it's position " + linkedCrystal.transform.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Weaveable"))
        {          
            Debug.Log("this is the weaveable item that it collided with cause physics and shit " + collision.gameObject.name);           
        }
    }
}
