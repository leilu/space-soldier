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
    private GameObject sniperMiniCam;

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
        sniperMiniCam = GameObject.Find("SniperMiniCam");
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

        if (EnemyUtil.IsOnScreen(Player.PlayerTransform.position))
        {
            sniperMiniCam.GetComponent<MeshRenderer>().enabled = false;
        } else
        {
            sniperMiniCam.GetComponent<MeshRenderer>().enabled = true;
            Vector2 playerViewportPos = Camera.main.WorldToViewportPoint(Player.PlayerTransform.position);
            float miniCamViewportX = Mathf.Clamp(playerViewportPos.x, 0, 1);
            float miniCamViewportY = Mathf.Clamp(playerViewportPos.y, 0, 1);
            Vector2 miniCamWorldCoords = Camera.main.ViewportToWorldPoint(new Vector2(miniCamViewportX, miniCamViewportY));
            sniperMiniCam.transform.position = new Vector3(miniCamWorldCoords.x, miniCamWorldCoords.y, -10);
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
