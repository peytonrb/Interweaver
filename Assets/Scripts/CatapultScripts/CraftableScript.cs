using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftableScript : MonoBehaviour
{
    public enum Craftable
    {
        CatapultBody,
        CatapultArm,
        CatapultBowl
    }
    [Header("References")]
    [Header("Variables")]
    [SerializeField] private Craftable craftableName;
    [SerializeField] private List<Craftable> compatibleCraftables = new List<Craftable>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
