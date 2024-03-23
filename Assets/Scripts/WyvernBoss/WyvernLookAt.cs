using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WyvernLookAt : MonoBehaviour
{
    public bool cameraIsActive = false;
    private float dampTime = 3f;
    private GameObject player;
    private GameObject wyvern;

    void Start()
    {
        player = this.transform.parent.gameObject;
    }

    void LateUpdate()
    {
        if (cameraIsActive) // ensures it only runs while player needs it 
        {
            if (wyvern == null)
            {
                wyvern = GameObject.Find("WyvernBoss");
            }

            if (wyvern != null) // catches if wyvern doesnt exist in the scene
            {
                Vector3 wyvernPos = wyvern.transform.position;
                Vector3 playerPos = player.transform.position;

                wyvernPos.y = 0f;
                playerPos.y = 0f;

                Vector3 direction = (wyvernPos - playerPos).normalized;
                direction.y = 0f;

                Vector3 camPos = player.transform.position + (direction * 15f);
                camPos.y = 13f;

                Vector3 zero = Vector3.zero;
                transform.position = Vector3.SmoothDamp(transform.position, camPos, ref zero, dampTime);
                transform.GetChild(0).LookAt(wyvern.transform.position);
            }
        }
    }

    private void TriggerCamera()
    {

    }
}
