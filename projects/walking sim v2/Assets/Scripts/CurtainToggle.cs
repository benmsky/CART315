using UnityEngine;
using UnityEngine.InputSystem;

// A curtain you can open and close. Look at it and press E.
// Put this on the curtain, with an AudioSource and a collider to look at.
// Each panel's pivot is at its outer edge, so squashing it sideways bunches it up at the side of the window.
public class CurtainToggle : MonoBehaviour
{
    public Camera playerCamera;
    public Transform leftPanel;
    public Transform rightPanel;
    public Light moonlight;          // the light coming in the window, dimmer while the curtain is closed
    public AudioClip openSound;
    public AudioClip closeSound;
    public float openWidth = 0.25f;  // how wide each panel is when pushed aside (1 = fully closed)
    public float speed = 1.6f;       // how fast it slides
    public float reachDistance = 3f;
    public bool isOpen = false;

    private AudioSource audioSource;
    private bool lookingAtIt;
    private float amountOpen;        // 0 = closed, 1 = open
    private float moonBrightness;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        moonBrightness = moonlight.intensity;
        amountOpen = isOpen ? 1f : 0f;
    }

    void Update()
    {
        // shoot an invisible line straight out from the middle of the screen
        Ray lookLine = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        lookingAtIt = Physics.Raycast(lookLine, out RaycastHit hit, reachDistance) && hit.transform.IsChildOf(transform);

        if (lookingAtIt && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            isOpen = !isOpen;
            audioSource.PlayOneShot(isOpen ? openSound : closeSound);
        }

        // slide a little bit each frame towards open or closed
        amountOpen = Mathf.MoveTowards(amountOpen, isOpen ? 1f : 0f, speed * Time.deltaTime);
        float smooth = Mathf.SmoothStep(0f, 1f, amountOpen); // starts and stops gently

        float width = Mathf.Lerp(1f, openWidth, smooth);
        leftPanel.localScale = new Vector3(width, 1f, 1f);
        rightPanel.localScale = new Vector3(width, 1f, 1f);
        moonlight.intensity = moonBrightness * Mathf.Lerp(0.15f, 1f, smooth);
    }

    // a small hint at the bottom of the screen while you're looking at the curtain
    void OnGUI()
    {
        if (!lookingAtIt) return;
        HintUI.Prompt("E", isOpen ? "close the curtain" : "open the curtain");
    }
}
