using UnityEngine;

// Makes a light flicker like a fireplace or an old oil lamp.
// Put this on any GameObject that has a Light component.
public class LightFlicker : MonoBehaviour
{
    public float minIntensity = 1f;
    public float maxIntensity = 2f;
    public float timeBetweenFlickers = 0.1f; // seconds

    private Light myLight;
    private float timer;
    private float targetIntensity;

    void Start()
    {
        myLight = GetComponent<Light>();
        targetIntensity = myLight.intensity;
    }

    void Update()
    {
        // every little while, pick a new random brightness
        timer += Time.deltaTime;
        if (timer >= timeBetweenFlickers)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity);
            timer = 0f;
        }

        // slide toward that brightness so it doesn't look too jumpy
        myLight.intensity = Mathf.Lerp(myLight.intensity, targetIntensity, Time.deltaTime * 10f);
    }
}
