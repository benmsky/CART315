using UnityEngine;

// The cellar trapdoor. When the player walks close to it, something underneath
// knocks on it, and every so often pushes it up a little before it slams back down.
public class TrapdoorRattle : MonoBehaviour
{
    public Transform player;
    public float triggerDistance = 4f;
    public float timeBetweenThumps = 1.5f;
    public AudioClip thumpSound;     // the slam after a push
    public AudioClip[] knockSounds;  // picks one at random

    public float pushChance = 0.35f; // how often it pushes instead of knocking (0.35 = 35%)
    public float pushHeight = 0.04f; // how far the door lifts, in metres
    public float pushTilt = 3f;      // how far it tips up on one side, in degrees

    private AudioSource audioSource;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private float thumpTimer;

    // the current movement: how long it lasts, how high and tilted it gets, and whether it slams at the end
    private float moveTimer;
    private float moveLength;
    private float moveHeight;
    private float moveTilt;
    private bool slamAtEnd;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // only act up when the player is close
        if (distance < triggerDistance)
        {
            thumpTimer += Time.deltaTime;
            if (thumpTimer >= timeBetweenThumps)
            {
                thumpTimer = 0f;
                timeBetweenThumps = Random.Range(1.2f, 3f); // a little randomness so it feels alive

                if (Random.value < pushChance)
                {
                    StartMove(0.6f, pushHeight, pushTilt, true);
                }
                else
                {
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(knockSounds[Random.Range(0, knockSounds.Length)]);
                    StartMove(0.2f, pushHeight * 0.15f, 0f, false); // a tiny jitter with each knock
                }
            }
        }

        if (moveTimer > 0f)
        {
            moveTimer -= Time.deltaTime;

            // 0 at the start, 1 halfway, back to 0 at the end: Mathf.Sin makes a smooth rise and fall
            float progress = 1f - moveTimer / moveLength;
            float lift = Mathf.Sin(progress * Mathf.PI);
            transform.position = startPosition + Vector3.up * moveHeight * lift;
            transform.rotation = startRotation * Quaternion.Euler(moveTilt * lift, 0f, 0f);

            if (moveTimer <= 0f)
            {
                transform.position = startPosition;
                transform.rotation = startRotation;
                if (slamAtEnd)
                {
                    audioSource.pitch = Random.Range(0.8f, 1.05f);
                    audioSource.PlayOneShot(thumpSound);
                }
            }
        }
    }

    void StartMove(float length, float height, float tilt, bool slam)
    {
        moveLength = length;
        moveTimer = length;
        moveHeight = height;
        moveTilt = tilt;
        slamAtEnd = slam;
    }
}
