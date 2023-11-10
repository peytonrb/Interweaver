using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class HighlightFix : MonoBehaviour,IPointerEnterHandler, IDeselectHandler, IPointerClickHandler
{

    public Button resumeButton;

        public void OnPointerEnter(PointerEventData eventData)
        {
            //if (!EventSystem.current.alreadySelecting)
                //EventSystem.current.SetSelectedGameObject(this.gameObject);
        }
        
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            EventSystem.current.SetSelectedGameObject(null);
            StartCoroutine(WaitAndReselect());
        }

        public void OnDeselect(BaseEventData eventData)
        {
            this.GetComponent<Selectable>().OnPointerExit(null);
        }
        
        public IEnumerator WaitAndReselect()
        {
            yield return new WaitForEndOfFrame();
            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
            yield break;
        }
}
