using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnify : MonoBehaviour
{
    private Renderer rend;
    private Camera mainCamera;

    void Start()
    {
        rend = GetComponent<Renderer>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(transform.position);
        screenPoint.x = screenPoint.x / Screen.width;
        screenPoint.y = screenPoint.y / Screen.height;
        rend.material.SetVector("_ObjectScreenPos", screenPoint);
    }
}
