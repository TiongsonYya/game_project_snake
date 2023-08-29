using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    public Transform segmentPrefab;
    public Vector2Int direction = Vector2Int.right; // sets up the initial movement direction for the snake in a 2D game.It makes the snake start moving to the right by default.
    public float speed = 20f;
    public float speedMultiplier = 1f;
    public int initialSize = 1;
    public bool moveThroughWalls = false;

    private List<Transform> segments = new List<Transform>();//to store the snake's body parts as Transform components. This allows the script to manage the snake's length, movement, and appearance.
    private Vector2Int input;
    private float nextUpdate;
    public TMP_Text scoreText;
    public int score = 0;

    private void Start()
    {
        ResetState();
        UpdateScoreText();
    }

    private void UpdateScoreText()
{
    scoreText.text = "Score: " + score;
}
    private void Update()
    {
        // Only allow turning up or down while moving in the x-axis
        if (direction.x != 0f) //This checks if the snake is currently moving along the x-axis (left or right). This condition is used to prevent turning up or down while moving horizontally.
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                input = Vector2Int.up;
            } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                input = Vector2Int.down;
            }
        }
        // Only allow turning left or right while moving in the y-axis
        else if (direction.y != 0f)//This checks if the snake is currently moving along the y-axis (up or down). This condition is used to prevent turning left or right while moving vertically.
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                input = Vector2Int.right;
            } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                input = Vector2Int.left;
            }
        }
    }

   private void FixedUpdate()
{
    // Wait until the next update before proceeding
    if (Time.time < nextUpdate) {
        return; // Skip this FixedUpdate if it's not time for the next update
    }

    // Set the new direction based on the input
    if (input != Vector2Int.zero) {
        direction = input; // Update the snake's direction if there's new input
    }

    // Set each segment's position to be the same as the one it follows.
    // Iterate in reverse to prevent stacking segments on top of each other.
    for (int i = segments.Count - 1; i > 0; i--) {
        segments[i].position = segments[i - 1].position;
    }

    // Move the snake in the direction it is facing.
    // Round the values to align the snake with the grid.
    int x = Mathf.RoundToInt(transform.position.x) + direction.x;
    int y = Mathf.RoundToInt(transform.position.y) + direction.y;
    transform.position = new Vector2(x, y);

    // Set the next update time based on the speed and speed multiplier.
    nextUpdate = Time.time + (1f / (speed * speedMultiplier));
}

    public void Grow()
    {
        Transform segment = Instantiate(segmentPrefab);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
    }

    public void ResetState()
{
    direction = Vector2Int.right;
    transform.position = Vector3.zero;

    // Start at 1 to skip destroying the head
    for (int i = 1; i < segments.Count; i++) {
        Destroy(segments[i].gameObject);
    }

    // Clear the list but add back this as the head
    segments.Clear();
    segments.Add(transform);

    // Reset the score to its initial value (0)
    score = 0;

    // -1 since the head is already in the list
    for (int i = 0; i < initialSize - 1; i++) {
        Grow();
    }

    UpdateScoreText(); // Update the score display after resetting the state
}

    public bool Occupies(int x, int y)
    {
        foreach (Transform segment in segments)
        {
            if (Mathf.RoundToInt(segment.position.x) == x &&
                Mathf.RoundToInt(segment.position.y) == y) {
                return true;
            }
        }

        return false;
    }

 private void OnTriggerEnter2D(Collider2D other)
{
    if (other.gameObject.CompareTag("Food"))
    {
        Grow();
        score += 1; // Increase the score by 1 point when food is eaten
        UpdateScoreText();
    }
    else if (other.gameObject.CompareTag("Obstacle"))
    {
        ResetState();
        UpdateScoreText(); // You might want to reset the score when the snake hits an obstacle
    }
}


private void Traverse(Transform wall)
{
    // Get the current position of the snake's head
    Vector3 position = transform.position;

    // Check if the snake is moving horizontally
    if (direction.x != 0f) {
        // Adjust the x-coordinate of the position based on the wall's position and direction
        // This effectively simulates the snake moving to the opposite side of the wall
        position.x = Mathf.RoundToInt(-wall.position.x + direction.x);
    }
    // Check if the snake is moving vertically
    else if (direction.y != 0f) {
        // Adjust the y-coordinate of the position based on the wall's position and direction
        // This effectively simulates the snake moving to the opposite side of the wall
        position.y = Mathf.RoundToInt(-wall.position.y + direction.y);
    }

    // Update the snake's position to the adjusted position
    transform.position = position;
}


}