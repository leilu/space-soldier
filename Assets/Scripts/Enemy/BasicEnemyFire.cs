﻿using UnityEngine;

public class BasicEnemyFire : EnemyWeapon {

    public float fireInterval = 2f;
    public float projectileSpeed = 10f;
    public string projectilePoolName;
    public int accuracyVariation = 30;

    private StackPool projectilePool;
    private float nextFireTime;

	void Start () {
        nextFireTime = Time.time + .5f;
        projectilePool = GameObject.Find(projectilePoolName).GetComponent<StackPool>();
	}
	
	public override int Fire () {
        int missAmountDegrees = Random.Range(-accuracyVariation, accuracyVariation);

        if (Time.time > nextFireTime)
        {
            nextFireTime = Time.time + fireInterval;
            GameObject projectile = projectilePool.Pop();
            projectile.transform.position = gameObject.transform.position;

            Vector3 offset = Player.PlayerTransform.position - gameObject.transform.position;
            float rotation = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg + 90 + missAmountDegrees;
            projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotation));

            projectile.SetActive(true);

            projectile.GetComponent<Rigidbody2D>().velocity = VectorUtil.RotateVector(projectileSpeed * offset.normalized,
                Mathf.Deg2Rad * missAmountDegrees);
            return 1;
        }

        return 0;
	}
}
