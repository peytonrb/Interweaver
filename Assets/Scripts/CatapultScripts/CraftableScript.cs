using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftableScript : MonoBehaviour
{
    public enum Craftable
    {
        CatapultBase,
        CatapultArm,
        CatapultBowl
    }
    [Header("References")]
    [Header("Variables")]
    [SerializeField] private Craftable craftableName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
