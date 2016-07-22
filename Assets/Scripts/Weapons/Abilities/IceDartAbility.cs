using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class IceDartAbility : Weapon {
    [SerializeField]
    private float energyCost;
    [SerializeField]
    private float freezeDuration;

    public override float Click ()
    {
        if (CanFire())
        {
            nextFiringTime = Time.time + FiringDelay;
            GameObject obj = StackPool.Pop();
            Vector2 addend = GetStandardOffset();
            obj.transform.position = new Vector2(transform.position.x + addend.x, transform.position.y + addend.y);
            IceDart dart = obj.GetComponent<IceDart>();
            dart.FreezeDuration = freezeDuration;
            FireStandardProjectile(obj);
            Debug.Log("fire dat b?");

            return energyCost;
        }

        return 0;
    }

    public override string GetDescription ()
    {
        return "Freeze enemies temporarily";
    }

    public override float GetEnergyRequirement ()
    {
        return energyCost;
    }

    public override string GetName ()
    {
        return "Ice Darts";
    }

    public override Dictionary<string, object> GetProperties ()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add(WeaponProperties.EnergyCost, GetEnergyRequirement());
        dict.Add(WeaponProperties.FreezeDuration, freezeDuration);
        return dict;
    }
}
