using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Button PlayButton;
    public Button QuitButton;
    public Button BeginButton;
    public InputField wdhmtoInput;

    // Start is called before the first frame update
    void Start()
    {
        PlayButton.onClick.AddListener(OnPlayClicked);
        BeginButton.onClick.AddListener(OnBeginClicked);
    }

    public void OnPlayClicked() {
        PlayButton.gameObject.SetActive(false);//Hide the play button
        wdhmtoInput.gameObject.SetActive(true);
        BeginButton.gameObject.SetActive(true);
    }

    public void OnBeginClicked() {
        //Transition the scene.
        //Save the text somehow.
    }
}
