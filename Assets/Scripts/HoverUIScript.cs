using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverUIScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public GameObject myLostSoulCount;
    private CanvasGroup canvasGroup;

    void Start() 
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0;
    }

    public void OnSelect(BaseEventData data) {
        canvasGroup.alpha = 1;
    }

    public void OnDeselect(BaseEventData data) {
        canvasGroup.alpha = 0;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        myLostSoulCount.SetActive(true);
        canvasGroup.alpha = 1;
    }

    public void OnPointerExit(PointerEventData eventData) {
        myLostSoulCount.SetActive(false);
        canvasGroup.alpha = 0;
    }
}
