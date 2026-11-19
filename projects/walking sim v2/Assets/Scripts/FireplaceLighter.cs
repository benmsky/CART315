using UnityEngine;
using UnityEngine.InputSystem;

// once the fire's out, bring logs back, look at the fireplace and press E to throw each one on.
// it takes 3 logs to get it going again, and once it's lit the cellar finally shuts up
// goes on the fireplace
public class FireplaceLighter : MonoBehaviour
{
    public PlayerHands hands;
    public float reachDistance = 2.5f;
    public int logsNeeded = 3;
    public Transform logSpot;            // middle of the grate

    // where each log sits, two side by side and one on top
    public Vector3[] stackSpots = { new Vector3(-0.12f, -0.04f, 0f), new Vector3(0.12f, -0.04f, 0f), new Vector3(0f, 0.2f, 0f) };

    // the fire bits to turn back on
    public Light fireLight;
    public ParticleSystem[] fireParticles;
    public AudioSource fireSound;
    public Renderer embers;
    public Material hotEmbers;

    public TrapdoorRattle trapdoor;      // calms down and goes quiet once the fire's lit

    private int logsIn;
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
        log.transform.localPosition = stackSpots[Mathf.Min(logsIn, stackSpots.Length - 1)];
        log.transform.localRotation = Quaternion.identity;
        logsIn++;

        if (logsIn >= logsNeeded) LightFire();
    }

    void LightFire()
    {
        fireLight.enabled = true;
        foreach (ParticleSystem p in fireParticles) p.Play();
        if (fireSound != null) fireSound.Play();
        if (embers != null && hotEmbers != null) embers.sharedMaterial = hotEmbers;

        // whatever's in the cellar gives up
        if (trapdoor != null) trapdoor.FadeOut();
    }

    void OnGUI()
    {
        if (!lookingAtIt) return;
        var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 22 };
        string hint = "Press E to throw the log on the fire (" + logsIn + "/" + logsNeeded + ")";
        GUI.Label(new Rect(0f, Screen.height * 0.75f, Screen.width, 40f), hint, style);
    }
}
