using UnityEngine;
using UnityEngine.SceneManagement;

// Keeps track of the goons in the room and opens the exit once they're all down.
// Getting to the open exit loads one of the other rooms at random.
public class VentLevel : MonoBehaviour
{
    public AudioClip punchSound; // drag the sounds here in the Inspector
    public AudioClip alertSound;
    public AudioClip winSound;

    public bool playing = true;

    int goons = 0;
    int goonsDown = 0;
    bool exitOpen = false;
    int nextRoom = 0;
    string message = "";

    void Start()
    {
        playing = true;

        // Count how many goons are in this room
        goons = GameObject.FindGameObjectsWithTag("Goon").Length;

        // Make the exit dark so you know it's locked
        GameObject[] exits = GameObject.FindGameObjectsWithTag("Exit");
        foreach (GameObject exit in exits)
        {
            exit.GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f);
        }

        message = "Take down all the goons then get to the exit";
    }

    public void GoonDown()
    {
        goonsDown++;
        AudioSource.PlayClipAtPoint(punchSound, Camera.main.transform.position);

        if (goonsDown == goons)
        {
            exitOpen = true;
            message = "All goons down! The exit is open";

            GameObject[] exits = GameObject.FindGameObjectsWithTag("Exit");
            foreach (GameObject exit in exits)
            {
                exit.GetComponent<SpriteRenderer>().color = Color.white;
            }

            AudioSource.PlayClipAtPoint(winSound, Camera.main.transform.position);
        }
    }

    public void Spotted()
    {
        if (playing == false)
        {
            return;
        }

        playing = false;
        message = "!";
        AudioSource.PlayClipAtPoint(alertSound, Camera.main.transform.position);
        Invoke("Restart", 1f);
    }

    public void ReachedExit()
    {
        if (exitOpen == false)
        {
            message = "The exit is locked! Take down all the goons first";
            return;
        }

        if (playing == false)
        {
            return;
        }

        playing = false;
        message = "Room clear!";

        // Pick a different room from the build settings to go to next
        int thisRoom = SceneManager.GetActiveScene().buildIndex;
        nextRoom = Random.Range(0, SceneManager.sceneCountInBuildSettings);
        while (nextRoom == thisRoom)
        {
            nextRoom = Random.Range(0, SceneManager.sceneCountInBuildSettings);
        }

        Invoke("LoadNextRoom", 1.5f);

        // Play the sound last so if it's missing it doesn't stop the next room loading
        AudioSource.PlayClipAtPoint(winSound, Camera.main.transform.position);
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void LoadNextRoom()
    {
        SceneManager.LoadScene(nextRoom);
    }

    void OnGUI()
    {
        // Show the room, goon count and messages on screen
        GUI.skin.label.fontSize = 24;
        GUI.Label(new Rect(10, 10, 600, 40), SceneManager.GetActiveScene().name + "    Goons: " + goonsDown + "/" + goons);
        GUI.Label(new Rect(10, 45, 900, 40), message);
    }
}
