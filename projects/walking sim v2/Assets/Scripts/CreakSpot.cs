using UnityEngine;

// A loose floorboard. It creaks once when the player steps on it,
// and can't creak again until the player has stepped off.
// Put this on an object sitting on the floor, with an AudioSource.
public class CreakSpot : MonoBehaviour
{
    public Transform player;
    public AudioClip[] creakSounds;  // picks one at random each time
    public float boardWidth = 1.2f;  // size of the board in metres, left to right
    public float boardLength = 1.2f; // and front to back
    public float cooldown = 0f;      // seconds before it's allowed to creak again

    private AudioSource audioSource;
    private bool playerWasOnBoard;
    private float nextCreakTime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // only measure flat across the floor, so the player's height doesn't matter
        Vector3 toPlayer = player.position - transform.position;
        bool playerOnBoard = Mathf.Abs(toPlayer.x) < boardWidth / 2f && Mathf.Abs(toPlayer.z) < boardLength / 2f;

        // only creak on the moment the player steps on, not the whole time they stand there
        if (playerOnBoard && !playerWasOnBoard && Time.time >= nextCreakTime)
        {
            AudioClip creak = creakSounds[Random.Range(0, creakSounds.Length)];
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(creak);
            nextCreakTime = Time.time + cooldown;
        }

        playerWasOnBoard = playerOnBoard;
    }
}
