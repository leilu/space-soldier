﻿using UnityEngine;

public class KirbySeekingState : State<KirbyAI> {
    private static KirbySeekingState instance;
    public static KirbySeekingState Instance
    {
        get
        {
            instance = instance == null ? new KirbySeekingState() : instance;
            return instance;
        }
    }

    public override void Execute(KirbyAI kirby)
    {
        kirby.lineRenderer.enabled = false;
        kirby.actualSpeed = kirby.speed;

        EnemyAI closestSeekableEnemy = kirby.GetClosestSeekableEnemy();
        if (closestSeekableEnemy != null && kirby.CanGuardEnemy(closestSeekableEnemy))
        {
            kirby.rb2d.velocity = Vector2.zero;
            kirby.fsm.ChangeState(KirbyDefendingState.Instance);
            return;
        } else if (closestSeekableEnemy != null)
        {
            kirby.Approach(closestSeekableEnemy.transform.position);
        } else
        {
            kirby.rb2d.velocity = Vector2.zero;
        }
    }
}
