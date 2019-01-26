using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteMode : MonoBehaviour
{

    public GameObject[] meteorites;
    public float spawnRadius;
    public GameObject targetObject;
    public float spawnTime;
    private float t = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (t >= spawnTime) {
            GameObject go = Instantiate(targetObject);
            float pos = Random.Range(0f, 1f) * 2 * Mathf.PI;
            go.transform.position = ((Vector2)targetObject.transform.position) * new Vector2(Mathf.Cos(pos), Mathf.Sin(pos));
            //go.transform.position = new Vector2(
            //    (targetObject.transform.position.x + spawnRadius) * Mathf.Cos(pos * 2 * Mathf.PI),
            //    (targetObject.transform.position.y + spawnRadius) * Mathf.Sin(pos * 2 * Mathf.PI));
            t = 0;
        }
    }
}