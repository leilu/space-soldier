using UnityEngine;
using System.Collections.Generic;

public class TimeDilation : Weapon {
    [SerializeField]
    private TimeDilationEffect effect;
    [SerializeField]
    private float energyRequirement;

    public override float Click ()
    {
        if (CanFire())
        {
            effect.ActivateEffect();
            return energyRequirement;
        }

        return 0;
    }

    public override string GetDescription ()
    {
        return "Slow down time";
    }

    public override float GetEnergyRequirement ()
    {
        return energyRequirement;
    }

    public override string GetName ()
    {
        return "Time Dilation";
    }

    public override Dictionary<string, object> GetProperties ()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add(WeaponProperties.EnergyCost, GetEnergyRequirement());
        return dict;
    }
}
