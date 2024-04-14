using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class FloatingIslandScript : MonoBehaviour
{

    [CannotBeNullObjectField]
    public GameObject myFakeCrystal;
    [CannotBeNullObjectField]
    public GameObject myRealCrystal;

    [CannotBeNullObjectField]
    public CinemachineVirtualCamera myFloatCamera;

    //Designer Variables
    [Header("Designer Variables")]
    [SerializeField] private bool isFloating = true;
    //public bool toggleTimer;
    [SerializeField] private float timerBeforeSwap;
    [CannotBeNullObjectField]
    [SerializeField] private Transform floatTransform;
    [CannotBeNullObjectField]
    [SerializeField] private Transform sitTransform;

    [SerializeField] private bool isFloatingIslandInTheTube;

    private bool animating = false;

    [CannotBeNullObjectField]
    public GameObject vcamPrefab;

    public bool cameraswitched;
    public bool isislandfalling;
    public Rigidbody rb;

    private WeaveableObject weaveable;

    public GameObject crystalWovenVFX;

    [SerializeField] private AudioClip fallClip;

    private Vector3 fallenVector;
    private Vector3 risenVector;

    private float animationSpeed = 1;

    private void Awake()
    {

        //Set Variables
        cameraswitched = false;
        isislandfalling = false;

        if (!isFloating)
        {
            myFakeCrystal.SetActive(false);
        }
    }

    //Called by the crystal, changes animation state, starts timer before respawning at sit location
    public void StartFalling()
    {
        if (isFloating)
        {
            StartCoroutine(TimerBeforeRespawn(true));

            cameraswitched = false;

            AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, fallClip);

            //Enable Real Crystal
            myRealCrystal.SetActive(true);

            //Animate Island
            AnimateIsland(true);
        }
    }

    //Swaps to the rising camera when woven to, called by the IslandSwap WeaveInteraction
    public void StartFloating(GameObject realCrystal)
    {
        StartCoroutine(WaitForSnap());

        //Assign Real Crystal, disable it and reset it!
        myRealCrystal = realCrystal;
        
        realCrystal.GetComponent<CrystalScript>().ResetCrystal();

        myFakeCrystal.SetActive(true);

        //Animate Island suspicious
        AnimateIsland(false);
    }

    // waits for crystal to reach snap point before starting to rise
    IEnumerator WaitForSnap()
    {
        // wait a single frame for functions to run
        yield return null;

        if (!isFloating)
        {
            //Camera is switched to a new view which watches the whole island rise. (Lasts about 2 seconds)
            if (cameraswitched == false)
            {
                Instantiate(crystalWovenVFX, transform.position + new Vector3(0, 9, 0), transform.rotation);
                CameraMasterScript.instance.FloatingIslandCameraSwitch(myFloatCamera, this);

                StartCoroutine(TimerBeforeRespawn(false));
                cameraswitched = true;
            }
        }
    }

    public void AnimateIsland(bool isFalling)
    {
        fallenVector = new Vector3(transform.parent.transform.position.x, transform.parent.transform.position.y - 125f, transform.parent.transform.position.z);
        risenVector = new Vector3(transform.parent.transform.position.x, transform.parent.transform.position.y + 125f, transform.parent.transform.position.z);
        //StartCoroutine(AnimationCoroutine(isFalling));
        animating = true;

    }

    public void FixedUpdate()
    {
        if (animating)
        {
            if (isFloating) //means its currently falling
            {
                transform.position = Vector3.MoveTowards(transform.position, fallenVector, animationSpeed);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, risenVector, animationSpeed);
            }
        }
    }


    public IEnumerator AnimationCoroutine(bool isFalling)
    {
        float duration = timerBeforeSwap;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;

            if (isFalling)
            {
                transform.position = Vector3.Lerp(transform.parent.transform.position, fallenVector, time / duration);
            }
            else // is floating up
            {
                transform.position = Vector3.Lerp(transform.parent.transform.position, risenVector, time / duration);
            }

            yield return null;
        }

        yield break;
       
    }

    //Coroutine timer for respawning the object at the float and sit locations FIX THIS UGLY SHIT
    public IEnumerator TimerBeforeRespawn(bool isFalling)
    {

        yield return new WaitForSeconds(timerBeforeSwap/2);


        if (!isFalling && !isFloatingIslandInTheTube)
        {
            CameraMasterScript.instance.FloatingIslandCameraReturn(myFloatCamera);
        }

        yield return new WaitForSeconds(timerBeforeSwap / 2);

        if (isFalling)
        {
            transform.parent.transform.position = sitTransform.position; // compensating for weaveable prefab structure
            transform.position = sitTransform.position;
            isFloating = false;

            if (isFloatingIslandInTheTube)
            {
                LevelManagerScript.instance.TurnOnOffSection(1);
            }
        }
        else
        {
            transform.parent.transform.position = floatTransform.position; // compensating for weaveable prefab structure
            transform.position = floatTransform.position;
            isFloating = true;
        }

        animating = false;

        yield break;
    }

#if UNITY_EDITOR

    public void SpawnCamera()
    {
        GameObject cam = Instantiate(vcamPrefab);

        cam.transform.position = new Vector3(transform.position.x - 10, transform.position.y + 10, transform.position.z);

        myFloatCamera = cam.GetComponent<CinemachineVirtualCamera>();
    }
#endif

}
