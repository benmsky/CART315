using UnityEngine;

// The old car's headlights. When the cabin door starts to open,
// they flicker for a moment and then die, with a groaning metal sound.
// Put this on the car, with an AudioSource.
public class CarLights : MonoBehaviour
{
    public CreepyDoor door;
    public Light[] lights;            // the lights that shine on the ground
    public Renderer[] beams;          // the see-through beam cones
    public Renderer[] headlights;     // the glowing glass on the front of the car
    public Material headlightOn;
    public Material headlightOff;
    public float flickerTime = 1.2f;  // how long it flickers before going dark
    public AudioClip groanSound;      // plays once when the flicker starts
    public float groanStartTime = 1f; // skip this many seconds of the groan's quiet start

    private bool flickering;
    private bool dead;
    private float timer;
    private float nextSwitch;
    private bool lightsOn = true;

    void Update()
    {
        if (dead) return;

        // wait for the door
        if (!flickering)
        {
            if (door.hasOpened)
            {
                flickering = true;
                AudioSource audioSource = GetComponent<AudioSource>();
                audioSource.clip = groanSound;
                audioSource.time = groanStartTime;
                audioSource.Play();
            }
            return;
        }

        timer += Time.deltaTime;

        // flip on and off at random little gaps
        if (timer >= nextSwitch)
        {
            SetLights(!lightsOn);
            nextSwitch = timer + Random.Range(0.03f, 0.18f);
        }

        // then off for good
        if (timer >= flickerTime)
        {
            SetLights(false);
            dead = true;
        }
    }

    void SetLights(bool on)
    {
        lightsOn = on;
        foreach (Light l in lights) l.enabled = on;
        foreach (Renderer r in beams) r.enabled = on;
        foreach (Renderer r in headlights) r.sharedMaterial = on ? headlightOn : headlightOff;
    }
}
