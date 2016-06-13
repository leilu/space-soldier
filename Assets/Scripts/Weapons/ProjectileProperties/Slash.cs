using UnityEngine;
using System.Collections.Generic;

public class Slash : MonoBehaviour {
    [SerializeField]
    private float offset;
    private int damage;

    private SpriteRenderer spriteRenderer;
    private List<Collider2D> hitObjects;
    private PolygonCollider2D polygonCollider;


	void Awake () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hitObjects = new List<Collider2D>();
        polygonCollider = GetComponent<PolygonCollider2D>();
	}

    public void DoSlash(int damage)
    {
        this.damage = damage;

        Vector2 dir = VectorUtil.DirectionToMousePointer(Player.PlayerTransform);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, VectorUtil.AngleToMousePointer(Player.PlayerTransform)));
        transform.position = new Vector2(Player.PlayerTransform.position.x + dir.x * offset, Player.PlayerTransform.position.y + dir.y * offset);

        spriteRenderer.enabled = true;
        polygonCollider.enabled = true;
    }

    public void Hide()
    {
        spriteRenderer.enabled = false;
        polygonCollider.enabled = false;
        hitObjects.Clear();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hitObjects.Contains(other))
        {
            if (isEnemy(other))
            {
                hitObjects.Add(other);
                Vector2 hitDir = (other.transform.position - transform.position).normalized;
                other.GetComponent<EnemyHealth>().InflictDamage(damage, hitDir);
                Knockback knockback = other.GetComponent<Knockback>();
                if (knockback)
                {
                    knockback.KnockBack(hitDir);
                }
            } else if (isProjectile(other))
            {
                Rigidbody2D projectileRb = other.GetComponent<Rigidbody2D>();
                Vector2 offset = (other.transform.position - Player.PlayerTransform.position).normalized;
                projectileRb.velocity = offset * projectileRb.velocity.magnitude;
                float rotation = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg + 90;
                other.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotation));
                other.GetComponent<BasicEnemyProjectile>().deflected = true;
            }
        }
    }

    bool isProjectile(Collider2D other)
    {
        return other.tag == "Enemy Projectile";
    }

    bool isEnemy (Collider2D other)
    {
        return other.tag == "Enemy" || other.tag == "Boss";
    }
}
