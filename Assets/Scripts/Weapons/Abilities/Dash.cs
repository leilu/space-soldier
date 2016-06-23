using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Dash : Weapon {
    [SerializeField]
    private float energyRequirement;
    [SerializeField]
    private float dashDistance;
    [SerializeField]
    private float dashSpeed;
    [SerializeField]
    private float targetDistanceStopThreshold;

    private bool dashing = false;
    private Vector2 target;
    private Rigidbody2D rb2d;

    public override float Click ()
    {
        if (!dashing)
        {
            dashing = true;
            Vector3 dashDir = VectorUtil.DirectionToMousePointer(Player.PlayerTransform);
            target = Player.PlayerTransform.position + dashDir * dashDistance;
            rb2d.velocity = dashDir * dashSpeed;
            return energyCost;
        }

        return 0;
    }

    void FixedUpdate ()
    {
        if ((rb2d.position - target).magnitude < targetDistanceStopThreshold)
        {
            dashing = false;
            rb2d.velocity = Vector2.zero;
        }
    }

    public override string GetDescription ()
    {
        return "Move really quickly.";
    }

    public override float GetEnergyRequirement ()
    {
        return energyRequirement;
    }

    public override string GetName ()
    {
        return "Dash";
    }

    public override Dictionary<string, object> GetProperties ()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add(WeaponProperties.EnergyCost, GetEnergyRequirement());
        return dict;
    }

    void Start()
    {
        rb2d = Player.PlayerTransform.GetComponent<Rigidbody2D>();
    }

    public bool IsDashing()
    {
        return dashing;
    }
}
