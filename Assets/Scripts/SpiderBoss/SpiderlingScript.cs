using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderlingScript : MonoBehaviour
{
    [SerializeField] private GameObject arachnophobiaSpider;
    [SerializeField] private GameObject defaultSpider;
    private bool arachnophobiaOn;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneHandler.instance.arachnophobiaState) {
            arachnophobiaSpider.SetActive(true);
            defaultSpider.SetActive(false);
            arachnophobiaOn = false;
        }
        else {
            arachnophobiaSpider.SetActive(false);
            defaultSpider.SetActive(true);
            arachnophobiaOn = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleArachnophobia(bool toggleOn) {
        if (toggleOn) {
            arachnophobiaSpider.SetActive(true);
            defaultSpider.SetActive(false);
            arachnophobiaOn = true;
        }
        else {
            arachnophobiaSpider.SetActive(false);
            defaultSpider.SetActive(true);
            arachnophobiaOn = false;
        }
    }
}
