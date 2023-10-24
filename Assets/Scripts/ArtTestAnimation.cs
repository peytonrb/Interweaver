using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Cash wrote this to make objects in art test scene rotate >:)

public class ArtTestAnimation : MonoBehaviour
{
    [SerializeField]
    float rotateSpeed;
    [SerializeField]
    Vector3 rotationDirection = new Vector3();

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotateSpeed * rotationDirection * Time.deltaTime);

    }
}
