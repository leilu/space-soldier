using UnityEngine;
using System.Collections;

public class Seed : MonoBehaviour {

    private Rigidbody2D rb2d;
    private GameObject plantEnemyPrefab;
    GameObject spawnedPlant;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        plantEnemyPrefab = Resources.Load("Knight") as GameObject;
    }

	public void Launch(Vector2 initialPosition, Vector2 direction, float initialSeedSpeed)
    {
        transform.position = initialPosition;
        gameObject.SetActive(true);
        rb2d.velocity = direction.normalized * initialSeedSpeed;
    }

    void Grow()
    {
        spawnedPlant = Instantiate(plantEnemyPrefab, transform.position, Quaternion.identity) as GameObject;
        GameState.LockOnTargets.Add(spawnedPlant.GetComponent<LockOnIndicator>());
        GameState.Enemies.Add(spawnedPlant.GetComponent<EnemyAI>());
        gameObject.SetActive(false);
    }

    void Update()
    {
        float speed = Mathf.Lerp(rb2d.velocity.magnitude, 0, Time.deltaTime * 3);

        if (speed <= 1)
        {
            Grow();
        } else
        {
            rb2d.velocity = rb2d.velocity.normalized * speed;
        }
    }
}
