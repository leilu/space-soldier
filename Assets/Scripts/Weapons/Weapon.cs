﻿using UnityEngine;
using System.Collections.Generic;

public abstract class Weapon : MonoBehaviour
{
    public int Points;
    public StackPool StackPool;
    public float FiringDelay;
    public float ProjectileSpeed;
    public Vector2 LocalLeftProjectileOffset;
    public Vector2 LocalRightProjectileOffset;
    public Vector2 LeftOffset;
    public Vector2 RightOffset;

    public bool Equipped = false;
    public bool FacingLeft = true;

    protected float nextFiringTime = 0;
    protected Vector2 activeProjectileOffset;

    public abstract float GetEnergyRequirement();
    public abstract float Click();
    public abstract string GetName();
    public abstract string GetDescription ();
    public abstract Dictionary<string, object> GetProperties ();

    public virtual float Release()
    {
        return 0;
    }

    protected bool CanFire()
    {
        return Time.time > nextFiringTime;
    }

    public void IncrementPoints()
    {
        Points++;
    }

    public void SetToRightSide()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        transform.localPosition = LeftOffset;
        transform.rotation = Quaternion.Euler(0, 0, VectorUtil.AngleToMousePointer(transform));
        activeProjectileOffset = LocalRightProjectileOffset;
        FacingLeft = false;
    }

    public void SetToLeftSide()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        transform.localPosition = RightOffset;
        transform.rotation = Quaternion.Euler(0, 180, 180 - VectorUtil.AngleToMousePointer(transform));
        activeProjectileOffset = LocalLeftProjectileOffset;
        FacingLeft = true;
    }

    protected void FireStandardProjectile(GameObject projectile, float missAmountDegrees = 0)
    {
        // this has to be done before setting velocity or it won't work.
        projectile.SetActive(true);

        float angleOffset = Random.Range(-missAmountDegrees, missAmountDegrees);

        // bullet rotation uses the vector from gun to target, not from player to target.
        Vector2 direction = VectorUtil.RotateVector(VectorUtil.DirectionToMousePointer(transform), angleOffset * Mathf.Deg2Rad);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
        projectile.GetComponent<Rigidbody2D>().velocity = direction * ProjectileSpeed;
    }

    protected Vector2 GetStandardOffset()
    {
        return VectorUtil.RotateVector(new Vector2(activeProjectileOffset.x, activeProjectileOffset.y),
                (FacingLeft ? -transform.rotation.eulerAngles.z : transform.rotation.eulerAngles.z) * Mathf.Deg2Rad);
    }
}