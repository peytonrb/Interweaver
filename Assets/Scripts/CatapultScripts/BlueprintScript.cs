using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlueprintScript : MonoBehaviour
{

    [Header("Current Crafting Ingredients")]
    private List<CraftableScript.Craftable> currentCraftingIngredients = new List<CraftableScript.Craftable>(); 
    [Header("Required Craftables")]
    [SerializeField] private List<CraftableScript.Craftable> requiredCraftables = new List<CraftableScript.Craftable>(); 
    [Header("Crafting Result")]
    [SerializeField] private GameObject craftingResult;
    [Header("Dummy")]
    [SerializeField] private List<GameObject> dummyCraftingResults = new List<GameObject>(); // dummy's represent the object we're making while not being that object
    private List<Material> dummyCraftingResultsOriginalMaterials = new List<Material>();
    [SerializeField] private Material dummyMaterial; 
    [SerializeField] private GameObject grunk; 
    // general premise of this is we have three fundamental lists, currentCraftingIngredients, which should start empty and gets filled with craftable names
    // as stuff enters the trigger zone. Then, once everything in currentCraftingIngredients and requiredCraftables (which should contain the craftable
    // names that we want) are equal, we activate the craftingResult gameobject. The final key list, dummyCraftingResults is meant to simulate the craftingResult
    // being built. It's a list of gameobjects that SHOULD have equivilant indexes and models to the requiredCraftables 
    // list (if catapult arm is at index 1 in requiredCraftables, the dummy object which represents the arm should be at index 1 in dummyCraftingResults).
    // dummyCraftingResults starts active, and has it material stripped.
    // As the equivilant indexes fill up in requiredCraftables, we re-apply their original materials. When it's all filled up, we delete all the 
    // gameobjects in dummyCraftingResults
    // hope this is comprehensible :)

    [Header("Audio")]
    [SerializeField] private AudioClip buildSound;

    void Start()
    {
        foreach(GameObject dummyCraftingResult in dummyCraftingResults)
        {
            dummyCraftingResultsOriginalMaterials.Add(dummyCraftingResult.GetComponent<Renderer>().material);
            dummyCraftingResult.GetComponent<Renderer>().material = dummyMaterial; // set dummy materials to an 'unbuilt' material
            dummyCraftingResult.transform.parent = null; // unparent all them. Why? Because Unity is fucking weird
        }
        craftingResult.SetActive(false);
        grunk.SetActive(false);
    }

    void OnTriggerEnter(Collider collider)
    {
        CraftableScript craftableScript = collider.gameObject.GetComponent<CraftableScript>(); // get the info from the craftable that just entered
        if (craftableScript != null)
        {
            if (!currentCraftingIngredients.Contains(craftableScript.GetCraftableName())) // make sure no dupes!
            {
                currentCraftingIngredients.Add(craftableScript.GetCraftableName());
                // instantiate VFX here!
                AudioManager.instance.PlaySound(AudioManagerChannels.SoundEffectChannel, buildSound, 1f);
                Destroy(craftableScript.gameObject);
                CheckIfCanCraft(); // check if we're good to go on crafting
            }
        }
    }

    private void CheckIfCanCraft()
    {
        int needededIngredients = 0;
        foreach(CraftableScript.Craftable craftableName in currentCraftingIngredients)
        {
            if (requiredCraftables.Contains(craftableName))
            {
                EnableDummyParts(requiredCraftables.IndexOf(craftableName));
                needededIngredients++;
            }
        }

        if (needededIngredients >= requiredCraftables.Count)
        {
            CraftObject();
        } 
    }

    private void EnableDummyParts(int i)
    {
        dummyCraftingResults[i].GetComponent<Renderer>().material = dummyCraftingResultsOriginalMaterials[i]; // re-apply original material
    }

    private void CraftObject()
    {
        // instantiate VFX here!
        foreach(GameObject dummyCraftingResult in dummyCraftingResults) 
        {
            Destroy(dummyCraftingResult); // dummies are unneeded now, get rid of em'
        }
        grunk.SetActive(true);
        ParticleSystem guh = grunk.GetComponent<ParticleSystem>();
        guh.Play();
        craftingResult.SetActive(true); // make crafted object visible
    }
}
