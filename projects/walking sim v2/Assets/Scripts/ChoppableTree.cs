using UnityEngine;

// a tree you can chop with the axe. a few hits and it falls over, away from you
// goes on each tree (the parent object, pivot at the bottom)
public class ChoppableTree : MonoBehaviour
{
    public int hitsToFell = 3;
    public float fallTime = 1.6f;    // how long it takes to hit the ground
    public AudioClip chopSound;      // optional
    public AudioClip fallSound;      // optional

    private int hits;
    private bool falling;
    private bool fallen;
    private float shakeTimer;
    private float fallTimer;
    private Quaternion startRotation;
    private Vector3 fallAxis;
    private Vector3 shakeAxis;

    void Start()
    {
        startRotation = transform.rotation;
    }

    // the axe calls this when it hits. chopperPosition = where the player is
    public void Chop(Vector3 chopperPosition)
    {
        if (falling || fallen) return;

        hits++;
        shakeTimer = 0.3f;
        shakeAxis = Random.onUnitSphere;
        if (chopSound != null) AudioSource.PlayClipAtPoint(chopSound, transform.position + Vector3.up, 0.8f);

        if (hits >= hitsToFell)
        {
            // tip it over away from the player
            Vector3 away = transform.position - chopperPosition;
            away.y = 0f;
            away.Normalize();
            fallAxis = Vector3.Cross(Vector3.up, away);
            falling = true;
            shakeTimer = 0f;

            // colliders off while it falls so it doesn't shove you around
            foreach (Collider c in GetComponentsInChildren<Collider>()) c.enabled = false;
            if (fallSound != null) AudioSource.PlayClipAtPoint(fallSound, transform.position + Vector3.up * 2f);
        }
    }

    void Update()
    {
        // little wobble after each hit
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            float wobble = Mathf.Sin(shakeTimer * 70f) * 2f * (shakeTimer / 0.3f);
            transform.rotation = Quaternion.AngleAxis(wobble, shakeAxis) * startRotation;
            if (shakeTimer <= 0f) transform.rotation = startRotation;
        }

        // falling speeds up as it goes, like it's really tipping
        if (falling)
        {
            fallTimer += Time.deltaTime;
            float t = Mathf.Clamp01(fallTimer / fallTime);
            transform.rotation = Quaternion.AngleAxis(88f * t * t, fallAxis) * startRotation;

            if (t >= 1f)
            {
                falling = false;
                fallen = true;
                foreach (Collider c in GetComponentsInChildren<Collider>()) c.enabled = true;
            }
        }
    }
}
