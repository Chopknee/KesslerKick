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
        MenuButton.onClick.AddListener(GoToMenu);
        QuitButton.onClick.AddListener(Quit);
        playerMessageText.text = MainMenuUI.PlayerHome;
    }

    public void GoToMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit() {
        Application.Quit();
    }
}
