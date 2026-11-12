using UnityEngine;

// The wall clock. Swings the pendulum back and forth,
// and plays a tick or a tock each time it reaches the end of a swing.
// Put this on the clock, with an AudioSource.
public class PendulumClock : MonoBehaviour
{
    public Transform pendulum;
    public float swingAngle = 6f;      // degrees either side of straight down
    public float secondsPerSwing = 1f; // a real pendulum clock swings once a second
    public AudioClip tickSound;
    public AudioClip tockSound;

    private AudioSource audioSource;
    private float lastDirection;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Mathf.Sin goes smoothly between -1 and 1, which makes a natural swing
        float time = Time.time * Mathf.PI / secondsPerSwing;
        float swing = Mathf.Sin(time);
        pendulum.localRotation = Quaternion.Euler(0f, 0f, swing * swingAngle);

        // the pendulum changes direction at each end of its swing, that's when it ticks
        float direction = Mathf.Sign(Mathf.Cos(time));
        if (lastDirection != 0f && direction != lastDirection)
        {
            audioSource.PlayOneShot(direction > 0f ? tickSound : tockSound);
        }
        lastDirection = direction;
    }
}
