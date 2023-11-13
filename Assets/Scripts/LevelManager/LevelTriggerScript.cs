using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTriggerScript : MonoBehaviour
{
    
    [Tooltip ("0 is for Projectile Cannons, 1 is for Breakeable Spikes")]
    public int triggerType;
    public bool triggered;

}
