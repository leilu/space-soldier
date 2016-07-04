using UnityEngine;
using System.Collections.Generic;

public class ScopedPistol : Weapon {
    [SerializeField]
    private int damage;
    [SerializeField]
    private float energyCost;

    private LineRenderer lineRenderer;
    private RaycastHit2D hit;

    public override float Click ()
    {
        if (CanFire())
        {
            nextFiringTime = Time.time + FiringDelay;
            GameObject bullet = StackPool.Pop();
            Vector2 addend = GetStandardOffset();
            bullet.transform.position = new Vector2(transform.position.x + addend.x, transform.position.y + addend.y);
            bullet.GetComponent<BasicPlayerProjectile>().Damage = damage;
            FireStandardProjectile(bullet);

            return energyCost;
        }

        return 0;
    }

    public override string GetDescription ()
    {
        return "A stronger and more accurate pistol with a scope.";
    }

    // TODO: Make this virtual since it is the exact same in almost all cases
    public override float GetEnergyRequirement ()
    {
        return energyCost;
    }

    public override string GetName ()
    {
        return "Scoped Pistol";
    }

    public override Dictionary<string, object> GetProperties ()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add(WeaponProperties.EnergyCost, GetEnergyRequirement());
        dict.Add(WeaponProperties.Damage, damage);
        return dict;
    }

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void OnEnable()
    {
        RenderLine();
    }

    void Update()
    {
        RenderLine();
    }

    void RenderLine()
    {
        Vector2 origin = (Vector2)transform.position + GetStandardOffset();
        hit = Physics2D.Raycast(origin, Camera.main.ScreenToWorldPoint(Input.mousePosition)
            - transform.position, Mathf.Infinity, LayerMasks.WallAndObstacleLayerMask);
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, hit.point);
    }
}
