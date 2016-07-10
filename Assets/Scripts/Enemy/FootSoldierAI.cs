﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Wander))]
public class FootSoldierAI : EnemyAI {

    public int firingDistance;
    public int attackingDistance;
    public float timeBetweenMoves;
    public int movementVariationTime;
    public int shotsFiredPerMovement;
    public float attackDuration;
    public float bounceDisplacement;

    private Rigidbody2D rb2d;
    private float nextMoveTime = 0;
    private int shotsFiredThisMovement = 0;
    private Wander wanderScript;
    private int numMovementAttempts;
    private Vector2 colliderSize;
    private Animator animator;

    private EnemyWeapon weapon;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        wanderScript = GetComponent<Wander>();
        colliderSize = GetComponent<BoxCollider2D>().size;
        weapon = GetComponent<EnemyWeapon>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (KnockbackInProgress || GameSettings.PauseAllEnemies || killed)
        {
            return;
        }

        ChaseIfNecessary();

        if (EnemyUtil.CanSee(transform.position, Player.PlayerTransform.position))
        {
            CancelInvoke("DeactivateAttack");
            chasing = true;
        }
        else if(chasing)
        {
            Invoke("DeactivateAttack", attackDuration);
        }
        else if (!GameState.TutorialMode)
        {
            wanderScript.DoWander();
        }

        if (Time.time >= nextMoveTime && chasing)
        {
            if (shotsFiredThisMovement < shotsFiredPerMovement)
            {
                animator.SetBool("Firing", true);
                rb2d.velocity = Vector2.zero;
                shotsFiredThisMovement += weapon.Fire();

                float xDirProportion, yDirProportion;
                EnemyUtil.GetXYRatios(Player.PlayerTransform.position - transform.position, out xDirProportion, out yDirProportion);
                animator.SetFloat("ToPlayerX", xDirProportion);
                animator.SetFloat("ToPlayerY", yDirProportion);
            }
            else
            {
                animator.SetBool("Firing", false);
                shotsFiredThisMovement = 0;
                nextMoveTime = Time.time + timeBetweenMoves;

                rb2d.velocity =
                    EnemyUtil.CalculateUnblockedDirection(transform.position, colliderSize, 2f, false) * speed;
            }
        }
        else
        {
            shotsFiredThisMovement = 0;
        }

        float xVelocityProportion, yVelocityProportion;
        EnemyUtil.GetXYRatios(rb2d.velocity, out xVelocityProportion, out yVelocityProportion);
        animator.SetFloat("MoveX", xVelocityProportion);
        animator.SetFloat("MoveY", yVelocityProportion);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Wall")
        {
            transform.Translate(-rb2d.velocity.normalized * bounceDisplacement);
            rb2d.velocity = Vector2.zero;
            nextMoveTime = Time.time + .1f;
        }
    }

    bool WithinAttackRange()
    {
        return Vector3.Distance(Player.PlayerTransform.position, gameObject.transform.position) <= attackingDistance;
    }

    void DeactivateAttack()
    {
        animator.SetBool("Firing", false);
        rb2d.velocity = Vector2.zero;
        DeactivateChase();
    }
}
