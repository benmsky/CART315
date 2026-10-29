using UnityEngine;

// The cellar trapdoor. When the player walks close to it,
// something underneath starts banging on it from below.
public class TrapdoorRattle : MonoBehaviour
{
    public Transform player;
    public float triggerDistance = 4f;
    public float timeBetweenThumps = 1.5f;
    public float bumpHeight = 0.05f; // how far the door jumps up
    public AudioClip thumpSound;

    private AudioSource audioSource;
    private Vector3 startPosition;
    private float thumpTimer;
    private float bumpTimer;

    void Start()
    {
        startPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // only bang when the player is close
        if (distance < triggerDistance)
        {
            thumpTimer += Time.deltaTime;
            if (thumpTimer >= timeBetweenThumps)
            {
                thumpTimer = 0f;
                bumpTimer = 0.2f; // shake for 0.2 seconds

                // a little randomness so it feels alive
                timeBetweenThumps = Random.Range(0.8f, 2f);
                audioSource.pitch = Random.Range(0.8f, 1.1f);
                audioSource.PlayOneShot(thumpSound);
            }
        }

        // shake the door while the bump is happening, otherwise rest flat
        if (bumpTimer > 0f)
        {
            bumpTimer -= Time.deltaTime;
            transform.position = startPosition + Vector3.up * Random.Range(0f, bumpHeight);
        }
        else
        {
            transform.position = startPosition;
        }
    }
}
