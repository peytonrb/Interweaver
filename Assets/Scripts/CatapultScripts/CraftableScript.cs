using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CraftableScript : MonoBehaviour
{
    public enum Craftable // IMPORTANT NOTE, IF YOU'RE ADDING TO THIS, DON'T ADD STUFF IN THE MIDDLE OR TOP, ADD STUFF AT THE BOTTOM. YIPPEE!
    {
        None,
        CatapultBody,
        CatapultArm,
        CatapultBowl,
        CatapultBodyAndArm,
        CatapultArmAndBowl,
    }
    [Header("References")]
    [Header("Variables")]
    [SerializeField] private Craftable craftableName;
    [SerializeField] private List<Craftable> compatibleCraftables = new List<Craftable>();
    [SerializeField] private List<GameObject> craftingResults = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        CraftableScript craftableScript = collision.gameObject.GetComponent<CraftableScript>();
        if (craftableScript != null)
        {
            if (compatibleCraftables.Contains(craftableScript.GetCraftableName())) // check if pieces are compatible
            {
                if (craftableName > craftableScript.GetCraftableName()) // check priority of pieces, this is so when we 'craft', we don't double up on the result
                {
                    int craftingIndex = compatibleCraftables.IndexOf(craftableScript.GetCraftableName()); // get index of what we're crafting with that's compatible
                    Debug.Log(craftingIndex);
                    Debug.Log(craftingResults[craftingIndex]);
                    GameObject newlyCraftedObject = Instantiate(craftingResults[craftingIndex]); // instantiate the crafted result via the index of the compatible craftable
                    newlyCraftedObject.transform.position = craftableScript.transform.position; // put it in the right position
                    Destroy(craftableScript.gameObject); // destroy object we're crafting with
                    Destroy(gameObject); // destroy self
                }
                else
                {

                }
            }
        }
    }

    public Craftable GetCraftableName()
    {
        return craftableName;
    }
}
