using UnityEngine;
using UnityEngine.InputSystem;

// once the fire's out, bring a log back, look at the fireplace and press E to throw it on.
// the log stays in the grate and the fire comes back
// goes on the fireplace
public class FireplaceLighter : MonoBehaviour
{
    public PlayerHands hands;
    public float reachDistance = 2.5f;
    public Transform logSpot;            // where the log ends up in the grate

    // the fire bits to turn back on
    public Light fireLight;
    public ParticleSystem[] fireParticles;
    public AudioSource fireSound;
    public Renderer embers;
    public Material hotEmbers;

    private bool lookingAtIt;

    void Update()
    {
        lookingAtIt = false;

        // only works with a log in your hand and the fire out
        HoldableItem held = hands.rightItem;
        bool holdingLog = held != null && held.itemName == "log";
        if (!holdingLog || fireLight.enabled) return;

        Transform cam = hands.playerCamera.transform;
        Ray lookLine = new Ray(cam.position, cam.forward);
        lookingAtIt = Physics.Raycast(lookLine, out RaycastHit hit, reachDistance, ~0, QueryTriggerInteraction.Ignore)
                      && hit.transform.IsChildOf(transform);

        if (lookingAtIt && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ThrowOn(held);
        }
    }

    void ThrowOn(HoldableItem log)
    {
        hands.LetGo(log);

        // it's firewood now, not something you can grab again
        Destroy(log.GetComponent<Rigidbody>());
        foreach (Collider c in log.GetComponentsInChildren<Collider>()) Destroy(c);
        Destroy(log);
        log.transform.SetParent(logSpot, false);
        log.transform.localPosition = Vector3.zero;
        log.transform.localRotation = Quaternion.identity;

        // fire back on
        fireLight.enabled = true;
        foreach (ParticleSystem p in fireParticles) p.Play();
        if (fireSound != null) fireSound.Play();
        if (embers != null && hotEmbers != null) embers.sharedMaterial = hotEmbers;
    }

    void OnGUI()
    {
        if (!lookingAtIt) return;
        var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 22 };
        GUI.Label(new Rect(0f, Screen.height * 0.75f, Screen.width, 40f), "Press E to throw the log on the fire", style);
    }
}
