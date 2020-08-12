using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject ship;
    [SerializeField] GameObject asteroid;
    [SerializeField] GameObject lifePrefab;
    [SerializeField] GameObject livesContainterGO;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI accuracyText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI scoreText;

    [Header("Balancing")]
    [SerializeField] int livesAtStart = 3;
    [SerializeField] int numOfAsteroidsAtStart = 2;
    [SerializeField] float defaultSafeRadius = 2f;
    [SerializeField] float maxAccuracyScore = 1000f;
    [SerializeField] float maxTimeScore = 1000f;
    [SerializeField] float minTimeToDestroyAsteroid = 8f; // in seconds
    [SerializeField] float maxTimeToDestroyAsteroid = 30f; // in seconds

    // status vars
    int currentLevel = 1;
    int score = 0;
    int numOfAsteroidsToSpawn;
    int numOfAsteroidsLeft;
    int lives;
    int shotsFiredThisLevel = 0;
    int shotsHitThisLevel = 0;
    float timeOfLevelStart;
    
    // vars for initial calculations
    Camera cam;
    float camHeight;
    float camWidth;
    
    float minAstStartingXPos;
    float maxAstStartingXPos;
    float minAstStartingYPos;
    float maxAstStartingYPos;

    Vector3 shipStartingPos;
    
    void Start()
    {
        AudioManager.instance.Play("Theme");
        ResetStartingParameters();
        InitialCalculations();
        
        StartCoroutine(StartLevel());
    }
    
    private void ResetStartingParameters()
    {
        currentLevel = 1;
        score = 0;
        numOfAsteroidsToSpawn = numOfAsteroidsAtStart;
        lives = livesAtStart;
        shotsFiredThisLevel = 0;
        shotsHitThisLevel = 0;
        timeOfLevelStart = Time.time;
    }

    private void InitialCalculations()
    {
        // camera's width and height
        cam = Camera.main;
        camHeight = 2f * cam.orthographicSize;
        camWidth = camHeight * cam.aspect;

        // asteroids' related calculations
        minAstStartingXPos = -(camWidth / 4f);
        maxAstStartingXPos = (camWidth / 4f);
        minAstStartingYPos = (camHeight / 8f);
        maxAstStartingYPos = (camHeight / 2f) - (camHeight / 8f);

        // ship's related calculations
        shipStartingPos = new Vector3(0, -(camHeight / 2f) + (camHeight / 4f), 0); ;
    }

    private IEnumerator StartLevel()
    {
        if (currentLevel == 1)
        {
            // called in level 1
            UpdateLives();
            scoreText.gameObject.SetActive(true);
            scoreText.text = score.ToString();
        }
        else
        {
            // called when advancing a level (levels 2,3,4...)
            AudioManager.instance.Play("Win");
            levelText.gameObject.SetActive(true);
            levelText.text = "Finished level " + (currentLevel - 1).ToString();

            int accuracyScore = GetAccuracyScore();
            accuracyText.gameObject.SetActive(true);
            accuracyText.text = "Accuracy +" + accuracyScore.ToString();
            
            int timeScore = GetTimeScore();
            timeText.gameObject.SetActive(true);
            timeText.text = "Time +" + timeScore.ToString();

            score += (accuracyScore + timeScore);
            scoreText.text = score.ToString();

            yield return new WaitForSeconds(3f);

            levelText.gameObject.SetActive(false);
            accuracyText.gameObject.SetActive(false);
            timeText.gameObject.SetActive(false);
            Destroy(FindObjectOfType<Ship>().gameObject);
            yield return new WaitForSeconds(2f);
        }

        levelText.gameObject.SetActive(true);
        levelText.text = "Level " + currentLevel.ToString();
        yield return new WaitForSeconds(2f);
        levelText.gameObject.SetActive(false);

        shotsFiredThisLevel = 0;
        shotsHitThisLevel = 0;
        timeOfLevelStart = Time.time;

        SpawnShip();
        SpawnAsteroids();
    }

    private void UpdateLives()
    {
        foreach (Transform child in livesContainterGO.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < lives; i++)
        {
            GameObject spawnedLife = Instantiate(lifePrefab, transform.position, Quaternion.identity); // spawn life image
            spawnedLife.transform.SetParent(livesContainterGO.transform); // set containter to be the parent
            spawnedLife.transform.localScale = Vector3.one; // make sure that the scale is (1,1,1)
            spawnedLife.GetComponent<RectTransform>().localPosition = new Vector3(spawnedLife.GetComponent<RectTransform>().localPosition.x, spawnedLife.GetComponent<RectTransform>().localPosition.y, 0f); // make sure that the ZPos is 0
            float size = Screen.height * (livesContainterGO.GetComponent<RectTransform>().anchorMax.y - livesContainterGO.GetComponent<RectTransform>().anchorMin.y); // calculate height of containter
            spawnedLife.GetComponent<RectTransform>().sizeDelta = new Vector2(size, size); // resize life image to fit the containter as a square
        }
    }

    private int GetAccuracyScore()
    {
        return Mathf.RoundToInt(1.0f * shotsHitThisLevel / shotsFiredThisLevel * maxAccuracyScore);
    }

    private int GetTimeScore()
    {
        float minScoreTimeInSeconds = minTimeToDestroyAsteroid * (numOfAsteroidsToSpawn - 1);
        float maxScoreTimeInSeconds = maxTimeToDestroyAsteroid * (numOfAsteroidsToSpawn - 1);
        
        Debug.Log((numOfAsteroidsToSpawn - 1).ToString());
        Debug.Log(minScoreTimeInSeconds.ToString());
        Debug.Log(maxScoreTimeInSeconds.ToString());
        
        // time passed from start of level
        float timePassed = Time.time - timeOfLevelStart;

        // time passed during the test interval
        float timePassedDuringInterval = timePassed - minScoreTimeInSeconds;

        // time passed during the test interval clamped
        timePassedDuringInterval = Mathf.Clamp(timePassedDuringInterval, 0, maxScoreTimeInSeconds - minScoreTimeInSeconds);

        float timeInterval = maxScoreTimeInSeconds - minScoreTimeInSeconds;

        float timeRemained = timeInterval - timePassedDuringInterval;

        return Mathf.RoundToInt(timeRemained / timeInterval * maxTimeScore);
    }
    
    private void SpawnShip()
    {
        Instantiate(ship, shipStartingPos, Quaternion.identity);
    }

    private void SpawnAsteroids()
    {
        for (int i = 0; i < numOfAsteroidsToSpawn; i++)
        {
            Vector3 asteroidPos = new Vector3(Random.Range(minAstStartingXPos, maxAstStartingXPos), Random.Range(minAstStartingYPos, maxAstStartingYPos), 0);
            Instantiate(asteroid, asteroidPos, Quaternion.identity);
        }
        numOfAsteroidsLeft = numOfAsteroidsToSpawn;
    }

    private IEnumerator GameOver()
    {
        levelText.gameObject.SetActive(true);
        levelText.text = "Game Over";
        yield return new WaitForSeconds(3f);
        levelText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        DestroyAllAsteroids();
        yield return new WaitForSeconds(2f);
        ResetStartingParameters();

        StartCoroutine(StartLevel());
    }

    private void DestroyAllAsteroids()
    {
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");

        for (int i = 0; i < asteroids.Length; i++)
        {
            Destroy(asteroids[i]);
        }
    }

    private IEnumerator TryToSpawnShip()
    {
        float startTime = Time.time;
        float safeRadius = defaultSafeRadius;
        while (Physics2D.OverlapCircle(shipStartingPos, safeRadius))
        {
            // trying to spawn with bigger saferadius for 3 seconds before changing to small saferadius
            if (Time.time - startTime > 3f)
            {
                safeRadius = 0.5f;
            }
            yield return null;
        }
        SpawnShip();
    }
    
    // Public functions

    public void AddAsteroid()
    {
        numOfAsteroidsLeft++;
    }

    public void SubtractAsteroid()
    {
        numOfAsteroidsLeft--;

        // level won
        if(numOfAsteroidsLeft == 0)
        {
            currentLevel++;
            numOfAsteroidsToSpawn++;
            StartCoroutine(StartLevel());
        }
    }

    public void LoseLife()
    {
        AudioManager.instance.Play("Die");
        lives--;
        UpdateLives();

        if (lives == 0)
        {
            StartCoroutine(GameOver());
        }
        else
        {
            StartCoroutine(TryToSpawnShip());
        }
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = score.ToString();
    }

    public void IncreaseShotsFiredThisLevel()
    {
        shotsFiredThisLevel++;
    }

    public void IncreaseShotsHitThisLevel()
    {
        shotsHitThisLevel++;
    }
}
