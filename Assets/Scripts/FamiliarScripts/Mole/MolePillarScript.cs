using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MolePillarScript : MonoBehaviour
{
    [Header("References")]
    private MovementScript movementScript;
    private FamiliarScript familiarScript;
    private MoleDigScript moleDigScript;
    private AudioManager audioManager;
    [SerializeField] private GameObject dirtPillar;
    public List<GameObject> pillarList = new List<GameObject>();
    [HideInInspector] public CinemachineVirtualCamera familiarCamera;
    private GameObject newPillar;

    [Header("Variables")]
    [SerializeField] [Range (1, 10)] private int maxPillarCount = 1;
    [SerializeField] [Range (10f, 50)] private float maxPillarHeight = 25f;
    [SerializeField] private float pillarBuildSpeed = 1.5f;
    private Vector3 pointToRiseTo = Vector3.up;
    private float distance = -1f;
    [SerializeField] [Range (0.1f, 4f)] private float pillarInteractionRadius = 1f; // the radius at which the mole can affect pillars around them
    private bool pillarRising;
    private bool pillarLowering;
    [HideInInspector] public bool riseInputPressed;
    [HideInInspector] public bool lowerInputPressed;
    [HideInInspector] public bool rise;
    [HideInInspector] public bool lower;

    [Header("Audio")]
    [SerializeField] AudioClip pillarBuildSound;
    private AudioSource pillarBuildAudioSource;

    [Header("Utility")]
    [SerializeField] private bool showHeightGizmo = true;
    [SerializeField] private bool showInteractionRadiusGizmo = true;

    void Start()
    {
        movementScript = GetComponent<MovementScript>();
        moleDigScript = GetComponent<MoleDigScript>();
        familiarScript = GetComponent<FamiliarScript>();
        audioManager = GameObject.FindGameObjectWithTag("Audio Manager").GetComponent<AudioManager>();
        familiarCamera = GameObject.FindGameObjectWithTag("FamiliarCamera").GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rise) // this is bad. I know this is bad. Sorry.
        {
            RaisePillar();
        }

        if (lower) // I knooooow. I'm sorryyyyy
        {
            pillarRising = false;
            rise = false; // lowering should override building methinks
            LowerPillar();
        }
    }

    public void DeployPillar()
    {
        if (moleDigScript.borrowed && SearchForNearbyPillars() == null) // can only deploy while burrowing
        {

            if (pillarList.Count() >= maxPillarCount) // destroy earliest pillar if we're at cap
            {
                DestroyPillar();
            }

            newPillar = Instantiate(dirtPillar);
            newPillar.transform.position = transform.position;
            pillarList.Add(newPillar);
            moleDigScript.MakePillarsDiggable();
        }
    }

    private void DestroyPillar()
    {
        Destroy(pillarList[0]);
        pillarList.RemoveAt(0);
    }

    public void RaisePillar()
    {
        if (!riseInputPressed || !moleDigScript.borrowed || !familiarScript.myTurn) // if input isn't currently being pressed or we've stopped digging for whatever reason
        {
            PillarRiseEnd();
        }
        else if (pillarList.Count > 0)
        {
            GameObject pillarToRaise = SearchForNearbyPillars();

            if (pillarToRaise != null)
            {
                if (pillarToRaise.transform.parent != null)
                {
                    pillarToRaise = pillarToRaise.transform.parent.gameObject; // if we have a top, get it
                }
                distance = Vector3.Distance(pillarToRaise.transform.position, pointToRiseTo);
                if (!pillarRising) // right as we start things off
                {
                    //pillarBuildAudioSource = audioManager.AddSFX(pillarBuildSound, true, pillarBuildAudioSource);
                    pillarLowering = false;
                    movementScript.ZeroCurrentSpeed(); // we do this to prevent sudden jarring movement after movement script is re-enabled
                    movementScript.enabled = false; // disable player movement
                    pointToRiseTo = new Vector3 (pillarToRaise.transform.position.x, transform.position.y, 
                    pillarToRaise.transform.position.z) + (Vector3.up * maxPillarHeight); // set a destination for the pillar to rise
                    pillarRising = true; // mark that pillar has started rising
                    familiarCamera.Follow = pillarToRaise.transform; // set camera to pillar top
                }
                else if (distance > 0.1f && pillarRising) // if pillar hasn't quite reached destination, and we're still meant to be rising
                {
                    pillarToRaise.transform.position = Vector3.MoveTowards(pillarToRaise.transform.position, 
                    pointToRiseTo, pillarBuildSpeed * Time.deltaTime);
                }
                else // once pillar has reached its destination or we've decided it needs to stop rising through other means
                {
                    PillarRiseEnd();
                }
            }
        }
    }

    public void LowerPillar()
    {
        if (!lowerInputPressed || !moleDigScript.borrowed || !familiarScript.myTurn)
        {
            PillarLowerEnd();
        }
        else
        {
            GameObject pillarToLower = SearchForNearbyPillars();
            
            if (pillarToLower != null)
            {
                if (pillarToLower.transform.parent != null)
                {
                    pillarToLower = pillarToLower.transform.parent.gameObject; // if we have a top, get it
                }
                if (!pillarLowering) // right as we start things off
                {
                    //pillarBuildAudioSource = audioManager.AddSFX(pillarBuildSound, true, pillarBuildAudioSource);
                    movementScript.ZeroCurrentSpeed(); // we do this to prevent sudden jarring movement after movement script is re-enabled
                    movementScript.enabled = false; // disable player movement
                    pillarLowering = true;
                    familiarCamera.Follow = pillarToLower.transform; // set camera to pillar top
                }
                pillarToLower.transform.position = Vector3.MoveTowards(pillarToLower.transform.position, 
                pillarToLower.transform.position - transform.up, pillarBuildSpeed * Time.deltaTime);
            }
            else
            {
                PillarLowerEnd();
            }
        }
    }

    public void PillarRiseEnd()
    {
        //pillarBuildAudioSource = audioManager.KillAudioSource(pillarBuildAudioSource);
        if (moleDigScript.startedToDig)
        {
            movementScript.enabled = true;
        }
        rise = false;
        pillarRising = false;
        familiarCamera.Follow = transform;
    }

    public void PillarLowerEnd()
    {
        //pillarBuildAudioSource = audioManager.KillAudioSource(pillarBuildAudioSource);
        if (moleDigScript.startedToDig)
        {
            movementScript.enabled = true;
        }
        lower = false;
        pillarLowering = false;
        familiarCamera.Follow = transform;
    }

    public GameObject SearchForNearbyPillars()
    {
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, pillarInteractionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Dirt Pillar"))
            {
               return hitCollider.gameObject;
            }
        }
        return null;
    }

    void OnDrawGizmos()
    {
        if (showHeightGizmo)
        {
            if (!pillarRising)
            {
                DrawArrow.ForGizmo(transform.position, Vector3.up * maxPillarHeight);
            }
            else
            {
                DrawArrow.ForGizmo(transform.position, Vector3.up * distance);
            }
        }
        if (showInteractionRadiusGizmo)
        {
            Gizmos.DrawWireSphere(transform.position, pillarInteractionRadius);
        }
    }
}
