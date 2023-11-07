using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewDialogue", menuName = "CustomScriptableObjects/Dialogue")]
public class Dialogue : ScriptableObject
{
    public string name;

    [TextArea(3,10)]
    public List<string> sentences;
    public AudioClip speechFile;
}