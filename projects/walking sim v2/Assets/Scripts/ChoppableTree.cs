using UnityEngine;

// a tree you can chop with the axe. a few hits and it falls over, away from you,
// then it breaks into a log you can carry and leaves a stump behind
// goes on each tree (the parent object, pivot at the bottom)
public class ChoppableTree : MonoBehaviour
{
    public int hitsToFell = 3;
    public float fallTime = 1.6f;    // how long it takes to hit the ground
    public float breakDelay = 0.7f;  // how long it lies there before turning into a log
    public GameObject logPrefab;
    public Material stumpMaterial;
    public AudioClip chopSound;      // optional
    public AudioClip fallSound;      // optional

    private int hits;
    private bool falling;
    private bool fallen;
    private float shakeTimer;
    private float fallTimer;
    private float breakTimer;
    private Quaternion startRotation;
    private Vector3 fallAxis;
    private Vector3 fallDirection;
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
            fallDirection = transform.position - chopperPosition;
            fallDirection.y = 0f;
            fallDirection.Normalize();
            fallAxis = Vector3.Cross(Vector3.up, fallDirection);
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
            }
        }

        // lies there for a sec, then breaks up
        if (fallen)
        {
            breakTimer += Time.deltaTime;
            if (breakTimer >= breakDelay) BreakIntoLog();
        }
    }

    void BreakIntoLog()
    {
        // how thick the trunk was, for the stump
        Transform trunk = transform.Find("Trunk");
        float thickness = trunk != null ? trunk.lossyScale.x : 0.4f;
        Vector3 basePoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        // short stump where it stood
        GameObject stump = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        stump.name = "Stump";
        stump.transform.position = basePoint + Vector3.up * 0.18f;
        stump.transform.localScale = new Vector3(thickness, 0.18f, thickness);
        if (stumpMaterial != null) stump.GetComponent<Renderer>().sharedMaterial = stumpMaterial;

        // one log lying where the trunk came down, along the way it fell
        if (logPrefab != null)
        {
            Vector3 spot = basePoint + fallDirection * 1.4f + Vector3.up * 0.25f;
            Quaternion lying = Quaternion.LookRotation(fallDirection);   // the log prefab runs along its z
            Instantiate(logPrefab, spot, lying);
        }

        Destroy(gameObject);
    }
}
