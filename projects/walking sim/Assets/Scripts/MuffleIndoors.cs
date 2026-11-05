using UnityEngine;

// Muffles an outside sound while the player is inside the cabin.
// Put this on any object with an outside AudioSource and an AudioLowPassFilter.
public class MuffleIndoors : MonoBehaviour
{
    public Transform player;

    // the box of space that counts as "inside the cabin"
    public Bounds cabinArea = new Bounds(new Vector3(0f, 2f, 0f), new Vector3(12f, 4f, 10f));

    public float outdoorCutoff = 22000f; // all of the sound gets through
    public float indoorCutoff = 2000f;   // high sounds like birds get softened
    public float indoorVolume = 1.25f;   // 1.25 = 25% louder, so it stays easy to hear
    public float fadeSpeed = 3f;

    private AudioLowPassFilter filter;
    private AudioSource audioSource;
    private float outdoorVolume;

    void Start()
    {
        filter = GetComponent<AudioLowPassFilter>();
        audioSource = GetComponent<AudioSource>();
        outdoorVolume = audioSource.volume;
    }

    void Update()
    {
        bool inside = cabinArea.Contains(player.position);

        float targetCutoff = inside ? indoorCutoff : outdoorCutoff;
        float targetVolume = inside ? outdoorVolume * indoorVolume : outdoorVolume;

        // slide toward the new sound so it doesn't snap when you cross the door
        filter.cutoffFrequency = Mathf.Lerp(filter.cutoffFrequency, targetCutoff, Time.deltaTime * fadeSpeed);
        audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, Time.deltaTime * fadeSpeed);
    }
}
