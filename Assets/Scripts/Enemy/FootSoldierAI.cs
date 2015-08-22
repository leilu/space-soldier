﻿using UnityEngine;
using System.Collections;

public class FootSoldierAI : MonoBehaviour {

    public int firingDistance;
    public int attackingDistance;
    public float speed;
    public float timeBetweenMoves;
    // The angle (in degrees) in each direction that the enemy can move.
    public int movementVariationDegrees;
    public int bounceVariationDegrees;
    public int movementVariationTime;
    public int shotsFiredPerMovement;

    private GameObject player;
    private Rigidbody2D rb2d;
    private float nextMoveTime = 0;
    private int shotsFiredThisMovement = 0;
    private BasicEnemyFire enemyFireScript;
    private int numMovementAttempts;
    private int wallLayerMask = 1 << 8;
    private Vector2 previousVelocity = Vector2.zero;
    private Vector2 colliderSize;

    void Awake()
    {
        player = GameObject.Find("Soldier");
        rb2d = GetComponent<Rigidbody2D>();
        enemyFireScript = GetComponent<BasicEnemyFire>();
        colliderSize = GetComponent<BoxCollider2D>().size;
    }

    void FixedUpdate()
    {
        if (WithinAttackRange() && Time.time >= nextMoveTime)
        {
            if (shotsFiredThisMovement < shotsFiredPerMovement)
            {
                rb2d.velocity = Vector2.zero;
                shotsFiredThisMovement += enemyFireScript.Fire();
            }
            else
            {
                shotsFiredThisMovement = 0;
                nextMoveTime = Time.time + timeBetweenMoves;
                int rotation = Random.Range(-movementVariationDegrees, movementVariationDegrees);

                Vector2 possibleVelocity = Vector2.zero;
                for (int addend = 0; addend < 360; addend += 45)
                {
                    possibleVelocity = VectorUtil.RotateVector(player.transform.position - gameObject.transform.position,
                        (rotation + addend) * Mathf.Deg2Rad).normalized * speed;
                    if (Physics2D.BoxCast(gameObject.transform.position, colliderSize, 0f, possibleVelocity, 
                        4.5f, wallLayerMask).transform == null)
                    {
                        break;
                    }
                }

                rb2d.velocity = possibleVelocity;
            }
        }
        else
        {
            shotsFiredThisMovement = 0;
        }

        previousVelocity = rb2d.velocity;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Wall")
        {
            rb2d.velocity = VectorUtil.RotateVector(new Vector2(-previousVelocity.x, -previousVelocity.y),
                Random.Range(-bounceVariationDegrees, bounceVariationDegrees) * Mathf.Deg2Rad).normalized * speed;
        }
    }

    bool WithinAttackRange()
    {
        return Vector3.Distance(player.transform.position, gameObject.transform.position) <= attackingDistance;
    }
}