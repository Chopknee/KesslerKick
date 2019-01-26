using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteModeUI : MonoBehaviour
{

    public Text missesText;
    public Text hitsText;
    public ShipControls playerShip;
    public EarthTrigger planetEarth;

    public int hits = 0;
    public int misses = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (missesText == null || hitsText == null || playerShip == null || planetEarth == null) {
            Debug.Log("Missing some elements from the UI component, please check it!");
        }

        playerShip.OnGetKnockout += OnHitsIncrease;
        planetEarth.OnMiss += OnMissesIncrease;
    }


    public void OnHitsIncrease() {
        hits++;
        hitsText.text = string.Format("Hits: {0, 0:D3} ", hits);
    }

    public void OnMissesIncrease() {
        misses++;
        missesText.text = string.Format("Misses: {0, 0:D3} ", misses);
    }
}
