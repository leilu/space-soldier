using UnityEngine;

public class EnergyOrbProperties : BasicPlayerProjectile
{
    public int ExplosionDamage { get; set; }
    public float ExplosionRadius { get; set; }

    private Animator animator;
    private int enemyLayerMask = 1 << 9;

    void Awake()
    {
        animator = GetComponent<Animator>();
        projectileDestroy = GetComponent<ProjectileDestroy>();
    }

    void DestroyEnergyOrb()
    {
        animator.SetBool("exploding", false);
        projectileDestroy.Destroy();
    }

    void DoExplosion()
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(gameObject.transform.position, ExplosionRadius, enemyLayerMask);
        foreach(Collider2D collider in nearbyEnemies) {
            collider.GetComponent<EnemyHealth>().InflictDamage(ExplosionDamage, rb2d.velocity);
        }
    }
}
