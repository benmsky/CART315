using UnityEngine;
using UnityEngine.InputSystem;

// Moves the paddle left and right with A/D or the arrow keys.
public class Paddle : MonoBehaviour
{
    public float speed = 8f;
    public float limit = 2.5f; // how far left/right the paddle can go

    void Update()
    {
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
        }

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }

        // Keep the paddle on the table
        float x = Mathf.Clamp(transform.position.x, -limit, limit);
        transform.position = new Vector3(x, transform.position.y, 0);
    }
}
