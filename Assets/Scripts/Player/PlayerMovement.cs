using UnityEngine;
using SpriteTile;

public class PlayerMovement : MonoBehaviour {
    public float speed;
    public float collisionDistance;
    public int wallSpriteTileLayer;
    public float colliderMovementMultiplier;

    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider;
    private Animator animator;
    private Dash dash;
    private float inputX, inputY;

    void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        dash = GameObject.Find("Dash").GetComponent<Dash>();
	}

    void Update()
    {
        if (GameState.Paused || GameState.InputLocked)
        {
            return;
        }

        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        if (GameState.TutorialMode && (inputX != 0 || inputY != 0))
        {
            TutorialEngine.Instance.Trigger(TutorialTrigger.Walk);
        }
    }

    void FixedUpdate()
    {
        if (GameState.Paused || GameState.InputLocked || dash.IsDashing())
        {
            return;
        }

        Vector2 newVelocity = new Vector2(inputX, inputY).normalized * speed;

        Vector2 colliderMin = new Vector2(boxCollider.bounds.min.x + colliderMovementMultiplier * boxCollider.bounds.extents.x,
            boxCollider.bounds.min.y + colliderMovementMultiplier * boxCollider.bounds.extents.y);
        Vector2 colliderMax = new Vector2(boxCollider.bounds.max.x - colliderMovementMultiplier * boxCollider.bounds.extents.x,
            boxCollider.bounds.max.y - colliderMovementMultiplier * boxCollider.bounds.extents.y);

        if (Physics2D.OverlapArea(new Vector2(colliderMin.x + collisionDistance, colliderMin.y), 
            new Vector2(colliderMax.x + collisionDistance, colliderMax.y), LayerMasks.MovementObstructedLayerMask) != null)
        {
            newVelocity.x = Mathf.Min(0, newVelocity.x);
        }

        if (Physics2D.OverlapArea(new Vector2(colliderMin.x - collisionDistance, colliderMin.y),
            new Vector2(colliderMax.x - collisionDistance, colliderMax.y), LayerMasks.MovementObstructedLayerMask) != null)
        {
            newVelocity.x = Mathf.Max(0, newVelocity.x);
        }

        if (Physics2D.OverlapArea(new Vector2(colliderMin.x, colliderMin.y + collisionDistance),
            new Vector2(colliderMax.x, colliderMax.y + collisionDistance), LayerMasks.MovementObstructedLayerMask) != null)
        {
            newVelocity.y = Mathf.Min(0, newVelocity.y);
        }

        if (Physics2D.OverlapArea(new Vector2(colliderMin.x, colliderMin.y - collisionDistance),
            new Vector2(colliderMax.x, colliderMax.y - collisionDistance), LayerMasks.MovementObstructedLayerMask) != null)
        {
            newVelocity.y = Mathf.Max(0, newVelocity.y);
        }

        rb2d.velocity = newVelocity;
        animator.SetInteger("HorizontalAxis", (int)newVelocity.x);
        animator.SetInteger("VerticalAxis", (int)newVelocity.y);
    }
}
