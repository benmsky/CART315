using UnityEngine;
using UnityEngine.InputSystem;

// A lamp you can switch on and off. Look at it and press E.
// Put this on the lamp, with an AudioSource and a collider to look at.
public class LampSwitch : MonoBehaviour
{
    public Camera playerCamera;
    public Light lampLight;
    public Renderer shade;         // the lampshade, which glows when the lamp is on
    public Material shadeOn;
    public Material shadeOff;
    public AudioClip onSound;      // plays when the lamp turns on
    public AudioClip offSound;     // plays when the lamp turns off
    public float reachDistance = 2.5f;

    private AudioSource audioSource;
    private bool lookingAtIt;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // shoot an invisible line straight out from the middle of the screen
        Ray lookLine = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        lookingAtIt = Physics.Raycast(lookLine, out RaycastHit hit, reachDistance) && hit.transform.IsChildOf(transform);

        if (lookingAtIt && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            lampLight.enabled = !lampLight.enabled;
            shade.sharedMaterial = lampLight.enabled ? shadeOn : shadeOff;
            audioSource.PlayOneShot(lampLight.enabled ? onSound : offSound);
        }
    }

    // a small hint at the bottom of the screen while you're looking at the lamp
    void OnGUI()
    {
        if (!lookingAtIt) return;
        var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 22 };
        string hint = lampLight.enabled ? "Press E to turn the lamp off" : "Press E to turn the lamp on";
        GUI.Label(new Rect(0f, Screen.height * 0.75f, Screen.width, 40f), hint, style);
    }
}
