﻿using UnityEngine;

public class EnemyDeath : MonoBehaviour {
    public int experiencePoints;

    private bool destroyed = false;
    private Animator animator;
    private EnemyAI enemyAI;

    void Awake()
    {
        animator = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();
    }

    public void KillEnemy()
    {
        // Destroy doesn't actually take effect until after the update loop, so I added this condition to prevent explosive damage (and other forms
        // of secondary damage) from doubling the experience points earned.
        if (!destroyed)
        {
            animator.SetBool("Killed", true);
            enemyAI.killed = true;
            gameObject.tag = "Killed";
            gameObject.layer = LayerMask.NameToLayer("Killed");

            Player.PlayerExperience.IncrementExperience(experiencePoints);
            GameState.NumEnemiesKilled++;
            destroyed = true;
            GameState.NumEnemiesRemaining--;
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Killed";

            GameState.LockOnTargets.Remove(gameObject.GetComponent<LockOnIndicator>());

            if (GameState.TutorialMode)
            {
                TutorialEngine.Instance.Trigger(TutorialTrigger.EnemyKilled);
            }

            if (GameState.NumEnemiesRemaining == 0 && !GameState.IsBossFight)
            {
                createPortal();
            }
        }
    }

    void createPortal()
    {
        GameObject portal = GameObject.Find("Portal");
        portal.transform.position = transform.position;
        portal.GetComponent<SpriteRenderer>().enabled = true;
        portal.GetComponent<BoxCollider2D>().enabled = true;
    }
}
