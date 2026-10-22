using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Moves Snake through the vents one square at a time (based on the snake tutorial).
// His tail is his bandana, and it gets longer every time he takes down a goon.
public class Snake : MonoBehaviour
{
    public Transform segmentPrefab; // drag the PlayerSegment prefab here in the Inspector
    public Vector2Int direction = Vector2Int.right;
    public float speed = 6f; // squares per second
    public int initialSize = 2;

    List<Transform> segments = new List<Transform>();
    List<Vector3> from = new List<Vector3>(); // where each piece is sliding from
    List<Vector3> to = new List<Vector3>(); // where each piece is sliding to
    List<Vector2Int> turns = new List<Vector2Int>(); // saves up to 2 turns so quick key presses don't get lost
    float stepTimer = 0f;
    bool started = false;
    VentLevel level;

    void Start()
    {
        level = FindFirstObjectByType<VentLevel>();

        segments.Add(transform);
        from.Add(transform.position);
        to.Add(transform.position);

        // Make the starting bandana behind the head
        for (int i = 1; i < initialSize; i++)
        {
            Transform segment = Instantiate(segmentPrefab);
            segment.position = transform.position - new Vector3(i, 0, 0);
            segments.Add(segment);
            from.Add(segment.position);
            to.Add(segment.position);
        }
    }

    void Update()
    {
        // Snake doesn't move until you press a key
        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            started = true;
        }

        if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            AddTurn(Vector2Int.up);
        }

        if (Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            AddTurn(Vector2Int.down);
        }

        if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            AddTurn(Vector2Int.right);
        }

        if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            AddTurn(Vector2Int.left);
        }

        if (started == false || level.playing == false)
        {
            return;
        }

        // Count up how far through this step we are (0 = start, 1 = done)
        stepTimer += Time.deltaTime * speed;

        if (stepTimer >= 1f)
        {
            stepTimer = 0f;
            NextStep();
        }

        // Slide every piece part of the way between its two squares
        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].position = Vector3.Lerp(from[i], to[i], stepTimer);
        }
    }

    void AddTurn(Vector2Int newDirection)
    {
        // Check against the last saved turn, or the way Snake is going now
        Vector2Int last = direction;
        if (turns.Count > 0)
        {
            last = turns[turns.Count - 1];
        }

        // Snake can only turn sideways, not go straight back or keep going the same way
        bool sideways = (last.x != 0 && newDirection.y != 0) || (last.y != 0 && newDirection.x != 0);

        if (sideways && turns.Count < 2)
        {
            turns.Add(newDirection);
        }
    }

    void NextStep()
    {
        // Use the next saved turn if there is one
        if (turns.Count > 0)
        {
            direction = turns[0];
            turns.RemoveAt(0);
        }

        // Everything starts where it ended up last step
        for (int i = 0; i < segments.Count; i++)
        {
            from[i] = to[i];
        }

        // Each bandana piece heads for where the piece in front of it was
        for (int i = segments.Count - 1; i > 0; i--)
        {
            to[i] = from[i - 1];
        }

        // The head heads for the next square
        to[0] = from[0] + new Vector3(direction.x, direction.y, 0);

        // A bandana piece is going to be in that square = Snake crashed into himself
        for (int i = 1; i < segments.Count; i++)
        {
            if (to[i] == to[0])
            {
                level.Spotted();
            }
        }

        // Turn the head the way Snake is going
        if (direction == Vector2Int.right)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        if (direction == Vector2Int.up)
        {
            transform.eulerAngles = new Vector3(0, 0, 90);
        }

        if (direction == Vector2Int.left)
        {
            transform.eulerAngles = new Vector3(0, 0, 180);
        }

        if (direction == Vector2Int.down)
        {
            transform.eulerAngles = new Vector3(0, 0, 270);
        }

        // Turn the bandana pieces so they go sideways or up and down
        for (int i = 1; i < segments.Count; i++)
        {
            Vector3 diff = to[i] - from[i];

            if (diff.x != 0)
            {
                segments[i].eulerAngles = new Vector3(0, 0, 0);
            }
            else if (diff.y != 0)
            {
                segments[i].eulerAngles = new Vector3(0, 0, 90);
            }
        }
    }

    void Grow()
    {
        // The new piece waits where the tail started this step, then follows next step
        Transform segment = Instantiate(segmentPrefab);
        Vector3 tailStart = from[from.Count - 1];
        segment.position = tailStart;
        segment.rotation = segments[segments.Count - 1].rotation;
        segments.Add(segment);
        from.Add(tailStart);
        to.Add(tailStart);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Goon")
        {
            other.GetComponent<Goon>().KnockOut();
            Grow();
            level.GoonDown();
        }

        if (other.tag == "Exit")
        {
            level.ReachedExit();
        }

        // Hitting a wall gets you spotted (crashing into your own bandana is checked in NextStep)
        if (other.tag == "Wall")
        {
            level.Spotted();
        }
    }
}
