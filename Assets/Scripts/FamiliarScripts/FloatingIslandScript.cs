using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Assertions.Must;

public class FloatingIslandScript : MonoBehaviour
{
   
    //Attachments
    [Header("Prereqs")]
    public CrystalScript myCrystal;
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


    [Header("Don't Touch!")]
    public float verticalOffset = 0;

    [CannotBeNullObjectField]
    public GameObject crystalPrefab;
    [CannotBeNullObjectField]
    public GameObject vcamPrefab;
    public bool cameraswitched;
    public bool isislandfalling;
    public Rigidbody rb;
    private Animator anim;

    private void Awake()
    {
        //Assign Components
        anim = GetComponent<Animator>();

        //if it has a crystal, it'll auto assign the itself to the crystals variable
        if (myCrystal != null)
        {
            myCrystal.AssignFloatingIsland(this);
        }

        //Set Variables
        cameraswitched = false;
        isislandfalling = false;

        //Set Animator
        if (isFloating)
        {
            anim.SetTrigger("Float");
        }
        else
        {
            anim.SetTrigger("Sit");
        }
    }

    void Update()
    {
        // Vertical Offset is set in the animations, this bit makes it actually fall
       if (verticalOffset != 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + verticalOffset, transform.position.z);
        }
    }

    public void AssignNewCrystal(CrystalScript crystal)
    {

        myCrystal = crystal;
   
        crystal.AssignFloatingIsland(this);

    }
    
    //Called by the crystal, changes animation state, starts timer before respawning at sit location
    public void StartFalling() 
    {
        if (isFloating)
        {
            anim.SetTrigger("Fall");
            StartCoroutine(TimerBeforeRespawn(true));
            cameraswitched = false;
            myCrystal.GetComponent<WeaveableNew>().canBeRelocated = true;
        }
    }

    //Coroutine timer for respawning the object at the float and sit locations
    public IEnumerator TimerBeforeRespawn(bool isFalling)
    {
        yield return new WaitForSeconds(timerBeforeSwap / 2);

        if (!isFalling)
        {
            CameraMasterScript.instance.FloatingIslandCameraReturn(myFloatCamera);
        }
        

        yield return new WaitForSeconds(timerBeforeSwap /2);

        if (isFalling)
        {
            anim.SetTrigger("Sit");
            verticalOffset = 0;
            transform.position = sitTransform.position;
            isFloating = false;
        }
        else
        {
            anim.SetTrigger("Float");
            verticalOffset = 0;
            transform.position = floatTransform.position;
            isFloating = true;
        }
        
        yield break;
    }

    //Swaps to the rising camera when woven to, called by the IslandSwap WeaveInteraction
    public void SwapToRiseCamera()
    {
        if (!isFloating)
        {
            //Camera is switched to a new view which watches the whole island rise. (Lasts about 2 seconds)
            if (cameraswitched == false)
            {
                CameraMasterScript.instance.FloatingIslandCameraSwitch(myFloatCamera, this);
            }
        }
    }

    //changes the animation and starts the respawn at float spot timer
    public void RaiseIsland()
    {
        StartCoroutine(TimerBeforeRespawn(false));
        cameraswitched = false;
        anim.SetTrigger("Rise");
    }

#if UNITY_EDITOR
    public void SpawnTransforms()
    {
        //spawn Transforms
        GameObject sitObj = new GameObject();
        GameObject floatObj = new GameObject();

        //assign names
        sitObj.name = "sitTransform";
        floatObj.name = "floatTransform";

        //move locations
        sitObj.transform.position = new Vector3(transform.position.x, transform.position.y - 10, transform.position.z);
        floatObj.transform.position = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z);

        //assign transforms
        floatTransform = floatObj.transform;
        sitTransform = sitObj.transform;
    }

    public void SpawnCrystal()
    {
        GameObject crystal = Instantiate(crystalPrefab);

        myCrystal = crystal.GetComponent<CrystalScript>();

        crystal.transform.position = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
    }

    public void SpawnCamera()
    {
        GameObject cam = Instantiate(vcamPrefab);

        cam.transform.position = new Vector3(transform.position.x - 10, transform.position.y + 10, transform.position.z);

        myFloatCamera = cam.GetComponent<CinemachineVirtualCamera>();
    }
#endif

}
