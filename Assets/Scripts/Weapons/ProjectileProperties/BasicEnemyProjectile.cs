using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
[RequireComponent (typeof(BoxCollider2D))]
[RequireComponent (typeof(ProjectileDestroy))]
[RequireComponent (typeof(Rigidbody2D))]
public class BasicEnemyProjectile : MonoBehaviour {
    public int damage;
    public bool deflected = false;
    private Rigidbody2D rb2d;
    private float previousSpeed;
    public bool slow = false;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (GameState.TimeDilationScale != 1 && !slow)
        {
            slow = true;
            previousSpeed = rb2d.velocity.magnitude;
            rb2d.velocity = rb2d.velocity.normalized * previousSpeed * GameState.TimeDilationScale;
        } else if (GameState.TimeDilationScale == 1 && slow)
        {
            slow = false;
            rb2d.velocity = rb2d.velocity.normalized * previousSpeed;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !deflected)
        {
            other.GetComponent<PlayerHealth>().InflictDamage(damage);
        }

        if (deflected && isEnemy(other))
        {
            other.GetComponent<EnemyHealth>().InflictDamage(damage, rb2d.velocity);
            Knockback knockback = other.GetComponent<Knockback>();
            if (knockback)
            {
                knockback.KnockBack(rb2d.velocity);
            }
        }

        if (other.tag == "Player" || other.gameObject.layer == GameSettings.WallLayerNumber || other.tag == "Obstacle"
            || (deflected && isEnemy(other)))
        {
            GetComponent<ProjectileDestroy>().Destroy();
            slow = false;
            deflected = false;
        }
    }

    private bool isEnemy (Collider2D other)
    {
        return other.tag == "Enemy" || other.tag == "Boss";
    }
}
