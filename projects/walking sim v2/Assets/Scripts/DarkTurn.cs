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
    public float angryShortestWait = 0.12f;  // basically nonstop
    public float angryLongestWait = 0.35f;
    public float angryPushChance = 0.6f;
    public float angryPushTime = 0.22f;      // quick hard shoves
    public float angryPushHeight = 0.09f;
    public float angryPushTilt = 7f;
    public float angryReach = 60f;       // keeps going wherever you are, it just gets fainter
    public float fullAngerBeforeEnd = 1f; // hits full speed this many seconds before the tape finishes
    public float rampCurve = 3f;         // higher = stays slow longer, then shoots up at the end

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

    // what the cellar was like before the tape, so it can build from there
    private float calmShortest, calmLongest, calmPushChance, calmPushHeight, calmPushTilt, calmPushTime;
    private float rampTime;

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
        BuildUpCellar();

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

        // remember how the cellar is now, it builds up from this over the tape
        if (trapdoor != null)
        {
            calmShortest = trapdoor.shortestWait;
            calmLongest = trapdoor.longestWait;
            calmPushChance = trapdoor.pushChance;
            calmPushHeight = trapdoor.pushHeight;
            calmPushTilt = trapdoor.pushTilt;
            calmPushTime = trapdoor.pushTime;
            trapdoor.triggerDistance = angryReach;
        }
        float tapeLength = tape.clip != null ? tape.clip.length : 12f;
        rampTime = Mathf.Max(1f, tapeLength - fullAngerBeforeEnd);

        // the fire dies. particles stop making new flames so the old ones fade out
        if (fireLight != null) fireLight.enabled = false;
        foreach (ParticleSystem p in fireParticles) p.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        if (fireSound != null) fireSound.Stop();
        if (embers != null && coldEmbers != null) embers.sharedMaterial = coldEmbers;
    }

    // slowly at first, then faster and faster until it's going nuts right before the tape ends
    void BuildUpCellar()
    {
        if (trapdoor == null || trapdoor.IsCalming) return;   // the fire's doing its thing, leave it alone
        float anger = Mathf.Pow(Mathf.Clamp01(timer / rampTime), rampCurve);

        trapdoor.shortestWait = Mathf.Lerp(calmShortest, angryShortestWait, anger);
        trapdoor.longestWait = Mathf.Lerp(calmLongest, angryLongestWait, anger);
        trapdoor.pushChance = Mathf.Lerp(calmPushChance, angryPushChance, anger);
        trapdoor.pushHeight = Mathf.Lerp(calmPushHeight, angryPushHeight, anger);
        trapdoor.pushTilt = Mathf.Lerp(calmPushTilt, angryPushTilt, anger);
        trapdoor.pushTime = Mathf.Lerp(calmPushTime, angryPushTime, anger);

        // if it already picked a long wait, cut it short so it keeps up with the build
        trapdoor.timeBetweenThumps = Mathf.Min(trapdoor.timeBetweenThumps, trapdoor.longestWait);
    }
}
