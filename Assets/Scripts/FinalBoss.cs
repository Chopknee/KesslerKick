using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    public GameObject orbitalBody;
    public GameObject projectile;
    public Vector2 projectileOffset;

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
    // Start is called before the first frame update
    void Start()
    {
        if (orbitalBody == null) {
            orbitalBody = GameObject.FindGameObjectWithTag("Earth");
        }
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
                Debug.Log("Finished accelerating orbit.");
                randomStopTime = Random.Range(5f, 15f);
                break;
            case 21:
                duration += Time.deltaTime;
                //Moving in orbit
                MovingOnOrbit();
                //Looking for next chance to stop and torment the player
                if (duration > randomStopTime) {
                    Debug.Log("Time to slow down.");
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
                Debug.Log("I have finished slowing down.");
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
                    Debug.Log("I have finished turning.");
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
                Debug.Log("Firing missiles or whatever.");
                GameObject go = Instantiate(projectile);
                go.transform.position = (Vector2)transform.position + projectileOffset;
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
                    Debug.Log("I have finished turning.");
                    duration = 0;
                    state = 0;
                }
                break;
        }
    }

    //Call this to do the ominous spawn in animation thing.
    public void SpawnIn() {

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
}
