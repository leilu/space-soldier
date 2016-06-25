using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Dash : Weapon {
    [SerializeField]
    private float energyRequirement;
    [SerializeField]
    private float maxDashDistance;
    [SerializeField]
    private float dashSpeed;
    [SerializeField]
    private float targetDistanceStopThreshold;

    public bool dashing = false;
    private Vector2 target;
    private Rigidbody2D rb2d;
    private float dashStartTime = 0;
    private float calculatedDashDistance;
    private BoxCollider2D boxCollider2D;

    public override float Click ()
    {
        if (!dashing && CanFire())
        {
            nextFiringTime = Time.time + FiringDelay;
            dashing = true;
            dashStartTime = Time.time;
            Vector3 dashDir = VectorUtil.DirectionToMousePointer(Player.PlayerTransform);
            RaycastHit2D obstacle = Physics2D.BoxCast((Vector2)Player.PlayerTransform.position + boxCollider2D.offset, boxCollider2D.size, 0, dashDir,
                maxDashDistance, LayerMasks.MovementObstructedLayerMask);

            calculatedDashDistance = obstacle.transform == null ? maxDashDistance : obstacle.distance;

            target = Player.PlayerTransform.position + dashDir * calculatedDashDistance;
            rb2d.velocity = dashDir * dashSpeed;
            return energyRequirement;
        }

        return 0;
    }

    void FixedUpdate ()
    {
        if (dashing)
        {
            float distanceTraveled = Mathf.Abs((Time.time - dashStartTime) * rb2d.velocity.magnitude);
            if (distanceTraveled >= calculatedDashDistance || (rb2d.position - target).magnitude < targetDistanceStopThreshold)
            {
                dashing = false;
                rb2d.velocity = Vector2.zero;
            }
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
        boxCollider2D = Player.PlayerTransform.GetComponent<BoxCollider2D>();
    }

    public bool IsDashing()
    {
        return dashing;
    }

    public void StopDash()
    {
        dashing = false;
    }
}
