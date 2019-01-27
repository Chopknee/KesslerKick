using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverSounds : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlayGameOver(gameObject);        
    }
}
