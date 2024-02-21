using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultScript : MonoBehaviour
{
    [Header("References")]
    public bool active;
    [Header("Variables")]
    [SerializeField] private float launchForce;
    [SerializeField] private Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            Debug.Log("EEEEEEEEEE");
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Familiar"))
        {
            collider.gameObject.transform.position = transform.position;
            MovementScript movementScript = collider.GetComponent<MovementScript>();
            movementScript.enabled = false;
            active = true;
            StartCoroutine(PrepareToLaunch(collider.gameObject));
        }
    }

    IEnumerator PrepareToLaunch(GameObject gameObject)
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("GUH");
        MovementScript movementScript = gameObject.GetComponent<MovementScript>();
        Launch(movementScript);
    }

    private void Launch(MovementScript movementScript)
    {
        movementScript.enabled = true;
        movementScript.canMove = false;
        Vector3 launchVelocity = transform.rotation * direction.normalized * 25f;
        movementScript.ChangeVelocity(launchVelocity);
        Debug.Log(launchVelocity);
    }

    IEnumerator RemoveLaunchForce(MovementScript movementScript)
    {
        yield return new WaitForSeconds(5f);
        movementScript.ChangeVelocity(new Vector3(0f, 0f, 0f));
    }

    void OnDrawGizmos()
    {
        DrawArrow.ForGizmo(transform.position, transform.rotation * direction); 
    }
}
