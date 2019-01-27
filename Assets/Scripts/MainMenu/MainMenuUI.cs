using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public Button PlayButton;
    public Button QuitButton;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.StartMenuMusic();
        PlayButton.onClick.AddListener(OnPlayClicked);
        QuitButton.onClick.AddListener(Quit);
    }

    public void OnPlayClicked() {
        SoundManager.Instance.StopMenuMusic();
        SceneManager.LoadScene("Drawing");
    }

    public void Quit() {
        Application.Quit();
    }
}
