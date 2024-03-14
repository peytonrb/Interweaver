using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUIScript : MonoBehaviour
{

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private bool isSpeakerPost;
    private Vector3 offset = new Vector3(0, 3.5f, 0);

    // Update is called once per frame
    private void Start()
    {
        isSpeakerPost = false;
    }
    void Update()
    {
        transform.rotation = mainCamera.transform.rotation;

        if(!isSpeakerPost) 
        {
            transform.position = targetTransform.position + offset;
        }
        
    }
}
