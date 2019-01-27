using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditScreenUi : MonoBehaviour
{

    public Button Back;
        
    // Start is called before the first frame update
    void Start()
    {
        Back.onClick.AddListener(OnBack);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnBack()
    {
        SoundManager.Instance.PlayUiButton();
        SceneManager.LoadScene("MainMenu");
    }
}
