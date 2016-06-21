using UnityEngine;
using System.Collections.Generic;

public class GrenadeLauncher : Weapon
{
    public float energyCost;
    [SerializeField]
    private int damage;

    public override float Click ()
    {
        if (CanFire())
        {
            nextFiringTime = Time.time + FiringDelay;
            GameObject grenade = StackPool.Pop();
            Vector2 addend = GetStandardOffset();
            grenade.transform.position = new Vector2(transform.position.x + addend.x, transform.position.y + addend.y);
            grenade.GetComponent<Grenade>().Damage = damage;
            FireStandardProjectile(grenade);

            return energyCost;
        }

        return 0;
    }

    public override float GetEnergyRequirement ()
    {
        return energyCost;
    }

    public override string GetName ()
    {
        return "Grenade Launcher";
    }

    public override Dictionary<string, object> GetProperties ()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add(WeaponProperties.EnergyCost, GetEnergyRequirement());
        return dict;
    }

    public override string GetDescription ()
    {
        return "Fires bouncy explosive things.";
    }
}
