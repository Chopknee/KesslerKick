using UnityEngine;
using UnityEngine.SceneManagement;

public class MeteoriteExplosion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<ParticleSystem>() != null) {
            Debug.Log("Particle system component not present!");
            Invoke("Kill", GetComponent<ParticleSystem>().startLifetime);
            GetComponent<ParticleSystem>().Play();

            if (SceneManager.GetActiveScene().name != "MainMenu")
                SoundManager.Instance.PlayPlanetHit(gameObject);
        }
    }

    void Kill() {
        Destroy(gameObject);
    }
}
