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
    [SerializeField]
    private float boxCastOffset;

    public bool dashing = false;
    private Vector2 target;
    private Rigidbody2D rb2d;
    private float dashStartTime = 0;
    private BoxCollider2D boxCollider2D;

    public override float Click ()
    {
        if (!dashing && CanFire())
        {
            Vector3 dashDir = VectorUtil.DirectionToMousePointer(Player.PlayerTransform);
            Vector2 boxCastOffsetVector = boxCastOffset * dashDir;
            RaycastHit2D obstacle = Physics2D.BoxCast((Vector2)Player.PlayerTransform.position + boxCollider2D.offset + boxCastOffsetVector,
                boxCollider2D.size, 0, dashDir, maxDashDistance, LayerMasks.MovementObstructedLayerMask);

            rb2d.velocity = Vector2.zero;
            float calculatedDashDistance = obstacle.transform == null ? maxDashDistance : obstacle.distance + boxCastOffset;

            if (calculatedDashDistance <= boxCastOffset)
            {
                return 0;
            }

            nextFiringTime = Time.time + FiringDelay;
            dashing = true;
            dashStartTime = Time.time;

            target = Player.PlayerTransform.position + dashDir * calculatedDashDistance;
            return energyRequirement;
        }

        return 0;
    }

    void FixedUpdate ()
    {
        if (dashing)
        {
            Player.PlayerTransform.position = Vector2.MoveTowards(Player.PlayerTransform.position, target,
                dashSpeed * Time.fixedDeltaTime);
            if (Mathf.Abs(((Vector2)Player.PlayerTransform.position - target).sqrMagnitude) == 0)
            {
                dashing = false;
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
