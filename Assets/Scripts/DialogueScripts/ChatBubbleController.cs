using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatBubbleController : MonoBehaviour
{
    private SpriteRenderer background;
    private TextMeshPro dialogueText;

    private void Awake()
    {
        background = gameObject.transform.Find("Background").GetComponent<SpriteRenderer>();
        dialogueText = gameObject.transform.Find("Dialogue").GetComponent<TextMeshPro>();
    }

    void Start()
    {
        initializeChatBubble("hello world");
    }

    private void initializeChatBubble(string text)
    {
        dialogueText.SetText(text);
        dialogueText.ForceMeshUpdate();
        Vector2 textSize = dialogueText.GetRenderedValues(false);
    }
}
