using UnityEngine;
using UnityEngine.InputSystem;

// The tape recorder. Look at it and press E to play the tape, press E again to stop it.
// The reels spin and the red button stays pushed in while the tape plays.
// You can only hear it inside the cabin: it fades out when you step outside.
// Put this on the recorder, with an AudioSource holding the recording and a collider to look at.
public class TapeRecorder : MonoBehaviour
{
    public Camera playerCamera;
    public Transform playButton;
    public Transform leftReel;
    public Transform rightReel;
    public float reachDistance = 2.5f; // how close you have to be to press it
    public float reelSpeed = 200f;     // degrees per second
    public Transform player;
    public Bounds cabinArea = new Bounds(new Vector3(0f, 2f, 0f), new Vector3(12f, 4f, 10f));
    public float fadeSpeed = 3f;

    private AudioSource audioSource;
    private Vector3 buttonUpPosition;
    private bool lookingAtIt;
    private float fullVolume;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        buttonUpPosition = playButton.localPosition;
        fullVolume = audioSource.volume;
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

        // full volume inside the cabin, silent outside, sliding between the two
        float targetVolume = cabinArea.Contains(player.position) ? fullVolume : 0f;
        audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, Time.deltaTime * fadeSpeed);

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
        HintUI.Prompt("E", audioSource.isPlaying ? "stop the tape" : "play the tape");
    }
}
