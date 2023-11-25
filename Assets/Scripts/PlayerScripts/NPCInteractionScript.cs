using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractionScript : MonoBehaviour
{
    public void Interact()
    {
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, 10f); // second number is radius of sphere
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag == "NPC")
            {
                hitCollider.gameObject.GetComponent<DialogueTriggers>().triggerDialogue(GetComponent<MovementScript>());
                
            }
        }
    }
}
