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
                Debug.Log("Test");
                break;
            }
            if (hitCollider.gameObject.tag == "Blackboard") 
            {
                BlackboardScript blackboard = hitCollider.gameObject.GetComponent<BlackboardScript>();
                if (blackboard != null) {
                    blackboard.GoToFromBlackboard(GetComponent<MovementScript>());
                }
                break;
            }
            if (hitCollider.gameObject.tag == "Interactable Book")
            {
                HubDiageticBookUIScript bookscript = hitCollider.gameObject.GetComponent<HubDiageticBookUIScript>();
                if (bookscript != null) {
                    bookscript.GoToFromBook(GetComponent<MovementScript>());
                }
            }
        }
    }
}
