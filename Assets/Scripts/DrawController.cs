using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DrawController : MonoBehaviour
{
    public Text[] text;
    public Button button;
    public float ColorLerpSpeed = 2;

    Color startingColor;
    float currentLerp = 0;

    private void Start()
    {
        SoundManager.Instance.StartAmbientMusic();
        startingColor = text[0].color;
        button.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentLerp < 1)
        {
            currentLerp += Time.deltaTime * ColorLerpSpeed;

            foreach(var t in text)
            {
                t.color = Color.Lerp(Color.clear, startingColor, currentLerp);
            }
        }
    }

    void OnClick()
    {
        SoundManager.Instance.StopAmbientMusic();
        SoundManager.Instance.PlayUiStart(gameObject);
        SceneManager.LoadScene("2DNavigation");
    }
}
