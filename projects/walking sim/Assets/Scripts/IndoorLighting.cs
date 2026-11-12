using UnityEngine;

// Makes the inside of the cabin darker and less foggy than the woods outside.
// The night's blue ambient light, the sky's reflection and the fog fill everything equally, which made the rooms look grey,
// so while the player is inside they're turned down and the lamps do the lighting.
// Put this on any empty object.
public class IndoorLighting : MonoBehaviour
{
    public Transform player;

    // the box of space that counts as "inside the cabin" (same as MuffleIndoors)
    public Bounds cabinArea = new Bounds(new Vector3(0f, 2f, 0f), new Vector3(12f, 4f, 10f));

    public float indoorAmbient = 0.3f;   // 0.3 = 30% of the outdoor ambient light
    public float indoorReflections = 0.3f; // how much of the sky shows up as a sheen on surfaces
    public float indoorFog = 0.4f;       // 0.4 = 40% of the outdoor fog
    public float fadeSpeed = 3f;

    private Color outdoorAmbient;
    private float outdoorFogDensity;
    private float outdoorReflections;
    private float insideAmount;          // 0 = outside, 1 = inside

    void Start()
    {
        outdoorAmbient = RenderSettings.ambientLight;
        outdoorFogDensity = RenderSettings.fogDensity;
        outdoorReflections = RenderSettings.reflectionIntensity;
    }

    void Update()
    {
        bool inside = cabinArea.Contains(player.position);

        // slide toward the new look so it doesn't snap when you cross the door
        insideAmount = Mathf.Lerp(insideAmount, inside ? 1f : 0f, Time.deltaTime * fadeSpeed);

        RenderSettings.ambientLight = outdoorAmbient * Mathf.Lerp(1f, indoorAmbient, insideAmount);
        RenderSettings.fogDensity = outdoorFogDensity * Mathf.Lerp(1f, indoorFog, insideAmount);
        RenderSettings.reflectionIntensity = outdoorReflections * Mathf.Lerp(1f, indoorReflections, insideAmount);
    }

    // put the outdoor values back when the game stops, so the scene isn't left dark
    void OnDisable()
    {
        RenderSettings.ambientLight = outdoorAmbient;
        RenderSettings.fogDensity = outdoorFogDensity;
        RenderSettings.reflectionIntensity = outdoorReflections;
    }
}
