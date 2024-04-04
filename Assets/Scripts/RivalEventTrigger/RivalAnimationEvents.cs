using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RivalAnimationEvents : MonoBehaviour
{

    [CannotBeNullObjectField] public RivalEventTrigger rivalEventTrigger;
    public void DisappearEvent()
    {
        rivalEventTrigger.smoke.Play();
        rivalEventTrigger.rival.SetActive(false);
    }
}
