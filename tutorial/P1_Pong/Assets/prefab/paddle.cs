using UnityEngine;
using UnityEngine.InputSystem;

public class paddle : MonoBehaviour
{
    private Rigidbody2D _rigidBody;

    public float speed = 10.0f;

    public Vector2 direction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        direction = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
        {
            direction = Vector2.up;
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            direction = Vector2.down;
        }
        
    }

    private void FixedUpdate()
    {
        if (direction.sqrMagnitude == 0) return;
        _rigidBody.AddForce(direction * speed);
    }
}
