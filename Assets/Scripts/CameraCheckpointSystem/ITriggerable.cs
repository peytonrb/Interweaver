using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITriggerable
{
    void OnTrigEnter(Collider collider);

    void OnTrigExit(Collider collider);
}
