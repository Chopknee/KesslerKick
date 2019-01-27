using UnityEngine;

public class Meteorite : MonoBehaviour
{
    public GameObject explosionPrefab;
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

    private int pulseBeat;
    private Vector3 startPoint;

    void Start()
    {
        if (pulseTowardTarget == null) {
            pulseTowardTarget = GameObject.FindGameObjectWithTag("Earth");
        }
        rigidBody = GetComponent<Rigidbody2D>();
        pulsesPerSecond = pulsesPerSecond/2;

        GetComponent<SpriteRenderer>().sprite = PossibleSprites[Random.Range(0, PossibleSprites.Length)];

        SoundManager.Instance.AddBeatCallback(OnBeat);

        pulseBeat = Random.Range(1, 6);

        var startPoint = transform.position;

        pulseMultiplier = Mathf.Max(Random.Range(1, Mathf.Max(InfiniteModeUI.wave / 5, 1)) * pulseMultiplier, pulseMultiplier);
    }

    // Update is called once per frame
    void Update()
    {
        if (!floatMode) {
            pulse += pulsesPerSecond * Time.deltaTime;
            Pulse();
        }
        else
        {
            if (this.GetComponent<Rigidbody2D>().velocity.magnitude < 2.0f)
            {

                Vector3 targetDirection = pulseTowardTarget.transform.position - transform.position;
                targetDirection.Normalize();

                rigidBody.AddForce(-targetDirection * 50);
            }

            Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
            if (!onScreen)
                Kill();
        }
    }

    public bool Hit(Vector3 velocity)
    {
        if (!floatMode)
        {
            floatMode = true;
            SoundManager.Instance.PlayEnemyHit(gameObject);

            //Send the thing flying off in the direction of the ship
            rigidBody.AddForce(velocity * 50);

            return true;
        }

        return false;
    }

    public void Kill() {
        if (explosionPrefab != null)
        {
            GameObject fb = Instantiate(explosionPrefab);
            fb.transform.position = transform.position;
            Vector3 point = transform.position - pulseTowardTarget.transform.position;
            point.Normalize();
            fb.transform.rotation = Quaternion.LookRotation(point);
        }
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        var sm = SoundManager.Instance;
        if (sm != null)
            sm.RemoveBeatCallback(OnBeat);
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
       if (beat == pulseBeat || ((bar + 3) % 4 == 0 && beat == 1) || pulseBeat == 5)
            pulse = 0;
    }
}
