using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /** Set the rate at which the meteorites "pulse"
     * 
     */
    public void SetBPM(float bpm) {
        float bps = bpm / 60f;
        GameObject[] meteors = GameObject.FindGameObjectsWithTag("Meteorite");
        foreach (GameObject m in meteors) {
            if (m.GetComponent<Meteorite>() != null)
                m.GetComponent<Meteorite>().pulsesPerSecond = bps;
        }
    }
    
    /**
     * Set the strength at which the meteorites pulse.
     * Basically, how far they move each pulse.
     */
    public void SetPunch(float value) {
        GameObject[] meteors = GameObject.FindGameObjectsWithTag("Meteorite");
        foreach (GameObject m in meteors) {
            if (m.GetComponent<Meteorite>() != null)
                m.GetComponent<Meteorite>().pulseMultiplier = value;
        }
    }
}
