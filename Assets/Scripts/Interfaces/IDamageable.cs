using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void Damage();
}
//use this example code as a template to use the interface

#region//FOR TRIGGERS
//if you're going to use a trigger for the interface try this code:

//void OnTriggerEnter(Collider other)
//{
//    if (other.gameObject.TryGetComponent(out IDamageable damageObject))
//    {
//        damageObject.Damage();
//    }
//}


//Other script: 
//public class Whatever the class is called : MonoBehavior, IDamageable <--this goes wherever you see the MonoBehavior of a script because you're 
//establishing a contract between the two scripts

//public void Damage
//{
//  your method here    
//}
#endregion
