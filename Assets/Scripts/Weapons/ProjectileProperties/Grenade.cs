using UnityEngine;
using System.Collections;

public class Grenade : BasicPlayerProjectile {

    [SerializeField]
    private float decelerationRate;
    // Don't make collisionDistance too small or else the collision point may be inside of the collider, 
    // causing the normal to be the exact opposite of the velocity which is no good.
    [SerializeField]
    private float collisionDistance;
    [SerializeField]
    private float wallBounceMultiplier;
    [SerializeField]
    private float groundScale;

    private RaycastHit2D hit;
    private float height;
    private float verticalVelocity;

	protected override void OnTriggerEnter2D(Collider2D other)
    {
        
	}

    void FixedUpdate()
    {
        // Can't do this in OnTriggerEnter2D, since sometimes Unity just completely misses the event.
        hit = Physics2D.Raycast(transform.position, rb2d.velocity, collisionDistance, LayerMasks.WallAndObstacleLayerMask);

        float speed = rb2d.velocity.magnitude;
        speed = Mathf.Max(0, speed - decelerationRate * Time.deltaTime);
        rb2d.velocity = rb2d.velocity.normalized * speed;

        if (hit.transform != null)
        {
            rb2d.velocity = Vector2.Reflect(rb2d.velocity, hit.normal) * wallBounceMultiplier;
        }

        // TODO: Add height-based scale adjustment for floor bounce
    }
}
