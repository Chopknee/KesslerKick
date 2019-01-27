using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableMeteor : MonoBehaviour
{
    bool grabbed = false;
    public GameObject particleObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag.Equals("Player") && grabbed == false) {
            //We gotta do somethin`
            GetComponent<Meteorite>().enabled = false;
            gameObject.transform.parent = collision.gameObject.transform;
            transform.localPosition = new Vector2(0, 2);//collision.gameObject.transform.up*2;
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            grabbed = true;
        }

    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag.Equals("FinalBoss") && grabbed == true) {
            Debug.Log("You are defo crazy " + grabbed);
            //The boss needs to take damage

            collision.gameObject.GetComponent<FinalBoss>().TakeDamage();
            if (particleObject != null) {
                GameObject go = Instantiate(particleObject);
                go.transform.position = transform.position;
            }
            Destroy(gameObject);
        }
    }
}
