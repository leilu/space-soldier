using UnityEngine;
using System.Collections;

public class IceDart : BasicPlayerProjectile {

    public float FreezeDuration
    {
        get; set;
    }

    protected override void OnTriggerEnter2D (Collider2D other)
    {
        if (isEnemy(other))
        {
            other.GetComponent<EnemyAI>().Freeze(FreezeDuration);
        }
        if (isObstacle(other))
        {
            projectileDestroy.Destroy();
        }
    }
}
