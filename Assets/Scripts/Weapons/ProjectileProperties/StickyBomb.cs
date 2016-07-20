using UnityEngine;
using System.Collections;

public class StickyBomb : MonoBehaviour {

    [SerializeField]
    private float timeBeforeDetonating;

    private Rigidbody2D rb2d;
    private ProjectileDestroy projectileDestroy;
    private GameObject stuckEnemy;
    private bool stuckToEnemy;
    private bool stuckToWall;
    private Animator animator;

    public int ExplosionDamage { get; set; }
    public float ExplosionRadius { get; set; }

    void Awake ()
    {
        rb2d = GetComponent<Rigidbody2D>();
        projectileDestroy = GetComponent<ProjectileDestroy>();
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        stuckToEnemy = false;
        stuckToWall = false;
    }

	void OnTriggerEnter2D (Collider2D other) {
        if (stuckToWall || stuckToEnemy) {
            return;
        }

        if (other.tag == "Enemy" || other.tag == "Boss")
        {
            stuckEnemy = other.gameObject;
            stuckToEnemy = true;
            rb2d.velocity = Vector2.zero;
            stuckEnemy.GetComponent<EnemyHealth>().AddStickyBomb(this);
        } else if (other.tag == "Obstacle" || other.gameObject.layer == GameSettings.WallLayerNumber)
        {
            stuckToWall = true;
            rb2d.velocity = Vector2.zero;
            Invoke("Detonate", timeBeforeDetonating);
        }
    }

    public void DestroyOrb()
    {
        animator.SetBool("exploding", false);
        projectileDestroy.Destroy();

        if (stuckEnemy)
        {
            stuckEnemy.GetComponent<EnemyHealth>().RemoveStickyBomb(this);
        }
    }

    public void Detonate ()
    {
        if (!animator.GetBool("exploding"))
        {
            animator.SetBool("exploding", true);
            Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(gameObject.transform.position, ExplosionRadius, LayerMasks.EnemyLayerMask);
            foreach (Collider2D collider in nearbyEnemies)
            {
                collider.GetComponent<EnemyHealth>().InflictDamage(ExplosionDamage, rb2d.velocity);
            }
        }
    }

    void Update()
    {
        if (stuckToEnemy)
        {
            transform.position = stuckEnemy.transform.position;
        }
    }
}