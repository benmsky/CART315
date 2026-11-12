using UnityEngine;

// Plays the porch swing banging against the cabin wall, over and over,
// and rocks it back and forth just a tiny bit so it reaches the wall each time it bangs.
// Put this on the swing's pivot (where the chains hang from) along with an AudioSource.
public class SwingBang : MonoBehaviour
{
    public AudioClip bangSound;
    public float timeBetweenBangs = 7f; // seconds
    public float swingAngle = 1.2f;     // degrees, keep it tiny
    public int swingsPerBang = 2;       // how many times it rocks between bangs

    private AudioSource audioSource;
    private float timer;
    private Quaternion startRotation;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        startRotation = transform.localRotation;
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

        // Mathf.Cos is at its highest when the timer is 0, so the swing is closest to the wall right when it bangs
        float rock = Mathf.Cos(timer / timeBetweenBangs * Mathf.PI * 2f * swingsPerBang);
        transform.localRotation = startRotation * Quaternion.Euler(rock * swingAngle, 0f, 0f);
    }
}
