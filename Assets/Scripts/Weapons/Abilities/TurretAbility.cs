using UnityEngine;
using System.Collections.Generic;

public class TurretAbility : Weapon
{
    [SerializeField]
    private float energyCost;
    [SerializeField]
    private float duration;
    [SerializeField]
    private float timeBetweenShots;
    [SerializeField]
    private int damage;
    [SerializeField]
    private float range;
    [SerializeField]
    private float projectileSpeed;
    [SerializeField]
    private float missAmountDegrees;
    [SerializeField]
    private string stackPoolName;

    public override float Click ()
    {
        if (CanFire())
        {
            nextFiringTime = Time.time + FiringDelay;
            TurretAI turret = StackPool.Pop().GetComponent<TurretAI>();
            turret.Init(range, duration, timeBetweenShots, stackPoolName, damage, projectileSpeed,
                missAmountDegrees, Player.PlayerTransform.position);

            return energyCost;
        }

        return 0;
    }

    public override string GetDescription ()
    {
        return "Build a turret.";
    }

    public override float GetEnergyRequirement ()
    {
        return energyCost;
    }

    public override string GetName ()
    {
        return "Deploy Turret";
    }

    public override Dictionary<string, object> GetProperties ()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add(WeaponProperties.EnergyCost, GetEnergyRequirement());
        dict.Add(WeaponProperties.Damage, damage);
        return dict;
    }
}
