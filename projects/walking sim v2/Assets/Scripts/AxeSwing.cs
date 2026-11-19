using UnityEngine;
using UnityEngine.InputSystem;

// swinging the axe. left click to chop while you're holding it (HoldableItem does the picking up)
// hits trees in front of you halfway through the swing
// goes on the axe, next to HoldableItem
public class AxeSwing : MonoBehaviour
{
    public float chopReach = 2.2f;    // how close a tree has to be to get hit
    public float swingTime = 0.45f;   // one chop, down and back
    public float swingAngle = 70f;    // how far it chops forward
    public AudioClip swingSound;      // optional

    private HoldableItem holdable;
    private bool lookingAtTree;
    private bool hitThisSwing;
    private float swingTimer;
    private AudioSource audioSource;

    void Start()
    {
        holdable = GetComponent<HoldableItem>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!holdable.IsHeld)
        {
            lookingAtTree = false;
            swingTimer = 0f;
            return;
        }

        Transform cam = holdable.hands.playerCamera.transform;
        Ray lookLine = new Ray(cam.position, cam.forward);

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
                if (tree != null) tree.Chop(cam.position);
            }
        }
        transform.localRotation = Quaternion.Euler(holdable.holdRotation) * Quaternion.Euler(chop * swingAngle, 0f, 0f);
    }

    ChoppableTree TreeInFront(Ray lookLine)
    {
        // ignore triggers so the curtain and stuff don't get in the way
        if (Physics.Raycast(lookLine, out RaycastHit hit, chopReach, ~0, QueryTriggerInteraction.Ignore))
            return hit.transform.GetComponentInParent<ChoppableTree>();
        return null;
    }

    void OnGUI()
    {
        if (!lookingAtTree) return;
        HintUI.Prompt("Click", "chop");
    }
}
