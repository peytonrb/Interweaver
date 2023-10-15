using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GustScript : MonoBehaviour
{
    private CharacterController characterController; //references the character controller component
    private MovementScript movementScript; // reference for the movement script component
    [SerializeField][Range(1f, 10f)] private float gustForce = 3f;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        movementScript = GetComponent<MovementScript>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Gust"))
        {
            movementScript.ChangeGravity(gustForce);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Gust"))
        {
            // DO MATHF.LERP LATER BITCH
            movementScript.ResetGravity();
        }
    }
}