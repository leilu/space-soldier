using UnityEngine;

public class Mine : MonoBehaviour {
    public int ImmediateDamage { get; set; }
    public int ExplosionDamage { get; set; }
    public float ExplosionRadius { get; set; }

    private ProjectileDestroy projectileDestroy;

    void Awake()
    {
        projectileDestroy = GetComponent<ProjectileDestroy>();
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.tag == "Enemy" || other.tag == "Boss")
        {
            other.GetComponent<EnemyHealth>().InflictDamage(ImmediateDamage);
            Detonate();
            Knockback knockback = other.GetComponent<Knockback>();
            if (knockback)
            {
                knockback.KnockBack(other.transform.position - transform.position);
            }
        }
    }

    void Detonate()
    {
        // Put animation here.
        projectileDestroy.Destroy();
    }
}
