using UnityEngine;
using System.Collections.Generic;

public class EnemyHealth : MonoBehaviour {
    public bool guarded = false;

    private EnemyAI enemyAI;
    private EnemyDeath enemyDeath;
    private Animator animator;
    private Material customSpriteMaterial;
    private float damageTakenMultiplier;

    private List<StickyBomb> attachedStickyBombs;

    void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
        enemyDeath = GetComponent<EnemyDeath>();
        animator = GetComponent<Animator>();
        customSpriteMaterial = GetComponent<SpriteRenderer>().material;

        attachedStickyBombs = new List<StickyBomb>();
    }

    public float health = 20;

    public void InflictDamage(int damagePoints)
    {
        InflictDamage(damagePoints, Vector2.zero);
    }

    public void InflictDamage(int damagePoints, Vector2 velocity)
    {
        if (guarded)
        {
            return;
        }

        DetonateStickyBombs();

        health -= damagePoints * damageTakenMultiplier;

        if (enemyAI)
        {
            enemyAI.chasing = true;
        }

        if (health <= 0)
        {
            enemyDeath.KillEnemy();
        } else if (animator != null)
        {
            animator.SetBool("Hit", true);
            animator.SetFloat("HitFromDirX", velocity.x > 0 ? 0 : 1);
            animator.SetFloat("HitFromDirY", velocity.y > 0 ? 1 : 0);
        }
    }

    public void InflictPoison(float poisonDamageMultiplier, float duration)
    {
        damageTakenMultiplier = poisonDamageMultiplier;
        customSpriteMaterial.SetFloat("_PoisonFlag", 1);
        Invoke("EndPoison", duration);
    }

    void EndPoison()
    {
        damageTakenMultiplier = 1;
        customSpriteMaterial.SetFloat("_PoisonFlag", 0);
    }

    void DetonateStickyBombs()
    {
        for (int i = 0; i < attachedStickyBombs.Count; i++)
        {
            attachedStickyBombs[i].Detonate();
        }
    }

    public void HitDone()
    {
        animator.SetBool("Hit", false);
    }

    public void AddStickyBomb(StickyBomb stickyBomb)
    {
        attachedStickyBombs.Add(stickyBomb);
    }

    public void RemoveStickyBomb(StickyBomb stickyBomb)
    {
        attachedStickyBombs.Remove(stickyBomb);
    }
}
