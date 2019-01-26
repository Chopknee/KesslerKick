using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthTrigger : MonoBehaviour
{
    public delegate void Miss();
    public event Miss OnMiss;
    public GameObject explosionPrefab;
    public void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag.Equals("Meteorite")) {
            OnMiss?.Invoke();
            if (explosionPrefab != null) {
                GameObject fb = Instantiate(explosionPrefab);
                fb.transform.position = collision.transform.position;
                //Point away from the planet
                Vector3 point = collision.transform.position - transform.position;
                point.Normalize();
                fb.transform.rotation = Quaternion.LookRotation(point);
            }
            Destroy(collision.gameObject);
        }
    }
}
