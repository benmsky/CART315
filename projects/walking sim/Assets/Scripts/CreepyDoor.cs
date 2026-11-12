using UnityEngine;

// The front door. It starts shut, and when the player gets close
// it creaks open slowly by itself, then stays open.
// Put this on the hinge (the door's parent object), with an AudioSource.
public class CreepyDoor : MonoBehaviour
{
    public Transform player;
    public AudioClip creakSound;
    public float triggerDistance = 11f; // how close the player gets before it starts opening
    public float waitBeforeOpening = 0.4f;
    public float openTime = 3f;         // seconds to swing all the way open
    public float openAngle = 95f;       // degrees, swinging into the cabin
    public bool hasOpened;              // turns true the moment the door starts to open (other scripts can check it)

    private AudioSource audioSource;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool triggered;
    private float timer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        closedRotation = transform.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    void Update()
    {
        // wait for the player to come close, then start opening (only once)
        if (!triggered)
        {
            if (Vector3.Distance(transform.position, player.position) < triggerDistance)
            {
                triggered = true;
                hasOpened = true;
                audioSource.PlayOneShot(creakSound);
            }
            return;
        }

        timer += Time.deltaTime;
        float progress = Mathf.Clamp01((timer - waitBeforeOpening) / openTime);

        // starts swinging quickly, then slows down as it reaches the end
        float eased = 1f - (1f - progress) * (1f - progress);
        transform.localRotation = Quaternion.Slerp(closedRotation, openRotation, eased);
    }
}
