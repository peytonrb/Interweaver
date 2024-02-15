using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractionScript : MonoBehaviour
{
    public float proximity = 5f;

    public void Interact()
    {
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, proximity); // second number is radius of sphere
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag == "NPC")
            {
                hitCollider.gameObject.GetComponent<DialogueTriggers>().TriggerDialogue(GetComponent<MovementScript>());
                break;
            }
        }
    }
}
