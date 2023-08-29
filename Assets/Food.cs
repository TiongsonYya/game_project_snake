using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Food : MonoBehaviour
{
    public Collider2D gridArea;

    private Snake snake;

    private void Awake()
    {
        // Find and store a reference to the Snake component in the scene
        snake = FindObjectOfType<Snake>();
    }

    private void Start()
    {
        // Set a random position for the food when it starts
        RandomizePosition();
    }

    public void RandomizePosition()
    {
        // Get the bounds of the grid area
        Bounds bounds = gridArea.bounds;

        // Pick a random position inside the grid area bounds
        // Round the values to ensure alignment with the grid
        int x = Mathf.RoundToInt(Random.Range(bounds.min.x, bounds.max.x));
        int y = Mathf.RoundToInt(Random.Range(bounds.min.y, bounds.max.y));

        // Prevent the food from spawning on the snake's body
        while (snake.Occupies(x, y))
        {
            x++;

            // If x exceeds the grid bounds, wrap around to the minimum x
            if (x > bounds.max.x)
            {
                x = Mathf.RoundToInt(bounds.min.x);
                y++;

                // If y exceeds the grid bounds, wrap around to the minimum y
                if (y > bounds.max.y) {
                    y = Mathf.RoundToInt(bounds.min.y);
                }
            }
        }

        // Set the food's position to the calculated position
        transform.position = new Vector2(x, y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // When the food is consumed by the snake, randomize its position
        RandomizePosition();
    }
}
