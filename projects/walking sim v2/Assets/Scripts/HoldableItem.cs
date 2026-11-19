using UnityEngine;
using UnityEngine.InputSystem;

// anything you can pick up. look at it + E to grab it, Q drops it (PlayerHands does the Q part)
// needs a collider to look at, and a Rigidbody so it falls when you drop it
public class HoldableItem : MonoBehaviour
{
    public string itemName = "item";   // shows up in the hint
    public PlayerHands hands;
    public bool leftHand;              // the lantern goes in the left hand, everything else the right
    public float reachDistance = 2.5f;

    // where it sits in your hand, relative to the camera
    public Vector3 holdPosition = new Vector3(0.35f, -0.4f, 0.7f);
    public Vector3 holdRotation = Vector3.zero;

    public bool IsHeld { get; private set; }

    private bool lookingAtIt;
    private Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        // things spawned in later (like logs) don't know the player yet, so go find them
        if (hands == null) hands = FindFirstObjectByType<PlayerHands>();
    }

    void Update()
    {
        if (IsHeld) return;

        // same look check as the lamp and tape
        Transform cam = hands.playerCamera.transform;
        Ray lookLine = new Ray(cam.position, cam.forward);
        lookingAtIt = Physics.Raycast(lookLine, out RaycastHit hit, reachDistance) && hit.transform.IsChildOf(transform);

        if (lookingAtIt && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            PickUp();
        }
    }

    public void PickUp()
    {
        hands.Take(this);
        IsHeld = true;
        lookingAtIt = false;

        // stop physics and colliders while it's in your hand so it doesn't bump into stuff
        // interpolation off too, otherwise it keeps pulling the item back to where the physics thinks it is
        if (body != null)
        {
            body.isKinematic = true;
            body.interpolation = RigidbodyInterpolation.None;
        }
        foreach (Collider c in GetComponentsInChildren<Collider>()) c.enabled = false;

        // stick it to the camera so it moves with you
        transform.SetParent(hands.playerCamera.transform, false);
        transform.localPosition = holdPosition;
        transform.localRotation = Quaternion.Euler(holdRotation);
    }

    public void Drop()
    {
        hands.LetGo(this);
        IsHeld = false;

        // let go a little in front of you, but not inside a wall
        Transform cam = hands.playerCamera.transform;
        float distance = 0.6f;
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit wall, distance, ~0, QueryTriggerInteraction.Ignore))
            distance = Mathf.Max(0.1f, wall.distance - 0.25f);

        transform.SetParent(null, true);
        transform.position = cam.position + cam.forward * distance + Vector3.down * 0.3f;

        // physics back on so it falls to the ground
        foreach (Collider c in GetComponentsInChildren<Collider>()) c.enabled = true;
        if (body != null)
        {
            body.position = transform.position;
            body.rotation = transform.rotation;
            body.isKinematic = false;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.linearVelocity = cam.forward * 0.5f;
        }
    }

    // little hint at the bottom of the screen
    void OnGUI()
    {
        if (IsHeld || !lookingAtIt) return;
        HintUI.Prompt("E", "pick up the " + itemName);
    }
}
