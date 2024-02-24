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
    public void GetOtherCrystals()
    {
        this.GetComponent<WeaveableNew>().Uncombine();
        Debug.Log("This is the linked crystal's name " + linkedCrystal.name + " and this is it's position " + linkedCrystal.transform.position);
    }

    private void OnTriggerEnter (Collider collision)
    {
        if (collision.gameObject.CompareTag("Weaveable") && this.GetComponent<WeaveableNew>().isCombined)
        {          
            Debug.Log("this is the weaveable item that it collided with cause physics and shit " + collision.gameObject.name);
            this.GetComponent<WeaveableNew>().StopAllCoroutines();
            collision.gameObject.GetComponent<WeaveableNew>().Uncombine();
            collision.gameObject.GetComponent <WeaveableNew>().StopAllCoroutines();
            collision.transform.position = new Vector3 (linkedCrystal.transform.position.x, 
                linkedCrystal.transform.position.y, linkedCrystal.transform.position.z + 3);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        DrawArrow.ForGizmo(transform.position,Vector3.forward * 3);
    }
}
