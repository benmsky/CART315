using UnityEngine;
using UnityEngine.InputSystem;

public class paddleleft : MonoBehaviour
{
    private Rigidbody2D _rigidBody;
    public float speed = 10f;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Vector2 direction = Vector2.zero;

        if (Keyboard.current.upArrowKey.isPressed)
        {
            direction = Vector2.up;
        }
        else if (Keyboard.current.downArrowKey.isPressed)
        {
            direction = Vector2.down;
        }

        if (direction != Vector2.zero)
        {
            _rigidBody.linearVelocity = direction * speed;
        }
        else
        {
            _rigidBody.linearVelocity = Vector2.zero;
        }
    }
}
