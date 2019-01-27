using UnityEngine;
using UnityEngine.SceneManagement;

public class MeteoriteExplosion : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<ParticleSystem>() != null) {
            Invoke("Kill", GetComponent<ParticleSystem>().main.startLifetime.constant);
            GetComponent<ParticleSystem>().Play();
            if (SceneManager.GetActiveScene().name != "MainMenu")
                SoundManager.Instance.PlayPlanetHit(gameObject);
        }
    }

    void Kill() {
        Destroy(gameObject);
    }
}
