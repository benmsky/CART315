using UnityEngine;

// Muffles an outside sound while the player is inside the cabin.
// Put this on any object with an outside AudioSource and an AudioLowPassFilter.
// Optional: give it an indoor version of the sound (same recording, birds taken out) and it fades over to that inside.
public class MuffleIndoors : MonoBehaviour
{
    public Transform player;

    // the box of space that counts as "inside the cabin"
    public Bounds cabinArea = new Bounds(new Vector3(0f, 2f, 0f), new Vector3(12f, 4f, 10f));

    public float outdoorCutoff = 22000f; // all of the sound gets through
    public float indoorCutoff = 2000f;   // high sounds like birds get softened
    public float indoorVolume = 1.25f;   // 1.25 = 25% louder, so it stays easy to hear
    public float fadeSpeed = 3f;
    public AudioSource indoorVersion;    // optional, leave empty to just muffle

    private AudioLowPassFilter filter;
    private AudioSource audioSource;
    private float outdoorVolume;
    private float insideAmount;          // 0 = outside, 1 = inside

    void Start()
    {
        filter = GetComponent<AudioLowPassFilter>();
        audioSource = GetComponent<AudioSource>();
        outdoorVolume = audioSource.volume;

        // keep the indoor version playing in step with the outdoor one, silent for now
        if (indoorVersion != null)
        {
            indoorVersion.volume = 0f;
            indoorVersion.timeSamples = audioSource.timeSamples;
        }
    }

    void Update()
    {
        bool inside = cabinArea.Contains(player.position);

        float targetCutoff = inside ? indoorCutoff : outdoorCutoff;

        // slide toward the new sound so it doesn't snap when you cross the door
        filter.cutoffFrequency = Mathf.Lerp(filter.cutoffFrequency, targetCutoff, Time.deltaTime * fadeSpeed);
        insideAmount = Mathf.Lerp(insideAmount, inside ? 1f : 0f, Time.deltaTime * fadeSpeed);
        float volume = outdoorVolume * Mathf.Lerp(1f, indoorVolume, insideAmount);

        if (indoorVersion != null)
        {
            // fade from the outdoor sound to the indoor version
            audioSource.volume = volume * (1f - insideAmount);
            indoorVersion.volume = volume * insideAmount;
        }
        else
        {
            audioSource.volume = volume;
        }
    }
}
