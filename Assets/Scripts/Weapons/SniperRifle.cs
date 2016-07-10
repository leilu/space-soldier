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
    [SerializeField]
    private float miniCamEdgeThreshold;
    [SerializeField]
    private float miniCamPadding;

    private LineRenderer lineRenderer; 
    private RaycastHit2D lineRendererHit;
    private RaycastHit2D[] scopeLinecastHits;
    private RaycastHit2D miniCamLinecastHit;
    private PlayerMovement playerMovement;
    private GameObject sniperMiniCam;
    private MeshRenderer sniperMiniCamMeshRenderer;
    private float sniperMiniCamOffset;

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
        lineRenderer.sortingLayerName = "Default";
        playerMovement = Player.PlayerTransform.GetComponent<PlayerMovement>();
        sniperMiniCam = GameObject.Find("SniperMiniCamQuad");
        sniperMiniCamMeshRenderer = sniperMiniCam.GetComponent<MeshRenderer>();
        sniperMiniCamOffset = sniperMiniCam.GetComponent<MeshCollider>().bounds.size.x / 2 + miniCamPadding;
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
        sniperMiniCamMeshRenderer.enabled = false;
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
                scopeLinecastHits = Physics2D.LinecastAll(origin, indicator.transform.position,
                    LayerMasks.PlayerSniperLayerMask);

                bool isHit = false;

                for (int i = 0; i < scopeLinecastHits.Length; i++)
                {
                    if (scopeLinecastHits[i].transform.tag != "Enemy")
                    {
                        break;
                    }

                    if (scopeLinecastHits[i].transform == indicator.transform)
                    {
                        isHit = true;
                        break;
                    }
                }

                if (isHit)
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

        Vector2 viewportPoint = Camera.main.WorldToViewportPoint(Player.PlayerTransform.position);

        if (viewportPoint.x > miniCamEdgeThreshold && viewportPoint.x < 1 - miniCamEdgeThreshold &&
            viewportPoint.y > miniCamEdgeThreshold && viewportPoint.y < 1 - miniCamEdgeThreshold)
        {
            sniperMiniCamMeshRenderer.enabled = false;
        } else
        {
            sniperMiniCamMeshRenderer.enabled = true;
            RepositionSniperMiniCam();
        }
    }

    void RepositionSniperMiniCam()
    {
        float yViewportOffset = sniperMiniCamOffset / (Camera.main.orthographicSize * 2);
        float xViewportOffset = sniperMiniCamOffset / (Camera.main.orthographicSize * 2 * Camera.main.aspect);

        float miniCamViewportX = xViewportOffset, miniCamViewportY = 1 - yViewportOffset;

        Vector2 miniCamWorldCoords = Camera.main.ViewportToWorldPoint(new Vector2(miniCamViewportX, miniCamViewportY));
        sniperMiniCam.transform.position = new Vector3(miniCamWorldCoords.x, miniCamWorldCoords.y, 0);
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
