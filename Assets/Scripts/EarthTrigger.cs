using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EarthTrigger : MonoBehaviour
{
    public int hits = 0;
    public int maxHits = 20;

    public delegate void Miss();
    public event Miss OnMiss;
    public GameObject explosionPrefab;
    public GameObject[] earthChunks;
    public float explosionForceMultiplier = 50;
    public void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag.Equals("Meteorite")) {
            OnMiss?.Invoke();
            if (explosionPrefab != null) {
                /*
                //GameObject fb = Instantiate(explosionPrefab);
                //fb.transform.position = collision.transform.position;
                //Point away from the planet
                Vector3 point = collision.transform.position - transform.position;
                point.Normalize();
                fb.transform.rotation = Quaternion.LookRotation(point);
                */
                hits++;
                if (hits >= maxHits) {
                    //Game over!
                    Invoke("SwitchScene", 20);
                    GetComponent<Renderer>().enabled = false;//Hide this thing!!!
                    //"Break" apart the world.
                    foreach (GameObject go in earthChunks) {
                        GameObject newGo = Instantiate(go);
                        go.transform.position = Vector3.zero;
                        go.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(.5f, 1f), Random.Range(.5f, 1f) * explosionForceMultiplier));
                    }
                }
            }
            collision.gameObject.GetComponent<Meteorite>().Kill();
        }


    }

    public void SwitchScene() {
        SoundManager.Instance.StopLevelMusic();
        SceneManager.LoadScene("GameOverDeath");
    }
}
