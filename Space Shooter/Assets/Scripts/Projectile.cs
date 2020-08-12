using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float lifeTime = 0.5f;

    IEnumerator Start()
    {
        FindObjectOfType<GameManager>().IncreaseShotsFiredThisLevel();
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
