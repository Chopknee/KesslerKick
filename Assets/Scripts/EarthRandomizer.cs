﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthRandomizer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(
            Random.Range(0f, 359f),
            Random.Range(0f, 359f),
            Random.Range(0f, 359f)
            );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
