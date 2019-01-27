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
    public bool IsWin;
    // Start is called before the first frame update
    void Start()
    {
        MenuButton.onClick.AddListener(GoToMenu);
        QuitButton.onClick.AddListener(Quit);
        IsWin = SceneManager.GetActiveScene().name == "GameOverWin"; 
        if (IsWin)
        {
            SoundManager.Instance.StartAmbientMusic();
        }
        else
        {
            SoundManager.Instance.StartGameOverMusic();
        }
    }

    public void GoToMenu() {
        if (IsWin)
        {
            SoundManager.Instance.StopAmbientMusic();
        }
        else
        {
            SoundManager.Instance.StopGameOverMusic();
        }


        SoundManager.Instance.PlayUiButton();
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit() {
        if (IsWin)
        {
            SoundManager.Instance.StopAmbientMusic();
        }
        else
        {
            SoundManager.Instance.StopSound(gameObject);
        }

        SoundManager.Instance.PlayUiQuit();
        Application.Quit();
    }
}
