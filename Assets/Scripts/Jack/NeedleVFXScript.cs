using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class NeedleVFXScript : MonoBehaviour
{

    public VisualEffect vfx;
    public GameObject impactEffect;
    private GameObject myWeaveable;
    // Start is called before the first frame update
    void Start()
    {
        vfx.SendEvent("OnPlay");
        
    }

    public void SetDestroyTimer(float time, GameObject weaveable)
    {
        myWeaveable = weaveable;
        StartCoroutine(DestroyDelay(time));
    }

    public IEnumerator DestroyDelay(float time)
    {
        yield return new WaitForSeconds(time);

        Instantiate(impactEffect, myWeaveable.transform.position, transform.rotation);
        Destroy(gameObject);

        yield break;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weaveable"))
        {
            Instantiate(impactEffect, myWeaveable.transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().AddForce(transform.up * 50, ForceMode.Force);
    }

    
}
