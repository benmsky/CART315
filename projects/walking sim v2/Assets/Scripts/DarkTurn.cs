using UnityEngine;

// the first time the tape plays, things go bad. the cabin lights freak out for a second,
// then the whole night drops darker and stays that way. the cellar goes nuts and the fire dies
// goes on any empty object
public class DarkTurn : MonoBehaviour
{
    public AudioSource tape;             // the tape recorder's audio
    public IndoorLighting lighting;      // handles the ambient light and fog
    public Light moonlight;
    public LensFlare moonFlare;
    public Light[] flickerLights;        // cabin lights that freak out
    public TrapdoorRattle trapdoor;
    public AudioSource nightAmbience;

    // the cellar once it's angry
    public float angryShortestWait = 0.4f;
    public float angryLongestWait = 1.1f;
    public float angryPushChance = 0.7f;
    public float angryPushHeight = 0.09f;
    public float angryPushTilt = 7f;
    public float angryReach = 14f;       // bangs even if you're across the cabin

    // the fire going out
    public Light fireLight;
    public ParticleSystem[] fireParticles;
    public AudioSource fireSound;
    public Renderer embers;
    public Material coldEmbers;

    public float flickerTime = 1.5f;     // how long the lights freak out
    public float fadeTime = 2.5f;        // then how long it takes to get dark
    public float darkMoon = 0.25f;       // how much moonlight is left

    private bool started;
    private bool flickerDone;
    private float timer;
    private float nextFlicker;
    private float moonStart;
    private float flareStart;

    void Start()
    {
        moonStart = moonlight.intensity;
        if (moonFlare != null) flareStart = moonFlare.brightness;
    }

    void Update()
    {
        if (!started)
        {
            if (tape.isPlaying) Begin();
            return;
        }

        timer += Time.deltaTime;

        // 1. lights going crazy
        if (timer < flickerTime)
        {
            if (timer >= nextFlicker)
            {
                foreach (Light l in flickerLights) l.enabled = Random.value > 0.5f;
                nextFlicker = timer + Random.Range(0.03f, 0.15f);
            }
            return;
        }

        // back on once it's over
        if (!flickerDone)
        {
            foreach (Light l in flickerLights) l.enabled = true;
            flickerDone = true;
        }

        // 2. the night sinks and stays down
        float t = Mathf.Clamp01((timer - flickerTime) / fadeTime);
        float smooth = Mathf.SmoothStep(0f, 1f, t);
        lighting.darkAmount = smooth;
        moonlight.intensity = moonStart * Mathf.Lerp(1f, darkMoon, smooth);
        if (moonFlare != null) moonFlare.brightness = flareStart * Mathf.Lerp(1f, darkMoon, smooth);
        if (nightAmbience != null) nightAmbience.pitch = Mathf.Lerp(1f, 0.8f, smooth);   // lower and slower
    }

    void Begin()
    {
        started = true;

        // the cellar goes off
        if (trapdoor != null)
        {
            trapdoor.shortestWait = angryShortestWait;
            trapdoor.longestWait = angryLongestWait;
            trapdoor.pushChance = angryPushChance;
            trapdoor.pushHeight = angryPushHeight;
            trapdoor.pushTilt = angryPushTilt;
            trapdoor.triggerDistance = angryReach;
            trapdoor.timeBetweenThumps = 0.2f;   // start right away
        }

        // the fire dies. particles stop making new flames so the old ones fade out
        if (fireLight != null) fireLight.enabled = false;
        foreach (ParticleSystem p in fireParticles) p.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        if (fireSound != null) fireSound.Stop();
        if (embers != null && coldEmbers != null) embers.sharedMaterial = coldEmbers;
    }
}
