using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverGUI : MonoBehaviour
{

    public Button MenuButton;
    public Button QuitButton;
    public Text playerMessageText;
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.StartAmbientMusic();
        MenuButton.onClick.AddListener(GoToMenu);
        QuitButton.onClick.AddListener(Quit);
    }

    public void GoToMenu() {
        SoundManager.Instance.StopAmbientMusic();
        SoundManager.Instance.PlayUiButton(gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit() {
        SoundManager.Instance.StopAmbientMusic();
        SoundManager.Instance.PlayUiQuit(gameObject);
        Application.Quit();
    }
}
