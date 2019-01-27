using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InfiniteModeUI : MonoBehaviour
{
    public ShipControls playerShip;
    public EarthTrigger planetEarth;
    public GameObject BossPrefab;
    GameObject boss;

    public int hits = 0;
    public int misses = 0;

    public static float wave = 10;
    // Start is called before the first frame update
    void Start()
    {
        if (playerShip == null || planetEarth == null) {
            Debug.Log("Missing some elements from the UI component, please check it!");
        }

        playerShip.OnGetKnockout += OnHitsIncrease;
        planetEarth.OnMiss += OnMissesIncrease;

        SoundManager.Instance.AddTimelineCallback(OnTimeline);
    }

    public void OnHitsIncrease() {
        hits++;
        //hitsText.text = string.Format("Hits: {0, 0:D3} ", hits);

        if (hits % 2 == 0)
        {
            wave = Mathf.Min(10, ++wave);
            SoundManager.Instance.SetMusicParam("Wave", wave);
        }
    }

    public void OnMissesIncrease() {
        misses++;
        //missesText.text = string.Format("Misses: {0, 0:D3} ", misses);
    }

    void OnTimeline(string tag)
    {
        switch(tag)
        {
            case "Boss":
                break;
            case "StartBoss":
                if (boss == null)
                {
                    boss = GameObject.Instantiate(BossPrefab);
                    boss.GetComponent<FinalBoss>().SpawnIn();
                }
                break;
            case "BossKilled":
                break;
            case "EndGame":
                SoundManager.Instance.StopLevelMusic();
                SceneManager.LoadScene("GameOverWin");
                break;
            default:
                break;
        }
    }

}
