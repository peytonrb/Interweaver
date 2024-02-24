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
    [SerializeField] private GameObject craftingResult;

    void Start()
    {
        craftingResult.SetActive(false); 
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
        Debug.Log("Length of current ingredients: " + currentCraftingIngredients.Count);
        Debug.Log("Length of current ingredients: " + requiredCraftables.Count);
        foreach(CraftableScript.Craftable craftableName in currentCraftingIngredients)
        {
            if (requiredCraftables.Contains(craftableName))
            {
                needededIngredients++;
            }
        }

        if (needededIngredients >= requiredCraftables.Count)
        {
            Debug.Log("All ingredients here yay! Let's build!");
            CraftObject();
        } 
    }

    private void CraftObject()
    {
        // instantiate VFX here!
        craftingResult.SetActive(true); // make crafted object visible
    }
}
