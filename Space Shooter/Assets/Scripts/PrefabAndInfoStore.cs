using UnityEngine;

public class PrefabAndInfoStore : MonoBehaviour
{
    public static PrefabAndInfoStore store;
    
    private void Awake()
    {
        // Singleton pattern
        if (store != null)
        {
            Destroy(gameObject);
        }
        else
        {
            store = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public GameObject[] Asteroids; // large, medium, small
    public float[] AsteroidsSpeeds; // large, medium, small
    public int[] AsteroidsScores; // large, medium, small
}
    