using UnityEngine;

public class LockOnIndicator : MonoBehaviour {

    [SerializeField]
    private float minRadius = .5f;
    [SerializeField]
    private float startingRadius = 3;
    [SerializeField]
    private float radiusDelta = 4;
    [SerializeField]
    private float angleDelta = Mathf.PI;

    private StackPool lockOnArrowPool;
    private EnemyHealth enemyHealth;
    private SniperScope sniperScope;
    private float currentRadius;
    private float currentAngle;
    private bool activated = false;
    private GameObject arrow1, arrow2, arrow3, arrow4;

    void Awake ()
    {
        lockOnArrowPool = GameObject.Find("LockOnArrowPool").GetComponent<StackPool>();
        enemyHealth = GetComponent<EnemyHealth>();
        sniperScope = Camera.main.GetComponent<SniperScope>();
    }

    public void Activate()
    {
        if (!activated && ShouldBeActive())
        {
            activated = true;
            currentAngle = 0;
            currentRadius = startingRadius;
            arrow1 = lockOnArrowPool.Pop();
            arrow2 = lockOnArrowPool.Pop();
            arrow3 = lockOnArrowPool.Pop();
            arrow4 = lockOnArrowPool.Pop();

            arrow1.SetActive(true);
            arrow2.SetActive(true);
            arrow3.SetActive(true);
            arrow4.SetActive(true);
        }
    }

    public void Deactivate ()
    {
        if (activated)
        {
            arrow1.SetActive(false);
            arrow2.SetActive(false);
            arrow3.SetActive(false);
            arrow4.SetActive(false);

            lockOnArrowPool.Push(arrow1);
            lockOnArrowPool.Push(arrow2);
            lockOnArrowPool.Push(arrow3);
            lockOnArrowPool.Push(arrow4);

            activated = false;
        }
    }

    void Update()
    {
        if (activated)
        {
            SetArrow(arrow1.transform, currentAngle, currentRadius);
            SetArrow(arrow2.transform, currentAngle + Mathf.PI / 2, currentRadius);
            SetArrow(arrow3.transform, currentAngle + Mathf.PI, currentRadius);
            SetArrow(arrow4.transform, currentAngle + 3 * Mathf.PI / 2, currentRadius);

            if (currentRadius > minRadius)
            {
                currentRadius -= Time.deltaTime * radiusDelta;
            }

            currentAngle += Time.deltaTime * angleDelta;

            if (!ShouldBeActive())
            {
                Deactivate();
            }
        }
    }

    bool ShouldBeActive()
    {
        return enemyHealth.health > 0 && sniperScope.IsActive();
    }

    void SetArrow(Transform t, float angle, float radius)
    {
        t.position = new Vector2(transform.position.x + Mathf.Cos(angle) * radius, transform.position.y + Mathf.Sin(angle) * radius);
        t.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg - 90);
    }
}
