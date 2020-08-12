using UnityEngine;

public class Background : MonoBehaviour
{
    private void Awake()
    {
        float camHeight = 2f * Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;
        
        GetComponent<SpriteRenderer>().size = new Vector2(camWidth, camHeight);
    }
}
