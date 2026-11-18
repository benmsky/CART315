using UnityEngine;

// Rocks the porch swing back and forth just a tiny bit.
// Put this on the swing's pivot (where the chains hang from).
public class Swing : MonoBehaviour
{
    public float swingAngle = 1.2f;      // degrees, keep it tiny
    public float secondsPerSwing = 3.5f; // how long one rock there and back takes

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    void Update()
    {
        // Mathf.Sin goes smoothly between -1 and 1, which makes a gentle rock
        float rock = Mathf.Sin(Time.time / secondsPerSwing * Mathf.PI * 2f);
        transform.localRotation = startRotation * Quaternion.Euler(rock * swingAngle, 0f, 0f);
    }
}
