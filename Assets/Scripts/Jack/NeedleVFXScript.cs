using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class NeedleVFXScript : MonoBehaviour
{

    public VisualEffect vfx;
    public GameObject impactEffect;
    // Start is called before the first frame update
    void Start()
    {
        vfx.SendEvent("OnPlay");
        StartCoroutine(DestroyDelay());
    }

    public IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(4f);


        Destroy(gameObject);

        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().AddForce(transform.up * 10, ForceMode.Force);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("weaveObject"))
        {
            Instantiate(impactEffect);
            Destroy(gameObject);
        }
    }
}
