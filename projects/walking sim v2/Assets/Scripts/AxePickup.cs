using UnityEngine;
using UnityEngine.InputSystem;

// the axe. look at it + E to grab it, left click to swing
// goes on the axe, needs a collider so the look check can hit it
public class AxePickup : MonoBehaviour
{
    public Camera playerCamera;
    public float reachDistance = 2.5f;

    // where it sits in your hands, relative to the camera
    public Vector3 holdPosition = new Vector3(0.38f, -0.42f, 0.75f);
    public Vector3 holdRotation = new Vector3(-15f, -20f, 25f);

    public float swingTime = 0.45f;   // one chop, down and back
    public float swingAngle = 70f;    // how far it chops forward
    public AudioClip swingSound;      // optional

    private bool held;
    private bool lookingAtIt;
    private float swingTimer;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!held)
        {
            // same look check as the lamp and tape
            Ray lookLine = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            lookingAtIt = Physics.Raycast(lookLine, out RaycastHit hit, reachDistance) && hit.transform.IsChildOf(transform);

            if (lookingAtIt && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                PickUp();
            }
            return;
        }

        // start a chop, only if one isn't already going
        if (swingTimer <= 0f && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            swingTimer = swingTime;
            if (audioSource != null && swingSound != null) audioSource.PlayOneShot(swingSound);
        }

        // 0 -> 1 -> 0 over the swing, so it goes down then comes back up
        float chop = 0f;
        if (swingTimer > 0f)
        {
            swingTimer -= Time.deltaTime;
            float progress = 1f - Mathf.Max(0f, swingTimer) / swingTime;
            chop = Mathf.Sin(progress * Mathf.PI);
        }
        transform.localRotation = Quaternion.Euler(holdRotation) * Quaternion.Euler(chop * swingAngle, 0f, 0f);
    }

    void PickUp()
    {
        held = true;
        lookingAtIt = false;

        // stick it to the camera so it moves with you
        transform.SetParent(playerCamera.transform, false);
        transform.localPosition = holdPosition;
        transform.localRotation = Quaternion.Euler(holdRotation);

        // turn off its colliders so it doesn't bump into you or the walls
        foreach (Collider c in GetComponentsInChildren<Collider>()) c.enabled = false;
    }

    // little hint at the bottom of the screen
    void OnGUI()
    {
        if (held || !lookingAtIt) return;
        var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 22 };
        GUI.Label(new Rect(0f, Screen.height * 0.75f, Screen.width, 40f), "Press E to pick up the axe", style);
    }
}
