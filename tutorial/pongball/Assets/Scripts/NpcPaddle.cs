using UnityEngine;

// The old Pong paddle stuck in the timeout box.
// It bobs up and down like it's impatiently waiting for its turn.
public class NpcPaddle : MonoBehaviour
{
    public float bobSpeed = 3f;
    public float bobHeight = 0.1f;

    Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = startPosition + new Vector3(0, y, 0);
    }
}
