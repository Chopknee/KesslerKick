using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalBoss : MonoBehaviour
{
    public static GameObject explosionPrefab;
    public GameObject orbitalBody;
    public GameObject projectile;

    public float currentSpeed = 0;
    public float maxSpeed;
    [Range(0f,1f)]
    public float percentAround = 0;
    private Vector2 lastPosition;
    private Vector2 velocity;
    public float altitude;

    public float accelerationTime;//The amount of time it takes to reach maximum speed
    public float direction;
    public float turningTime;
    public int state = 0;
    public float duration = 0;
    private float stopDistance;

    private float randomStopTime;

    private float targetRotation;
    private float startRotation;

    public float hits = 0;
    public float hitLimit = 4;
    public float ominousTime = 10;//Time boss takes to slide in scene
    public float slideInAltitude = 11;//How far off screen is the boss when it spawns?
    float startAltitude = 0;
    //Maybe useful?
    public delegate void Killed();
    public event Killed OnKilled;
    // Start is called before the first frame update
    void Start()
    {
        if (orbitalBody == null) {
            orbitalBody = GameObject.FindGameObjectWithTag("Earth");
        }
        startAltitude = altitude;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state) {
            case 0:
                //Set up for the acceleration
                duration = 0;
                state = 10;
                break;
            case 10:
                duration += Time.deltaTime;
                //Accelerating to top speed
                Accelerating(duration);
                //Also moving in the orbit
                MovingOnOrbit();
                //Once finished with acceleration, move on to the next part
                if (duration >= accelerationTime) {
                    duration = 0;
                    state = 20;
                }
                break;
            case 20:
                state = 21;
                randomStopTime = Random.Range(5f, 15f);
                break;
            case 21:
                duration += Time.deltaTime;
                //Moving in orbit
                MovingOnOrbit();
                //Looking for next chance to stop and torment the player
                if (duration > randomStopTime) {
                    duration = 0;
                    state = 30;
                }
                break;
            case 30:
                //Chosen to decelerate
                MovingOnOrbit();
                if (!(percentAround > 0.125f && percentAround < 0.375f || percentAround > 0.625f && percentAround < 0.875f)) {
                    duration += Time.deltaTime;
                    //Decelerate only if inside of the allowed areas.
                    Accelerating(1 - duration);
                    if (duration >= accelerationTime) {
                        duration = 0;
                        state = 40;
                    }
                }
                break;
            case 40:
                state = 41;
                startRotation = transform.rotation.eulerAngles.z;
                targetRotation = (Mathf.Rad2Deg * percentAround * 2 * Mathf.PI) + 90;
                break;
            case 41:
                //Begin turning
                duration += Time.deltaTime;
                RotateTowardPlanet(duration);
                if (duration > turningTime) {
                    state = 50;
                    duration = 0;
                    state = 51;
                }
                break;
            case 50:
                //Wait a bit
                duration += Time.deltaTime;
                if (duration > 0.5f) {
                    duration = 0;
                    state = 51;
                }
                break;
            case 51:
                //Shoot
                GameObject go = Instantiate(projectile);
                go.transform.position = (Vector2)transform.position + ((Vector2)transform.up*1.5f);
                state = 60;
                startRotation = transform.rotation.eulerAngles.z;
                //Pick a direction at random.
                direction = (Random.Range(0, 2) == 1) ? 1 : -1;
                targetRotation = Mathf.Rad2Deg * percentAround * 2 * Mathf.PI - ((direction < 0) ? 180 : 0);
                break;
            case 60:
                //Turning back
                duration += Time.deltaTime;
                RotateTowardPlanet(duration);
                if (duration > turningTime) {
                    state = 50;
                    duration = 0;
                    state = 0;
                }
                break;
            case 70:
                //Spawn in 'animation'
                state = 71;
                duration = 0;
                break;
            case 71:
                MovingOnOrbit();
                altitude = Mathf.Lerp(slideInAltitude, startAltitude, duration / ominousTime);
                //Waiting for slide in animation to finish
                if (duration > ominousTime) {
                    //Finished being ominous
                    state = 60;
                    //Begin the cycle of tormentation!
                    direction = (Random.Range(0, 2) == 1) ? 1 : -1;
                    targetRotation = Mathf.Rad2Deg * percentAround * 2 * Mathf.PI - ((direction < 0) ? 180 : 0);
                }
                break;
            case 80:
                //Explode and do fun things, yay!
                //Do some explosions, then run on killed
                duration += Time.deltaTime;
                
                if (duration > 5) {
                    OnKilled?.Invoke();
                    //Game won
                    state = 100;//Non checked state!!
                    //Destroy(gameObject);
                    duration = 0;
                    GetComponent<Renderer>().enabled = false;
                    SoundManager.Instance.SetMusicParam("BossKilled", 1.0f);
                    SoundManager.Instance.PlayBossDeath(gameObject);
                }
                break;
            case 100:
                duration += Time.deltaTime;
                if (duration > 5) {
                    SceneManager.LoadScene("GameOverWin");
                }
                break;
        }
    }

    //Call this to do the ominous spawn in animation thing.
    public void SpawnIn() {
        state = 70;//Set to spawn in state
        transform.rotation = Quaternion.Euler(0, 0, (Mathf.Rad2Deg * percentAround * 2 * Mathf.PI) + 90);//Turn toward the planet
    }

    public static void KillMetors()
    {
        GameObject[] meteors = GameObject.FindGameObjectsWithTag("Meteorite");
        foreach (GameObject met in meteors) {
            met.GetComponent<Meteorite>().Kill();
        }
    }

    public void RotateTowardPlanet(float t) {
        //What is the target angle?
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(startRotation, targetRotation, t));
    }

    public void Accelerating(float t) {
        currentSpeed = direction * Mathf.Lerp(0, maxSpeed, t/accelerationTime);
    }

    public void MovingOnOrbit() {
        percentAround += currentSpeed * Time.deltaTime;
        if (percentAround > 1) { percentAround = percentAround - 1; }
        if (percentAround < 0) { percentAround = percentAround + 1; }
        transform.position = new Vector2(
            (orbitalBody.transform.position.x + altitude) * Mathf.Cos(percentAround * 2 * Mathf.PI),
            (orbitalBody.transform.position.y + altitude) * Mathf.Sin(percentAround * 2 * Mathf.PI));
        transform.rotation = Quaternion.Euler(0, 0, (Mathf.Rad2Deg * percentAround * 2 * Mathf.PI) - ((currentSpeed < 0) ? 180 : 0));
        //Calculating velocity for hitting rocks
        velocity = (Vector2)transform.position - lastPosition * Time.deltaTime;
        lastPosition = transform.position;
    }

    //The boss got hit!
    public void TakeDamage() {
        hits++;
        SoundManager.Instance.PlayBossMelody(gameObject);
        if (hits >= hitLimit) {
            state = 80;
            duration = 0;
        }
    }

}
