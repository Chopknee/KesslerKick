using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public Button PlayButton;
    public Button QuitButton;
    public Button BeginButton;
    public InputField wdhmtoInput;

    public static string PlayerHome = "Apparently... nothing.";

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.StartMenuMusic();
        PlayButton.onClick.AddListener(OnPlayClicked);
        BeginButton.onClick.AddListener(OnBeginClicked);
        QuitButton.onClick.AddListener(Quit);
        wdhmtoInput.onEndEdit.AddListener(OnUserTyped);
    }

    public void OnPlayClicked() {
        SoundManager.Instance.StopMenuMusic();
        SceneManager.LoadScene("Drawing");
    }

    public void OnBeginClicked() {
        //Transition the scene.
        //Save the text somehow.
        SoundManager.Instance.StopMenuMusic();
        SceneManager.LoadScene("2DNavigation");
    }

    public void Quit() {
        Application.Quit();
    }

    public void OnUserTyped(string text) {
        PlayerHome = text;
    }
}
