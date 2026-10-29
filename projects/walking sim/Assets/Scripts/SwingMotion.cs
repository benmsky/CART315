using UnityEngine;

// Swings an object back and forth, like the porch swing or the clock pendulum.
// Put this on the pivot point (the top of the chains), not on the seat itself.
// If you give it a bang sound, it plays every time it swings all the way back.
public class SwingMotion : MonoBehaviour
{
    public float swingAngle = 15f;  // how far it swings, in degrees
    public float swingSpeed = 1.5f; // how fast it swings
    public AudioClip bangSound;     // optional

    private AudioSource audioSource;
    private Quaternion startRotation;
    private float lastSwing;
    private bool wasSwingingBack;

    void Start()
    {
        startRotation = transform.localRotation;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Mathf.Sin goes smoothly between -1 and 1 forever, which is perfect for swinging
        float swing = Mathf.Sin(Time.time * swingSpeed);
        transform.localRotation = startRotation * Quaternion.Euler(swing * swingAngle, 0f, 0f);

        // when it stops moving back and starts moving forward, it has hit the wall
        bool swingingBack = swing > lastSwing;
        if (wasSwingingBack && !swingingBack && bangSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(bangSound);
        }

        wasSwingingBack = swingingBack;
        lastSwing = swing;
    }
}
