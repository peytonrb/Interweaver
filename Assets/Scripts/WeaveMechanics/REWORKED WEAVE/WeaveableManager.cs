using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaveableManager : MonoBehaviour
{
    public List<weaveableGroup> combinedWeaveables;
    public static WeaveableManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        combinedWeaveables = new List<weaveableGroup>();
    }

    // destroys the fixed joints, resets weaveables to original state, clears the weaveables from the list,
    //      deletes the list from parent list
    // <param> the index of the list to be deleted in the parent list
    public void DestroyJoints(int listIndex)
    {
        for (int i = 0; i < combinedWeaveables[listIndex].weaveableObjectGroup.Count; i++)
        {
            // its possible that there may be multiple fixed joints per gameobject
            FixedJoint[] joints = combinedWeaveables[listIndex].weaveableObjectGroup[i].GetComponents<FixedJoint>();
            foreach (FixedJoint joint in joints)
            {
                Destroy(joint);
            }

            combinedWeaveables[listIndex].weaveableObjectGroup[i].ResetWeaveable();
            StartCoroutine(WaitForFunction(listIndex, i));
        }

        combinedWeaveables[listIndex].weaveableObjectGroup.Clear();
        RemoveList(listIndex);
    }

    // waits for ResetWeaveable() to occur before setting variable to false for combine logic
    IEnumerator WaitForFunction(int listIndex, int i)
    {
        yield return null;
        combinedWeaveables[listIndex].weaveableObjectGroup[i].hasBeenCombined = false;
    }

    // adds weaveable to new or existing list depending on combined status (wip)
    // <param> the index of the list in parent list and the weaveable itself
    // <returns> the current length of the internal list, and whether or not this is happening in response to combining
    public Vector2 AddWeaveableToList(WeaveableObject weaveable)
    {
        int listIndex = 0;

        // determines which list Weaveable should be inserted into
        if (weaveable != null)
        {
            listIndex = combinedWeaveables.Count;
            combinedWeaveables.Add(new weaveableGroup());
            combinedWeaveables[listIndex].weaveableObjectGroup.Add(weaveable);
        }

        return new Vector2(listIndex, combinedWeaveables[listIndex].weaveableObjectGroup.Count - 1);
    }

    // overload of previous method, receives index for combining into proper list
    public Vector2 AddWeaveableToList(WeaveableObject weaveable, int index)
    {
        // determines which sublist Weaveable should be inserted into
        if (weaveable != null && !combinedWeaveables[index].weaveableObjectGroup.Contains(weaveable))
        {
            combinedWeaveables[index].weaveableObjectGroup.Add(weaveable);
            weaveable.ID = combinedWeaveables[index].weaveableObjectGroup.Count - 1;

            // ensures ID does not go out of bounds
            if (weaveable.ID <= 0)
            {
                weaveable.ID = 0;
            }
        }

        return new Vector2(index, combinedWeaveables[index].weaveableObjectGroup.Count - 1);
    }

    // removes a weaveable from a list
    // <param> the index of the list in parent list and the index of weaveable in internal list
    public void RemoveWeaveableFromList(int listIndex, int ID)
    {
        if (combinedWeaveables[listIndex] != null)
        {
            // deletes specific ID
            if (combinedWeaveables[listIndex].weaveableObjectGroup[ID] != null)
            {
                combinedWeaveables[listIndex].weaveableObjectGroup.RemoveAt(ID);
            }

            // deletes array index as a whole if list is empty
            if (combinedWeaveables[listIndex].weaveableObjectGroup.Count <= 0)
            {
                combinedWeaveables.RemoveAt(listIndex);
            }
        }
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
[System.Serializable]
public class weaveableGroup
{
    public List<WeaveableObject> weaveableObjectGroup = new List<WeaveableObject>();
}