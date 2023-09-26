using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weaveable : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("This is interactable");
    }
}
