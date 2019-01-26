﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuMeteorShower : MonoBehaviour
{

    public GameObject meteorite;
    public float spawnRadius;
    public GameObject targetObject;
    public float spawnTime;
    private float t = 0;

    public int baseBPM;
    [Range(1, 100)]
    public int maxBMPMultiplier;
    public int initialPunch;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (t >= spawnTime) {
            GameObject go = Instantiate(meteorite);
            float pos = Random.Range(0f, 1f) * 2 * Mathf.PI;
            go.transform.position = ((Vector2)targetObject.transform.position) * new Vector2(Mathf.Cos(pos), Mathf.Sin(pos));
            go.transform.position = new Vector2(
                (targetObject.transform.position.x + spawnRadius) * Mathf.Cos(pos * 2 * Mathf.PI),
                (targetObject.transform.position.y + spawnRadius) * Mathf.Sin(pos * 2 * Mathf.PI));
            Meteorite m = go.GetComponent<Meteorite>();
            if (m != null) {
                int mul = Random.Range(1, maxBMPMultiplier);
                m.pulsesPerSecond = (baseBPM * mul) / 60;
                m.pulseMultiplier = initialPunch * mul;
            }
            t = 0;
        }
    }
}