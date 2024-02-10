using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaveableManager : MonoBehaviour
{
    public List<weaveableGroup> combinedWeaveables;

    void Start()
    {
        combinedWeaveables = new List<weaveableGroup>();
    }

    // adds weaveable to new or existing list depending on combined status (wip)
    // <param> the index of the list in parent list and the weaveable itself
    // <returns> the current length of the internal list
    public int AddWeaveableToList(int listIndex, GameObject weaveable)
    {
        if (weaveable.GetComponent<WeaveableObject>() != null)
            combinedWeaveables[listIndex].weaveableObjectGroup.Add(weaveable.GetComponent<WeaveableObject>());

        return combinedWeaveables[listIndex].weaveableObjectGroup.Count;
    }

    // removes a weaveable from a list
    // <param> the index of the list in parent list and the index of weaveable in internal list
    public void RemoveWeaveableFromList(int listIndex, int ID)
    {
        combinedWeaveables[listIndex].weaveableObjectGroup.RemoveAt(ID);
    }

    // helper function - adds a List to the combinedWeaveables List
    // <returns> the current length of the ArrayList
    public int AddList()
    {
        combinedWeaveables.Add(new weaveableGroup());
        return combinedWeaveables.Count;
    }

    // helper function - removes a list from the combinedWeaveables list at a specified index
    // <param> the index of the list to remove in parent list
    public void RemoveList(int listIndex)
    {
        combinedWeaveables.RemoveAt(listIndex);
    }
}

// this class defines the nested ArrayList to be Serializable, which would otherwise prevent it from
//      being shown in the inspector
[System.Serializable] public class weaveableGroup
{
    public List<WeaveableObject> weaveableObjectGroup;
}