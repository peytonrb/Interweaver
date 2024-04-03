using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BouncyCrystalController : MonoBehaviour
{
    private BouncyCrystalsScript bouncyCrystalsScript;
    private float timer;
    private float officalTimer;
    private bool respawnTimerOn;
    private GameObject bouncyCrystals;
    private GameObject inActiveBouncyCrystals;
    [System.Serializable]
    public struct BouncyCrystalData
    {
        public GameObject realBouncyCrystals;
        public float realBouncyCrystalTimer;
    }
    public List<BouncyCrystalData> bouncyCrystalList = new List<BouncyCrystalData>();
    // Start is called before the first frame update
    void Start()
    {
        respawnTimerOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        BatchCrystalActive();

        if (respawnTimerOn) 
        {
            timer -= Time.deltaTime;           
            if (timer <= 0)
            {
                
                respawnTimerOn = false;
                inActiveBouncyCrystals.SetActive(true);
            }
        }

    }
   

    void BatchCrystalActive()
    {        
        for (int i = 0; i < bouncyCrystalList.Count; i++)
        {
            bouncyCrystals = bouncyCrystalList[i].realBouncyCrystals;

            officalTimer = bouncyCrystalList[i].realBouncyCrystalTimer;

            bouncyCrystalsScript = bouncyCrystals.GetComponent<BouncyCrystalsScript>();

            if (!bouncyCrystals.activeInHierarchy && !respawnTimerOn)
            {               
                respawnTimerOn = true;               
                timer = officalTimer;
                inActiveBouncyCrystals = bouncyCrystals;
            }           
            
        }
    }
   
}
