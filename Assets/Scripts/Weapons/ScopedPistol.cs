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

    void Update()
    {
        hit = Physics2D.Raycast(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)
            - transform.position, Mathf.Infinity, LayerMasks.SniperLayerMask);
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, hit.point);
    }
}
