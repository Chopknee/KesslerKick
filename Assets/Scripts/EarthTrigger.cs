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
    public float explosionForceMultiplier = 20000;
    public bool destryoed = false;
    public void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag.Equals("Meteorite")) {
            OnMiss?.Invoke();
            if (destryoed) { return; }
            if (explosionPrefab != null) {
                GameObject fb = Instantiate(explosionPrefab);
                fb.transform.position = collision.transform.position;
                //Point away from the planet
                Vector3 point = collision.transform.position - transform.position;
                point.Normalize();
                fb.transform.rotation = Quaternion.LookRotation(point);
                hits++;
                if (hits >= maxHits) {
                    destryoed = true;
                    //Game over!
                    Invoke("SwitchScene", 20);
                    //"Break" apart the world.
                    foreach (GameObject go in earthChunks) {
                        GameObject newGo = Instantiate(go);
                        newGo.transform.position = Vector3.zero;
                        Vector2 force = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                        force *= explosionForceMultiplier;
                        Debug.Log(force);
                        newGo.GetComponent<Rigidbody2D>().AddForce(force);
                    }
                    foreach (Renderer rend in GetComponentsInChildren<Renderer>()) {
                        rend.enabled = false;
                    }
                }
            }
            Destroy(collision.gameObject);
        }


    }

    public void SwitchScene() {
        SceneManager.LoadScene("GameOverDeath");
    }
}
