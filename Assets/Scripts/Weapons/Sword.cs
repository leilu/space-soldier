using UnityEngine;
using System;
using System.Collections.Generic;

public class Sword : Weapon {
    [SerializeField]
    private int damage;
    private Slash slash;
    private bool released = true;

    void Awake()
    {
        slash = GameObject.Find("Slash").GetComponent<Slash>();
    }

    public override float Click ()
    {
        // The "released" variable is used since normally I allow the player to hold down the mouse to fire repeatedly.
        if (CanFire() && released)
        {
            nextFiringTime = Time.time + FiringDelay;
            DoSlash();
            released = false;
        }

        return 0;
    }

    public override float Release ()
    {
        released = true;
        return 0;
    }

    public override string GetDescription ()
    {
        return "Does melee damage and deflects enemy projectiles";
    }

    public override float GetEnergyRequirement ()
    {
        return 0;
    }

    public override string GetName ()
    {
        return "Sword";
    }

    public override Dictionary<string, object> GetProperties ()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add(WeaponProperties.Damage, damage);
        dict.Add(WeaponProperties.EnergyCost, GetEnergyRequirement());
        return dict;
    }

    void DoSlash()
    {
        slash.DoSlash(damage);
        Invoke("HideSlash", .15f);
    }

    void HideSlash()
    {
        slash.Hide();
    }
}
