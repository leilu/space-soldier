using UnityEngine;
using System.Collections;

public class Grenade : BasicPlayerProjectile {

    [SerializeField]
    private float decelerationRate;
    // Don't make collisionDistance too small or else the collision point may be inside of the collider, 
    // causing the normal to be the exact opposite of the velocity which is no good.
    [SerializeField]
    private float collisionDistance;
    [SerializeField]
    private float wallBounceMultiplier;
    [SerializeField]
    private float groundScale;
    [SerializeField]
    private float backupDistance;
    [SerializeField]
    private int explosiveDamage;
    [SerializeField]
    private float explosionRadius;
    [SerializeField]
    private float detonationTime;

    private RaycastHit2D hit;
    private float height;
    private float verticalVelocity;

	protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (isEnemy(other))
        {
            EnemyHealth healthComponent = other.GetComponent<EnemyHealth>();
            if (!healthComponent.guarded)
            {
                healthComponent.InflictDamage(Damage, rb2d.velocity);
                Knockback knockback = other.GetComponent<Knockback>();
                if (knockback)
                {
                    knockback.KnockBack(rb2d.velocity);
                }

                Detonate();
            } else
            {
                Vector2 pos = new Vector2(transform.position.x, transform.position.y) - rb2d.velocity.normalized * backupDistance;
                hit = Physics2D.Raycast(pos, rb2d.velocity);
                if (hit.normal != null)
                {
                    rb2d.velocity = Vector2.Reflect(rb2d.velocity, hit.normal) * wallBounceMultiplier;
                }
            }
        }
	}

    void FixedUpdate()
    {
        Decelerate();
        HandleCollisions();

        // TODO: Add height-based scale adjustment for floor bounce
    }

    void Decelerate()
    {
        float originalSpeed = rb2d.velocity.magnitude;
        float newSpeed = Mathf.Max(0, originalSpeed - decelerationRate * Time.deltaTime);
        rb2d.velocity = rb2d.velocity.normalized * newSpeed;

        if (newSpeed == 0 && originalSpeed != 0)
        {
            Invoke("Detonate", detonationTime);
        }
    }

    void HandleCollisions()
    {
        float hitDistance = 0;
        Vector2 pos = transform.position;
        hit = Physics2D.Raycast(pos, rb2d.velocity, collisionDistance, LayerMasks.WallAndObstacleLayerMask);

        if (hit.transform == null)
        {
            return;
        }


        int numRaycastAttempts = 0;
        while (hitDistance == 0 && numRaycastAttempts < 3)
        {
            // Can't do this in OnTriggerEnter2D, since sometimes Unity just completely misses the event.
            hit = Physics2D.Raycast(pos, rb2d.velocity, collisionDistance, LayerMasks.WallAndObstacleLayerMask);
            hitDistance = hit.distance;
            if (hitDistance == 0)
            {
                pos -= rb2d.velocity.normalized * backupDistance;
            }

            numRaycastAttempts++;
        }

        rb2d.velocity = Vector2.Reflect(rb2d.velocity, hit.normal) * wallBounceMultiplier;
    }

    void Detonate()
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(gameObject.transform.position, explosionRadius, LayerMasks.EnemyLayerMask);
        foreach (Collider2D collider in nearbyEnemies)
        {
            collider.GetComponent<EnemyHealth>().InflictDamage(explosiveDamage, rb2d.velocity);
        }

        projectileDestroy.Destroy();
    }
}
