using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaveableObject : MonoBehaviour
{
    // for WeaveableManager - protected so they can be accessed and visible in Inspector in Debug Mode 
    //      but hidden otherwise
    protected int listIndex { get; set; }
    protected int ID { get; set; }
}
