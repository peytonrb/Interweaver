using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverUIScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject myLostSoulCount;

    public void OnPointerEnter(PointerEventData eventData) {
        myLostSoulCount.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        myLostSoulCount.SetActive(false);
    }
}
