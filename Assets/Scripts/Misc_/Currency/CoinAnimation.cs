using UnityEngine;

public class CoinAnimation : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public float bounceHeight = 0.25f;
    public float bounceSpeed = 2f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Spin around the Y-axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Bounce using sine wave
        float newY = startPosition.y + Mathf.Abs(Mathf.Sin(Time.time * bounceSpeed)) * bounceHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
