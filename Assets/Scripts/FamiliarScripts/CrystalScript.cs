using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalScript : MonoBehaviour
{
    public FloatingIslandScript myFloatingIsland;

    public bool startsWeaveable = false;

    private Vector3 startPos;

    private WeaveController weaveController;


    public void Start()
    {
        weaveController = InputManagerScript.instance.player.GetComponent<WeaveController>();

        if (startsWeaveable)
        {
            startPos = transform.position;
        }
    }

    public void ResetCrystal()
    {
        gameObject.SetActive(false);
        transform.position = startPos;
        StartCoroutine(ResetWeave());
    }

    public IEnumerator ResetWeave()
    {
        WeaveableManager.Instance.DestroyJoints(weaveController.currentWeaveable.listIndex);

        yield return new WaitForSeconds(0.1f);

        weaveController.OnDrop();

        yield break;
    }

    

}
