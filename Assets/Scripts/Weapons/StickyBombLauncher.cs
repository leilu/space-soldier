using UnityEngine;
using System.Collections.Generic;

public class StickyBombLauncher : Weapon
{
    public float energyCost;
    [SerializeField]
    private int explosionDamage;
    [SerializeField]
    private int explosionRadius;

    public override float Click ()
    {
        if (CanFire())
        {
            nextFiringTime = Time.time + FiringDelay;
            GameObject stickyBomb = StackPool.Pop();
            Vector2 addend = GetStandardOffset();
            stickyBomb.transform.position = new Vector2(transform.position.x + addend.x, transform.position.y + addend.y);
            StickyBomb bombComponent = stickyBomb.GetComponent<StickyBomb>();
            bombComponent.ExplosionDamage = explosionDamage;
            bombComponent.ExplosionRadius = explosionRadius;
            FireStandardProjectile(stickyBomb);

            return energyCost;
        }

        return 0;
    }

    public override string GetDescription ()
    {
        return "Sticks to enemies, explodes when shot.";
    }

    public override float GetEnergyRequirement ()
    {
        return energyCost;
    }

    public override string GetName ()
    {
        return "Sticky Bomb";
    }

    public override Dictionary<string, object> GetProperties ()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add(WeaponProperties.EnergyCost, GetEnergyRequirement());
        dict.Add(WeaponProperties.ExplosiveDamage, explosionDamage);
        dict.Add(WeaponProperties.ExplosiveRadius, explosionRadius);
        return dict;
    }
}
