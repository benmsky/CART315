using UnityEngine;
using UnityEngine.InputSystem;

// the axe. look at it + E to grab it, left click to swing
// hits trees in front of you halfway through the swing
// goes on the axe, needs a collider so the look check can hit it
public class AxePickup : MonoBehaviour
{
    public Camera playerCamera;
    public float reachDistance = 2.5f;
    public float chopReach = 2.2f;    // how close a tree has to be to get hit

    // where it sits in your hands, relative to the camera
    public Vector3 holdPosition = new Vector3(0.38f, -0.42f, 0.75f);
    public Vector3 holdRotation = new Vector3(-15f, -20f, 25f);

    public float swingTime = 0.45f;   // one chop, down and back
    public float swingAngle = 70f;    // how far it chops forward
    public AudioClip swingSound;      // optional
    public Light glow;                // faint light so it stands out on the wall, off once you grab it

    private bool held;
    private bool lookingAtIt;
    private bool lookingAtTree;
    private bool hitThisSwing;
    private float swingTimer;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        Ray lookLine = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (!held)
        {
            // same look check as the lamp and tape
            lookingAtIt = Physics.Raycast(lookLine, out RaycastHit hit, reachDistance) && hit.transform.IsChildOf(transform);

            if (lookingAtIt && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                PickUp();
            }
            return;
        }

        // is there a tree close enough to chop
        ChoppableTree tree = TreeInFront(lookLine);
        lookingAtTree = tree != null;

        // start a chop, only if one isn't already going
        if (swingTimer <= 0f && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            swingTimer = swingTime;
            hitThisSwing = false;
            if (audioSource != null && swingSound != null) audioSource.PlayOneShot(swingSound);
        }

        // 0 -> 1 -> 0 over the swing, so it goes down then comes back up
        float chop = 0f;
        if (swingTimer > 0f)
        {
            swingTimer -= Time.deltaTime;
            float progress = 1f - Mathf.Max(0f, swingTimer) / swingTime;
            chop = Mathf.Sin(progress * Mathf.PI);

            // halfway = the blade's all the way down, that's the hit
            if (progress >= 0.5f && !hitThisSwing)
            {
                hitThisSwing = true;
                if (tree != null) tree.Chop(playerCamera.transform.position);
            }
        }
        transform.localRotation = Quaternion.Euler(holdRotation) * Quaternion.Euler(chop * swingAngle, 0f, 0f);
    }

    ChoppableTree TreeInFront(Ray lookLine)
    {
        // ignore triggers so the curtain and stuff don't get in the way
        if (Physics.Raycast(lookLine, out RaycastHit hit, chopReach, ~0, QueryTriggerInteraction.Ignore))
            return hit.transform.GetComponentInParent<ChoppableTree>();
        return null;
    }

    void PickUp()
    {
        held = true;
        lookingAtIt = false;

        // stick it to the camera so it moves with you
        transform.SetParent(playerCamera.transform, false);
        transform.localPosition = holdPosition;
        transform.localRotation = Quaternion.Euler(holdRotation);
        if (glow != null) glow.enabled = false;

        // turn off its colliders so it doesn't bump into you or the walls
        foreach (Collider c in GetComponentsInChildren<Collider>()) c.enabled = false;
    }

    // little hints at the bottom of the screen
    void OnGUI()
    {
        string hint = null;
        if (!held && lookingAtIt) hint = "Press E to pick up the axe";
        if (held && lookingAtTree) hint = "Click to chop";
        if (hint == null) return;

        var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 22 };
        GUI.Label(new Rect(0f, Screen.height * 0.75f, Screen.width, 40f), hint, style);
    }
}
