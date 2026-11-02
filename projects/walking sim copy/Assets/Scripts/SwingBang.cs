using UnityEngine;

// Plays the porch swing banging against the cabin wall, over and over.
// Put this on the swing along with an AudioSource.
public class SwingBang : MonoBehaviour
{
    public AudioClip bangSound;
    public float timeBetweenBangs = 7f; // seconds

    private AudioSource audioSource;
    private float timer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // count up, and bang every time the timer runs out
        timer += Time.deltaTime;
        if (timer >= timeBetweenBangs)
        {
            timer = 0f;

            // a little randomness so it doesn't sound like a machine
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(bangSound);
        }
    }
}
