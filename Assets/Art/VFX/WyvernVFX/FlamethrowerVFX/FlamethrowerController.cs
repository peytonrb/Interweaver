using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class FlamethrowerController : MonoBehaviour
{
    public bool isActive;
    private VisualEffect flamethrowerVFX;
    private ParticleSystem flamethrowerPS;
    private Vector3 origin;
    private CapsuleCollider capsuleCollider;

    [Header("Audio")]
    [SerializeField] private AudioClip  flamethrowerSound;

    private void Start()
    {
        origin = this.transform.position;
        flamethrowerVFX = this.transform.GetChild(0).GetComponent<VisualEffect>();
        flamethrowerPS = this.transform.GetChild(1).GetComponent<ParticleSystem>();
        capsuleCollider = gameObject.GetComponent<CapsuleCollider>();

        capsuleCollider.enabled = false;
    }

    private void Update()
    {
        // if isActive is on and the VFX or PS is turned off, turn them on
        if (isActive && !flamethrowerVFX.gameObject.activeSelf || isActive && !flamethrowerPS.gameObject.activeSelf)
        {
            flamethrowerVFX.gameObject.SetActive(true);
            flamethrowerVFX.Play();
            flamethrowerPS.gameObject.SetActive(true);
            flamethrowerPS.Play();
            AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, flamethrowerSound, .7f);
        }
        // if isActive is off and the VFX or PS is turned on, turn them off
        else if (!isActive && flamethrowerVFX.gameObject.activeSelf || !isActive && flamethrowerPS.gameObject.activeSelf)
        {
            flamethrowerVFX.gameObject.SetActive(false);
            flamethrowerPS.gameObject.SetActive(false);
            capsuleCollider.enabled = false;
        }

        if (isActive)
        {
            RaycastHit hit;
            origin = this.transform.position;

            if (Physics.Raycast(origin, transform.TransformDirection(Vector3.forward), out hit, 100000f))
            {
                //Debug.DrawRay(origin, transform.TransformDirection(Vector3.forward) * 100,
                //              Color.red);
                Vector3 hitPosition = hit.point;
                float distance = Vector3.Distance(origin, hitPosition);
                flamethrowerVFX.SetFloat("BeamLength", distance / 2);
            }
            capsuleCollider.enabled = true;
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerControllerNew playerController = other.gameObject.GetComponent<PlayerControllerNew>();
            playerController.Death();
        }
        if (other.gameObject.CompareTag("Familiar"))
        {
            FamiliarScript familiarScript = other.gameObject.GetComponent<FamiliarScript>();
            familiarScript.Death();
        }
        Debug.Log("Hit");
    }

    /*
    private void OnParticleCollision(GameObject other) {
        if (other.CompareTag("Player")) {
            PlayerControllerNew playerController = other.GetComponent<PlayerControllerNew>();
            playerController.Death();
        }
        if (other.CompareTag("Familiar")) {
            FamiliarScript familiarScript = other.GetComponent<FamiliarScript>();
            familiarScript.Death();
        }
        Debug.Log("Collided");
    }
    */
}
