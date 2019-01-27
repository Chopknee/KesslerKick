using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteExplosion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<ParticleSystem>() != null) {
            Debug.Log("Particle system component not present!");
            Invoke("Kill", GetComponent<ParticleSystem>().startLifetime);
            GetComponent<ParticleSystem>().Play();
            SoundManager.Instance.PlayPlanetHit(gameObject);
        }
    }

    void Kill() {
        Destroy(gameObject);
    }
}
