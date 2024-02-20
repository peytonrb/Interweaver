using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportationCrystalScript : MonoBehaviour
{
   
    public GameObject linkedCrystal;
    public bool isTeleportable = true;

   
    //public void GetOtherCrystals()
    //{
    //    this.GetComponent<WeaveableObject>().ResetWeaveable();
    //    this.GetComponent<WeaveableObject>().StopAllCoroutines();
    //    WeaveableManager.Instance.RemoveWeaveableFromList(0, 0);
    //    Debug.Log("This is the linked crystal's name " + linkedCrystal.name + " and this is it's position " + linkedCrystal.transform.position);
    //}

    private void OnTriggerEnter (Collider collision)
    {
        if (collision.gameObject.CompareTag("Weaveable") && this.GetComponent<WeaveableObject>().hasBeenCombined)
        {          
            Debug.Log("this is the weaveable item that it collided with cause physics and shit " + collision.gameObject.name);
            this.GetComponent<WeaveableObject>().StopAllCoroutines();
            this.GetComponent<Rigidbody>().useGravity = true;
            collision.gameObject.GetComponent<WeaveableObject>().StopAllCoroutines();
            //this.GetComponent<WeaveableObject>().hasBeenCombined = false;
            collision.gameObject.GetComponent<WeaveableObject>().hasBeenCombined = false;
            collision.gameObject.GetComponent<WeaveableObject>().ResetWeaveable();
            
            
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
