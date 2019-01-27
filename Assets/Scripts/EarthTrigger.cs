using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EarthTrigger : MonoBehaviour
{
    public int hits = 0;
    public int maxHits = 40;

    public delegate void Miss();
    public event Miss OnMiss;
    public GameObject explosionPrefab;
    public GameObject[] earthChunks;
    public float explosionForceMultiplier = 20000;
    public bool destroyed = false;

    public void OnCollisionEnter2D(Collision2D collision) {

        if (collision.gameObject.tag.Equals("Meteorite")) {
            OnMiss?.Invoke();
            TakeHit(collision.gameObject);
        }
    }

    public void TakeHit(GameObject collision)
    {
        if (explosionPrefab == null)
            return;

        if (SceneManager.GetActiveScene().name == "2DNavigation")
        {
            hits++;
            if (hits >= maxHits && !destroyed) {
                StartEndGame();
            }
        }

        collision.gameObject.GetComponent<Meteorite>().Kill();
    }

    public void StartEndGame()
    {
        //Game over!
        InfiniteMode.CanSpawn = false;
        FinalBoss.CanShoot = false;
        FinalBoss.KillMetors();

        //"Break" apart the world.
        foreach (GameObject go in earthChunks) {
            GameObject newGo = Instantiate(go);
            newGo.transform.position = Vector3.zero;
            Vector2 force = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            force *= explosionForceMultiplier;
            newGo.GetComponent<Rigidbody2D>().AddForce(force);
        }

        foreach (Renderer rend in GetComponentsInChildren<Renderer>()) {
            rend.enabled = false;
        }

        destroyed = true;
        Invoke("SwitchScene", 5);
    }

    public void SwitchScene() {
        SoundManager.Instance.StopSound(GameObject.FindGameObjectWithTag("Player"));
        SoundManager.Instance.StopLevelMusic();
        SceneManager.LoadScene("GameOverDeath");
    }
}
