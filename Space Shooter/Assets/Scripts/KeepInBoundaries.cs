using UnityEngine;

public class KeepInBoundaries : MonoBehaviour
{
    Camera cam;

    float leftBoundary;
    float rightBoundary;
    float topBoundary;
    float bottomBoundary;

    private void Awake()
    {
        CalculateBoundaries();
    }

    void Update()
    {
        KeepInBoundariesFunction();
    }

    private void CalculateBoundaries()
    {
        cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        leftBoundary = camWidth / -2f;
        rightBoundary = camWidth / 2f;
        topBoundary = camHeight / 2f;
        bottomBoundary = camHeight / -2f;
    }

    private void KeepInBoundariesFunction()
    {
        if (transform.position.x < leftBoundary)
        {
            transform.position = new Vector2(rightBoundary, transform.position.y);
        }

        if (transform.position.x > rightBoundary)
        {
            transform.position = new Vector2(leftBoundary, transform.position.y);
        }

        if (transform.position.y > topBoundary)
        {
            transform.position = new Vector2(transform.position.x, bottomBoundary);
        }

        if (transform.position.y < bottomBoundary)
        {
            transform.position = new Vector2(transform.position.x, topBoundary);
        }
    }
}
