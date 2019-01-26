using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteorite : MonoBehaviour
{
    public Sprite[] PossibleSprites;

    [Tooltip("The object to aim for when pulsing.")]
    public GameObject pulseTowardTarget;
    private Rigidbody2D rigidBody;
    // Start is called before the first frame update
    private float pulse = 0;
    [Tooltip("Pulses per second.")]
    public float pulsesPerSecond = 0;
    [Tooltip("How strong the pulse is.")]
    public float pulseMultiplier;
    [Tooltip("Changes the 'punch' of the pulse.")]
    public AnimationCurve pulseCurve;
    public bool floatMode = false;

    public delegate void Collided();
    public event Collided OnHitPlayer;
    public event Collided OnHitPlanet;

    public delegate void Destroyed();
    public event Destroyed OnDestroyed;

    private int pulseBeat;

    void Start()
    {
        if (pulseTowardTarget == null) {
            pulseTowardTarget = GameObject.FindGameObjectWithTag("Earth");
        }
        rigidBody = GetComponent<Rigidbody2D>();
        pulsesPerSecond = pulsesPerSecond/2;

        GetComponent<SpriteRenderer>().sprite = PossibleSprites[Random.Range(0, PossibleSprites.Length)];

        SoundManager.Instance.AddBeatCallback(OnBeat);

        pulseBeat = Random.Range(1, 5);
    }

    // Update is called once per frame
    void Update()
    {
        if (!floatMode) {
            pulse += pulsesPerSecond * Time.deltaTime;
            Pulse();
        }
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        floatMode = true;
        switch(collision.gameObject.tag) {
            case "Player":
                OnHitPlayer?.Invoke();
                break;
            case "Earth":
                OnHitPlanet?.Invoke();
                break;
        }
    }

    public void Kill() {
        Destroy(gameObject);
    }

    private void Pulse()
    {
        if (!floatMode)
        {
            Vector3 targetDirection = pulseTowardTarget.transform.position - transform.position;
            targetDirection.Normalize();
            //Now the "pulsing" mechanic.
           rigidBody.velocity = targetDirection * pulseMultiplier * pulseCurve.Evaluate(pulse);

        }
    }

    private void OnBeat(int bar, int beat)
    {
        if (beat == pulseBeat || ((bar + 3) % 4 == 0 && beat == 1))
            pulse = 0;
    }
}
