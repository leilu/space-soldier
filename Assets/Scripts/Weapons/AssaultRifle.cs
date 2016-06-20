using UnityEngine;
using System.Collections.Generic;
using System;

public class AssaultRifle : Weapon {

    public float energyCost;

    [SerializeField]
    private float timeBetweenBullets;
    [SerializeField]
    private int bulletsPerBurst;
    [SerializeField]
    private int damage;
    [SerializeField]
    private int missAmountDegrees;

    private int bulletsFiredThisBurst;

    public override float Click ()
    {
        if (CanFire())
        {
            nextFiringTime = Time.time + FiringDelay;
            FireBullet();
            return energyCost;
        }

        return 0;
    }

    void FireBullet()
    {
        bulletsFiredThisBurst++;

        GameObject bullet = StackPool.Pop();
        Vector2 addend = GetStandardOffset();
        bullet.transform.position = new Vector2(transform.position.x + addend.x, transform.position.y + addend.y);
        bullet.GetComponent<BasicPlayerProjectile>().Damage = damage;
        FireStandardProjectile(bullet, missAmountDegrees);

        if (bulletsFiredThisBurst < bulletsPerBurst)
        {
            Invoke("FireBullet", timeBetweenBullets);
        } else
        {
            bulletsFiredThisBurst = 0;
        }
    }

    public override float GetEnergyRequirement ()
    {
        return energyCost;
    }

    public override string GetName ()
    {
        return "Assault Rifle";
    }

    public override Dictionary<string, object> GetProperties ()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add(WeaponProperties.EnergyCost, GetEnergyRequirement());
        return dict;
    }

    public override string GetDescription ()
    {
        return "Fires in bursts.";
    }
}
