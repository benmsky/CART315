using UnityEngine;
using UnityEngine.InputSystem;

// The tape recorder. Look at it and press E to play the tape, press E again to stop it.
// The reels spin and the red button stays pushed in while the tape plays.
// Put this on the recorder, with an AudioSource holding the recording and a collider to look at.
public class TapeRecorder : MonoBehaviour
{
    public Camera playerCamera;
    public Transform playButton;
    public Transform leftReel;
    public Transform rightReel;
    public float reachDistance = 2.5f; // how close you have to be to press it
    public float reelSpeed = 200f;     // degrees per second

    private AudioSource audioSource;
    private Vector3 buttonUpPosition;
    private bool lookingAtIt;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        buttonUpPosition = playButton.localPosition;
    }

    void Update()
    {
        // shoot an invisible line straight out from the middle of the screen
        Ray lookLine = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        lookingAtIt = Physics.Raycast(lookLine, out RaycastHit hit, reachDistance) && hit.transform.IsChildOf(transform);

        if (lookingAtIt && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (audioSource.isPlaying) audioSource.Stop();
            else audioSource.Play();
        }

        if (audioSource.isPlaying)
        {
            // the tape winds from the full reel onto the empty one, which has to spin faster to keep up
            leftReel.Rotate(0f, -reelSpeed * Time.deltaTime, 0f);
            rightReel.Rotate(0f, -reelSpeed * 1.4f * Time.deltaTime, 0f);
            playButton.localPosition = buttonUpPosition + Vector3.down * 0.006f;
        }
        else
        {
            playButton.localPosition = buttonUpPosition;
        }
    }

    // a small hint at the bottom of the screen while you're looking at the recorder
    void OnGUI()
    {
        if (!lookingAtIt) return;
        var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 22 };
        string hint = audioSource.isPlaying ? "Press E to stop the tape" : "Press E to play the tape";
        GUI.Label(new Rect(0f, Screen.height * 0.75f, Screen.width, 40f), hint, style);
    }
}
