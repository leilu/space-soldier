using UnityEngine;
using System.Collections.Generic;

public class MineAbility : Weapon
{
    public float energyCost; // TODO: move this into parent class
    [SerializeField]
    private int damage;
    [SerializeField]
    private int explosiveDamage;
    [SerializeField]
    private int explosionRadius;

    public override float Click ()
    {
        if (CanFire())
        {
            nextFiringTime = Time.time + FiringDelay;
            Mine mine = StackPool.Pop().GetComponent<Mine>();
            mine.transform.position = Player.PlayerTransform.position;
            mine.ImmediateDamage = damage;
            mine.gameObject.SetActive(true);

            return energyCost;
        }

        return 0;
    }

    public override string GetDescription ()
    {
        return "Place mines that blow up when enemies step on them.";
    }

    public override float GetEnergyRequirement ()
    {
        return energyCost;
    }

    public override string GetName ()
    {
        return "Landmines";
    }

    public override Dictionary<string, object> GetProperties ()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add(WeaponProperties.EnergyCost, GetEnergyRequirement());
        dict.Add(WeaponProperties.Damage, damage);
        dict.Add(WeaponProperties.ExplosiveDamage, explosiveDamage);
        return dict;
    }
}
