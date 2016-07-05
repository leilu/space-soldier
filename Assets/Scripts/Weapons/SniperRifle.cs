using UnityEngine;
using System.Collections.Generic;

public class SniperRifle : Weapon
{
    [SerializeField]
    private int damage;
    [SerializeField]
    private float energyCost;
    [SerializeField]
    private SniperScope sniperScope;

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
        return "Dat snipa rifle doe";
    }

    public override float GetEnergyRequirement ()
    {
        return energyCost;
    }

    public override string GetName ()
    {
        return "Sniper Rifle";
    }

    public override Dictionary<string, object> GetProperties ()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add(WeaponProperties.EnergyCost, GetEnergyRequirement());
        dict.Add(WeaponProperties.Damage, damage);
        return dict;
    }

    void Awake ()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void OnEnable ()
    {
        sniperScope.Activate();
        RenderLine();
    }

    void OnDisable()
    {
        sniperScope.Deactivate();
    }

    void Update ()
    {
        RenderLine();
    }

    void RenderLine ()
    {
        Vector2 origin = (Vector2)transform.position + GetStandardOffset();
        hit = Physics2D.Raycast(origin, Camera.main.ScreenToWorldPoint(Input.mousePosition)
            - transform.position, Mathf.Infinity, LayerMasks.PlayerSniperLayerMask);
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, hit.point);

        if (1 << hit.transform.gameObject.layer == LayerMasks.EnemyLayerMask)
        {
            hit.transform.GetComponent<LockOnIndicator>().Activate();
        }
    }
}
