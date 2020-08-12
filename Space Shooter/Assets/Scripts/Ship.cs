using System.Collections;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject projectile;
    [SerializeField] Transform projectilePos;
    Rigidbody2D rb;
    GameManager gameManager;

    [Header("Balancing")]
    [SerializeField] float rotationSpeed = 360f;
    [SerializeField] float shipAcceleration = 15f;
    [SerializeField] float maxShipSpeed = 10f;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float fireRate = 0.1f;
    
    // status vars
    float rotationAmount;
    float accelerationAmount;
    bool canShoot = true;
    bool isDead = false;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();
    }
    
    private void Update()
    {
        HandleMovementInput();
        RotateShip();
        HandleFire();
    }
    
    private void FixedUpdate()
    {
        MoveShip();
    }
    
    private void HandleMovementInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rotationAmount = rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rotationAmount = -rotationSpeed * Time.deltaTime;
        }
        else
        {
            rotationAmount = 0;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            accelerationAmount = shipAcceleration;
        }
        else
        {
            accelerationAmount = 0;
        }
    }

    private void RotateShip()
    {
        transform.Rotate(0, 0, rotationAmount);
    }

    private void HandleFire()
    {
        if (Input.GetKey("space") && canShoot)
        {
            StartCoroutine(Fire());
        }
    }

    private IEnumerator Fire()
    {
        AudioManager.instance.Play("Shoot");
        canShoot = false;
        GameObject spawnedProjectile = Instantiate(projectile, projectilePos.position, Quaternion.identity);
        spawnedProjectile.GetComponent<Rigidbody2D>().velocity = (Vector2)(transform.up * projectileSpeed) + rb.velocity;
        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }

    private void MoveShip()
    {
        rb.AddForce(transform.up * accelerationAmount);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxShipSpeed);
    }
    
    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (isDead) { return; } // safety measures
        if (otherCollider.gameObject.CompareTag("Asteroid"))
        {
            isDead = true;
            gameManager.LoseLife();
            Destroy(gameObject);
        }
    }
}
