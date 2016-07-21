using UnityEngine;
using System.Collections;

public class PoisonDart : BasicPlayerProjectile {

    public float DamageMultiplier
    {
        get; set;
    }

    public float Duration
    {
        get; set;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (isEnemy(other))
        {
            other.GetComponent<EnemyHealth>().InflictPoison(DamageMultiplier, Duration);
        }
        if (isObstacle(other))
        {
            projectileDestroy.Destroy();
        }
    }
}
