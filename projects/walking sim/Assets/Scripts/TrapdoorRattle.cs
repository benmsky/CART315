using UnityEngine;

// The cellar trapdoor. When the player walks close to it,
// something underneath starts thumping on it from below.
public class TrapdoorRattle : MonoBehaviour
{
    public Transform player;
    public float triggerDistance = 4f;
    public float timeBetweenThumps = 1.5f;
    public AudioClip thumpSound;

    private AudioSource audioSource;
    private float thumpTimer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // only thump when the player is close
        if (distance < triggerDistance)
        {
            thumpTimer += Time.deltaTime;
            if (thumpTimer >= timeBetweenThumps)
            {
                thumpTimer = 0f;

                // a little randomness so it feels alive
                timeBetweenThumps = Random.Range(0.8f, 2f);
                audioSource.pitch = Random.Range(0.8f, 1.1f);
                audioSource.PlayOneShot(thumpSound);
            }
        }
    }
}
