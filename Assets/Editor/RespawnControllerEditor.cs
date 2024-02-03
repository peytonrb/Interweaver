using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using log4net.Util;

[CustomEditor(typeof(RespawnController))]
[CanEditMultipleObjects]
public class WevebleRespawnEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RespawnController respawnController = (RespawnController)target;

        if (GUILayout.Button("Button for getting the weaveables in the list"))
        {
            AddObjectFromBoxCast(respawnController);
        }
       
       
    }

    private void AddObjectFromBoxCast(RespawnController respawnController)
    {
        var boxCastParameters = respawnController.boxCastHalfExtent;
        RaycastHit[] hits = Physics.BoxCastAll(
            respawnController.transform.position, 
            boxCastParameters, 
            respawnController.transform.up, 
            respawnController.transform.rotation, 
            respawnController.layersToCheck);
        respawnController.myRespawnables.Clear();

        foreach (var hit in hits) 
        {
            if (!hit.collider.CompareTag("FloatingIsland") && hit.collider.GetComponent<WeaveableNew>() != null)
            {
                respawnController.myRespawnables.Add(hit.collider.gameObject);
            }
        }
    }
}
