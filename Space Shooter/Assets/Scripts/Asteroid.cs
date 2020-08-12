using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [Header("Balancing")]
    [SerializeField] Size size;
    [SerializeField] float maxRotation = 140f;
    
    // status vars
    float rotationAmount;
    bool hasExploded = false;

    // references
    GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    
    private void Start()
    {
        // set random velocity to the asteroid
        GetComponent<Rigidbody2D>().velocity = GetRandomDirection() * PrefabAndInfoStore.store.AsteroidsSpeeds[(int)size];

        // randomize the rotation amount
        rotationAmount = Random.Range(-1f,1f) * maxRotation;
    }

    private void Update()
    {
        // rotate asteroid
        transform.Rotate(0, 0, rotationAmount * Time.deltaTime);
    }

    private Vector2 GetRandomDirection()
    {
        Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        randomDirection.Normalize();
        return randomDirection;
    }
    
    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if(hasExploded) { return; } // safety measures

        // handle collision with projectile 
        if(otherCollider.gameObject.CompareTag("Projectile"))
        {
            AudioManager.instance.Play("Break");
            hasExploded = true;
            Destroy(otherCollider.gameObject); // destroy projectile

            if (size == Size.large || size == Size.medium)
            {
                SpawnTwoSmallerAsteroids();
                AddScoreAndIncreaseHits();
                gameManager.AddAsteroid();

            }

            if (size == Size.small)
            {
                AddScoreAndIncreaseHits();
                gameManager.SubtractAsteroid();
            }

            Destroy(gameObject);
        }
    }
    
    private void SpawnTwoSmallerAsteroids()
    {
        GameObject smallerAsteroid1 = Instantiate(PrefabAndInfoStore.store.Asteroids[(int)size + 1], transform.position, Quaternion.identity);
        GameObject smallerAsteroid2 = Instantiate(PrefabAndInfoStore.store.Asteroids[(int)size + 1], transform.position, Quaternion.identity);

        smallerAsteroid1.GetComponent<Rigidbody2D>().velocity = GetRandomDirection() * PrefabAndInfoStore.store.AsteroidsSpeeds[(int)size + 1];
        smallerAsteroid2.GetComponent<Rigidbody2D>().velocity = GetRandomDirection() * PrefabAndInfoStore.store.AsteroidsSpeeds[(int)size + 1];
    }

    private void AddScoreAndIncreaseHits()
    {
        gameManager.AddScore(PrefabAndInfoStore.store.AsteroidsScores[(int)size]);
        gameManager.IncreaseShotsHitThisLevel();
    }
}

enum Size
{
    large,
    medium,
    small
}
