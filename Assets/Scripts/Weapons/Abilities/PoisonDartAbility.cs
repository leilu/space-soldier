using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PoisonDartAbility : Weapon
{
    [SerializeField]
    private float energyCost;
    [SerializeField]
    private float damageMultiplier;
    [SerializeField]
    private float duration;

    public override float Click ()
    {
        if (CanFire())
        {
            nextFiringTime = Time.time + FiringDelay;
            GameObject obj = StackPool.Pop();
            Vector2 addend = GetStandardOffset();
            obj.transform.position = new Vector2(transform.position.x + addend.x, transform.position.y + addend.y);
            PoisonDart dart = obj.GetComponent<PoisonDart>();
            dart.DamageMultiplier = damageMultiplier;
            dart.Duration = duration;
            FireStandardProjectile(obj);

            return energyCost;
        }

        return 0;
    }

    public override string GetDescription ()
    {
        return "Makes enemies take more damage from all attacks.";
    }

    public override float GetEnergyRequirement ()
    {
        return energyCost;
    }

    public override string GetName ()
    {
        return "Poison Dartz";
    }

    public override Dictionary<string, object> GetProperties ()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add(WeaponProperties.EnergyCost, GetEnergyRequirement());
        return dict;
    }
}
