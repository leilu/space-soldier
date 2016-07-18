﻿using UnityEngine;

[RequireComponent (typeof (EnemyHealth))]
[RequireComponent (typeof(SpriteRenderer))]
[RequireComponent (typeof(BoxCollider2D))]
[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(EnemyDeath))]
public class EnemyAI : MonoBehaviour {
    public float chaseActivationRadius;
    public float chaseCheckInterval;
    public float chaseCheckCooldown;
    public float speed;
    public bool chasing = false;
    public bool killed = false;
    public float KnockbackEndTime = 0;
    public bool KnockbackInProgress = false;
    public Animator animator;
    private float previousSpeed;

    protected void Init()
    {
        // TODO:  add more stuff to this init to reduce code duplication
        animator = GetComponent<Animator>();
    }

    private float nextChaseCheckTime = 0;

    protected void ChaseIfNecessary()
    {
        if (chasing == false && Time.time > nextChaseCheckTime)
        {
            nextChaseCheckTime = Time.time + chaseCheckInterval;
            Collider2D[] otherEnemies = Physics2D.OverlapCircleAll(transform.position, chaseActivationRadius, LayerMasks.EnemyLayerMask);
            for (int x = 0; x < otherEnemies.Length; x++)
            {
                EnemyAI enemyAI = otherEnemies[x].GetComponent<EnemyAI>();
                if (enemyAI && enemyAI.chasing)
                {
                    chasing = true;
                    break;
                }
            }
        }
    }

    protected void DeactivateChase()
    {
        chasing = false;
        // To prevent enemies from perpetually chasing due to immediately reactivating chase after idling.
        nextChaseCheckTime = Time.time + chaseCheckCooldown;
    }

    public void SlowDown()
    {
        previousSpeed = speed;
        speed *= GameState.TimeDilationScale;
        animator.speed = GameState.TimeDilationScale;
    }

    public void RestoreSpeed()
    {
        speed = previousSpeed;
        animator.speed = 1;
    }
}
