using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControls : MonoBehaviour
{
    public GameObject orbitalBody;

    public float currentSpeed = 0;
    public float accelerationSpeed = 0;
    public float maxSpeed;
    [Range(0.01f, 0.99f)]
    public float slowdownPercent;//Percent of the current speed to loose when not accelerating.

    public string accelerationAxis;
    public string altitudeAxis;
    //public string accelerateCClockwise;
    private Vector2 lastPosition;
    private float percentAround = 0;
    private Vector2 velocity;

    public float maxAltitude;
    public float minAltitude;
    private float altitude;
    public float altAcceleration;
    public float altSpeed;
    [Range(0.01f, 0.99f)]
    public float altitudeDeceleration;
    public float maxAltitudeSpeed;

    public delegate void GetKnockout();
    public event GetKnockout OnGetKnockout;

    private ParticleSystem thruster;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;
        velocity = Vector2.zero;
        GetComponentInChildren<ForceFieldTrigger>().OnTriggered += OnTriggerEnter2D;
        thruster = GetComponentInChildren<ParticleSystem>(); 
    }

    // Update is called once per frame
    void Update()
    {
        float orbt = 0;

        if (Input.GetAxis(accelerationAxis) < 0) {
            orbt = accelerationSpeed;
            SoundManager.Instance.StartThruster(gameObject);
            if (thruster.isStopped)
                thruster.Play();
        } else if (Input.GetAxis(accelerationAxis) > 0) {
            orbt = -accelerationSpeed;
            SoundManager.Instance.StartThruster(gameObject);
            if (thruster.isStopped)
                thruster.Play();
        } else {
            orbt = -currentSpeed * slowdownPercent;
            SoundManager.Instance.StopThruster(gameObject);
            if (thruster.isPlaying)
                thruster.Stop();
        }


        currentSpeed += orbt;
        currentSpeed = Mathf.Sign(currentSpeed) * Mathf.Min(Mathf.Abs(currentSpeed), maxSpeed);
        percentAround += currentSpeed * Time.deltaTime;

        float alt = 0;

        if (Input.GetAxis(altitudeAxis) < 0) {
            alt = -altAcceleration;
        } else if (Input.GetAxis(altitudeAxis) > 0) {
            alt = altAcceleration;
        } else {
            alt = -altSpeed * altitudeDeceleration;
        }
        altSpeed += alt;
        altSpeed = Mathf.Sign(altSpeed) * Mathf.Min(Mathf.Abs(altSpeed), maxAltitudeSpeed);
        altitude = Mathf.Min(Mathf.Max(altitude + (altSpeed * Time.deltaTime), minAltitude), maxAltitude);

        transform.position = new Vector2(
            (orbitalBody.transform.position.x + altitude) * Mathf.Cos(percentAround * 2 * Mathf.PI),
            (orbitalBody.transform.position.y + altitude) * Mathf.Sin(percentAround * 2 * Mathf.PI));
        transform.rotation = Quaternion.Euler(0, 0, (Mathf.Rad2Deg * percentAround * 2 * Mathf.PI) - ((currentSpeed < 0) ? 180 : 0));
        //Calculating velocity for hitting rocks
        velocity = (Vector2)transform.position - lastPosition * Time.deltaTime;
        lastPosition = transform.position;
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag.Equals("Meteorite")) {
            collision.gameObject.GetComponent<Meteorite>().floatMode = true;
            //Send the thing flying off in the direction of the ship
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(velocity * 50);
            OnGetKnockout?.Invoke();
        }
    }
}
