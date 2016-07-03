using UnityEngine;
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
    [SerializeField]
    private float minDashAmount;
    [SerializeField]
    private float wallGlideBoxScale;
    [SerializeField]
    private float minWallGlideDotProduct;

    public bool dashing = false;
    private int waypointIndex;
    private Rigidbody2D rb2d;
    private float dashStartTime = 0;
    private BoxCollider2D boxCollider2D;
    private List<Vector2> waypoints;
    private RaycastHit2D obstacle;

    public override float Click ()
    {
        if (!dashing && CanFire())
        {
            waypoints.Clear();
            waypointIndex = 0;

            Vector3 dashDir = VectorUtil.DirectionToMousePointer(Player.PlayerTransform);
            Vector2 boxCastOffsetVector = boxCastOffset * dashDir;
            obstacle = Physics2D.BoxCast((Vector2)Player.PlayerTransform.position + boxCollider2D.offset + boxCastOffsetVector,
                boxCollider2D.size, 0, dashDir, maxDashDistance, LayerMasks.MovementObstructedLayerMask);

            if (obstacle.transform != null)
            {
                float originalDistance = obstacle.distance + boxCastOffset;

                Vector2 currPos = Player.PlayerTransform.position + dashDir * originalDistance;
                bool alreadyTouchingCollider = false;

                if (originalDistance > boxCastOffset)
                {
                    waypoints.Add(currPos);
                } else
                {
                    currPos = Player.PlayerTransform.position;
                    alreadyTouchingCollider = true;
                }

                Vector2 newDir = CalculateNewDirection(dashDir, obstacle.normal);
                if (newDir != Vector2.zero)
                {
                    Vector2 dashOrigin = alreadyTouchingCollider ? (Vector2)Player.PlayerTransform.position : currPos + .05f * obstacle.normal;
                    obstacle = Physics2D.BoxCast(dashOrigin, boxCollider2D.size * (alreadyTouchingCollider ? wallGlideBoxScale : 1), 0, newDir, maxDashDistance,
                        LayerMasks.MovementObstructedLayerMask);
                    float subtrahend = newDir.x == 0 ? (1 - wallGlideBoxScale) * boxCollider2D.size.y / 2
                        : (1 - wallGlideBoxScale) * boxCollider2D.size.x / 2;
                    float distanceTraveled = (obstacle.transform == null ? maxDashDistance - originalDistance :
                        Mathf.Min(obstacle.distance, maxDashDistance - originalDistance)) - subtrahend;
                    if (distanceTraveled > minDashAmount)
                    {
                        currPos += newDir * distanceTraveled;
                        waypoints.Add(currPos);
                    }
                }
            } else
            {
                waypoints.Add(Player.PlayerTransform.position + dashDir * maxDashDistance);
            }

            if (waypoints.Count > 0)
            {
                rb2d.velocity = Vector2.zero;
                nextFiringTime = Time.time + FiringDelay;
                dashing = true;
                dashStartTime = Time.time;

                return energyRequirement;
            }
        }

        return 0;
    }

    Vector2 CalculateNewDirection(Vector2 dashDir, Vector2 normal)
    {
        float dashDirNormalAngle = Mathf.Acos(Vector2.Dot(dashDir, normal));
        float ninetyDegreesInRadians = Mathf.PI / 2;
        float rotationRadians = Vector3.Cross(dashDir, normal).z < 0 ? ninetyDegreesInRadians - dashDirNormalAngle
            : dashDirNormalAngle - ninetyDegreesInRadians;
        return Vector2.Dot(dashDir, normal) < minWallGlideDotProduct ? Vector2.zero : VectorUtil.RotateVector(dashDir, rotationRadians);
    }

    void FixedUpdate ()
    {
        if (dashing)
        {
            Player.PlayerTransform.position = Vector2.MoveTowards(Player.PlayerTransform.position, waypoints[waypointIndex],
                dashSpeed * Time.fixedDeltaTime);
            if (Mathf.Abs(((Vector2)Player.PlayerTransform.position - waypoints[waypointIndex]).sqrMagnitude) == 0)
            {
                if (waypointIndex < waypoints.Count - 1)
                {
                    waypointIndex++;
                } else
                {
                    dashing = false;
                }
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

    void Start ()
    {
        rb2d = Player.PlayerTransform.GetComponent<Rigidbody2D>();
        boxCollider2D = Player.PlayerTransform.GetComponent<BoxCollider2D>();
        dashing = false;
        waypoints = new List<Vector2>();
    }

    void OnLevelWasLoaded ()
    {
        dashing = false;
    }

    public bool IsDashing()
    {
        return dashing;
    }

    public void StopDash()
    {
        dashing = false;
    }

    void Awake ()
    {
        // Since GameObject.Find doesn't work on inactive objects (lame!)
        Player.PlayerTransform.GetComponent<PlayerMovement>().Dash = this;
        Player.PlayerTransform.GetComponent<PlayerHealth>().Dash = this;
    }
}
