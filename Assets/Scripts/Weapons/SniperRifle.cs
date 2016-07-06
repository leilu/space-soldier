using UnityEngine;
using System.Collections.Generic;

public class SniperRifle : Weapon
{
    [SerializeField]
    private int damage;
    [SerializeField]
    private float energyCost;
    [SerializeField]
    private float enhancedMaxScrollFraction;
    [SerializeField]
    private float modifiedPlayerSpeed;

    private LineRenderer lineRenderer; 
    private RaycastHit2D lineRendererHit;
    private RaycastHit2D scopeLinecastHit;
    private PlayerMovement playerMovement;

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
        playerMovement = Player.PlayerTransform.GetComponent<PlayerMovement>();
    }

    void OnEnable ()
    {
        // TODO: Cache
        Camera.main.GetComponent<SniperScope>().Activate();
        Camera.main.GetComponent<CameraControl>().SetMaxScrollFraction(enhancedMaxScrollFraction);
        playerMovement.SetSpeed(modifiedPlayerSpeed);
        
        RenderLine();
    }

    void OnDisable()
    {
        // TODO: Cache
        Camera.main.GetComponent<SniperScope>().Deactivate();
        Camera.main.GetComponent<CameraControl>().ResetMaxScrollFraction();
        playerMovement.ResetSpeed();
    }

    void Update ()
    {
        RenderLine();

        foreach (LockOnIndicator indicator in GameState.LockOnTargets)
        {
            Vector2 origin = (Vector2)transform.position + GetStandardOffset();
            if (EnemyUtil.IsOnScreen(indicator.transform.position))
            {
                scopeLinecastHit = Physics2D.Linecast(origin, indicator.transform.position,
                    LayerMasks.PlayerSniperLayerMask);
                if (scopeLinecastHit != null && scopeLinecastHit.transform == indicator.transform)
                {
                    indicator.Activate();
                } else
                {
                    indicator.Deactivate();
                }
            } else
            {
                indicator.Deactivate();
            }
        }
    }

    void RenderLine ()
    {
        Vector2 origin = (Vector2)transform.position + GetStandardOffset();
        lineRendererHit = Physics2D.Raycast(origin, Camera.main.ScreenToWorldPoint(Input.mousePosition)
            - (Vector3)origin, Mathf.Infinity, LayerMasks.PlayerSniperLayerMask);
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, lineRendererHit.point);
    }
}
