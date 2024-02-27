using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlueprintScript : MonoBehaviour
{

    [Header("Current Crafting Ingredients")]
    //[SerializeField] public List<List<GameObject>> SCRONK = new List<List<GameObject>>();
    [SerializeField] private List<CraftableScript.Craftable> currentCraftingIngredients = new List<CraftableScript.Craftable>(); 
    [Header("Required Craftables")]
    [SerializeField] private List<CraftableScript.Craftable> requiredCraftables = new List<CraftableScript.Craftable>(); 
    [Header("Crafting Result")]
    [SerializeField] private List<GameObject> dummyCraftingResults = new List<GameObject>();
    [SerializeField] private GameObject craftingResult;

    void Start()
    {
        var scripts = craftingResult.GetComponentsInChildren(typeof(MonoBehaviour)); // get all scripts on object(s) we're gonna craft
        for (int i = 0; i < scripts.Length; i++)  
        {
            Debug.Log(scripts[i]);
            Behaviour bhvr = (Behaviour)scripts[i];
            bhvr.enabled = false;
        }
        foreach(GameObject dummyCraftingResult in dummyCraftingResults)
        {
            dummyCraftingResult.SetActive(false);
        }
        craftingResult.SetActive(false);
    }

    void Update()
    {
    }

    void OnTriggerEnter(Collider collider)
    {
        CraftableScript craftableScript = collider.gameObject.GetComponent<CraftableScript>(); // get the info from the craftable that just entered
        if (craftableScript != null)
        {
            if (!currentCraftingIngredients.Contains(craftableScript.GetCraftableName())) // make sure no dupes!
            {
                currentCraftingIngredients.Add(craftableScript.GetCraftableName()); // add craftable to list
                // instantiate VFX here!
                Destroy(craftableScript.gameObject); // destroy craftable
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
            //Debug.Log("All ingredients here yay! Let's build!");
            CraftObject();
        } 
    }

    private void EnableDummyParts(int i)
    {
        dummyCraftingResults[i].SetActive(true);
    }

    private void CraftObject()
    {
        // instantiate VFX here!
        craftingResult.SetActive(true); // make crafted object visible
    }
}
