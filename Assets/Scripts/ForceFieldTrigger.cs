using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldTrigger : MonoBehaviour
{

    public delegate void Triggered(Collider2D cl);
    public event Triggered OnTriggered;
    // Start is called before the first frame update
    public void OnTriggerEnter2D(Collider2D collision) {
        if (OnTriggered != null) {
            OnTriggered(collision);
        }
    }
}
