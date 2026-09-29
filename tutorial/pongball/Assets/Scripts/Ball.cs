using UnityEngine;
using UnityEngine.UI;

// Launches the ball, bounces it off bumpers and the paddle, and keeps score.
public class Ball : MonoBehaviour
{
    public float bumperPush = 8f;
    public int score = 0;
    public Text scoreText; // drag the ScoreText object here in the Inspector

    Rigidbody2D rb;
    Vector3 startPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        ResetBall();
    }

    void Update()
    {
        // The ball fell past the paddle
        if (transform.position.y < -6)
        {
            score = 0;
            ResetBall();
        }

        // Show the score on screen
        scoreText.text = "Score: " + score;
    }

    void ResetBall()
    {
        transform.position = startPosition;
        rb.linearVelocity = new Vector2(Random.Range(-3f, 3f), 2f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Bumper")
        {
            // Push the ball away from the bumper
            Vector2 away = (transform.position - collision.transform.position).normalized;
            rb.linearVelocity = away * bumperPush;
            score += 10;
        }

        if (collision.gameObject.name == "Paddle")
        {
            // Hit the left side of the paddle = go left, right side = go right (like Pong)
            float offset = transform.position.x - collision.transform.position.x;
            rb.linearVelocity = new Vector2(offset * 5f, 8f);
            score += 1;
        }
    }
}
