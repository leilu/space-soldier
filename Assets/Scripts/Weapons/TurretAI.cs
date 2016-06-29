using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class TurretAI : MonoBehaviour {

    [SerializeField]
    private float range;
    [SerializeField]
    private float timeBetweenShots;
    [SerializeField]
    private int damage;
    [SerializeField]
    private float projectileSpeed;
    [SerializeField]
    private float missAmountDegrees;

    private GameObject target;
    private float sqrRange;
    private float nextShotTime = 0;
    private StackPool projectileStackPool;
    private StackPool turretStackPool;

    private List<Collider2D> nearbyEnemyList;
    private Collider2D[] collidaz;

    public void Init(float range, float duration, float timeBetweenShots, string stackPoolName, int damage, float projectileSpeed,
        float missAmountDegrees, Vector2 position)
    {
        this.range = range;
        this.timeBetweenShots = timeBetweenShots;
        this.damage = damage;
        this.projectileSpeed = projectileSpeed;
        this.missAmountDegrees = missAmountDegrees;
        transform.position = position;
        gameObject.SetActive(true);

        sqrRange = range * range;
        projectileStackPool = GameObject.Find(stackPoolName).GetComponent<StackPool>();
        turretStackPool = GameObject.Find("TurretPool").GetComponent<StackPool>();
        Invoke("Destroy", duration);
    }

    void Destroy()
    {
        CancelInvoke("Destroy");
        gameObject.SetActive(false);
        turretStackPool.Push(gameObject);
    }

    void Update ()
    {
        if (!TargetIsValid(target))
        {
            target = null;
            collidaz = Physics2D.OverlapCircleAll(transform.position, range, LayerMasks.EnemyLayerMask);
            if (collidaz.Length > 0)
            {
                nearbyEnemyList = collidaz.Where(enemy => TargetIsValid(enemy.gameObject)).ToList();
                if (nearbyEnemyList.Count > 0)
                {
                    float minSqrMagnitude = (nearbyEnemyList[0].transform.position - transform.position).sqrMagnitude;
                    target = nearbyEnemyList[0].transform.gameObject;
                    for (int i = 1; i < nearbyEnemyList.Count; i++)
                    {
                        float sqrMagnitude = (nearbyEnemyList[i].transform.position - transform.position).sqrMagnitude;
                        if (sqrMagnitude < minSqrMagnitude)
                        {
                            minSqrMagnitude = sqrMagnitude;
                            target = nearbyEnemyList[i].gameObject;
                        }
                    }
                }
            }
        }

        if (Time.time > nextShotTime && target != null)
        {
            nextShotTime = Time.time + timeBetweenShots;
            GameObject projectile = projectileStackPool.Pop();
            float angleOffset = Random.Range(-missAmountDegrees, missAmountDegrees);

            Vector2 direction = VectorUtil.RotateVector(target.transform.position - transform.position, angleOffset * Mathf.Deg2Rad).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            projectile.transform.position = transform.position;

            projectile.SetActive(true);

            projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
            projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
            projectile.GetComponent<BasicPlayerProjectile>().Damage = damage;
        }
	}

    bool TargetIsValid(GameObject target)
    {
        return target != null && ((target.transform.position - transform.position).sqrMagnitude <= sqrRange && target.tag != "Killed" &&
            EnemyUtil.CanSee(transform.position, target.transform.position, true));
    }
}
