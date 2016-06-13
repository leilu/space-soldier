using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    [SerializeField]
    private int healthPoints;
    [SerializeField]
    private Slider healthSlider;
    [SerializeField]
    private GameObject gameOverUI;
    [SerializeField]
    private float hitDuration;

    private SpriteRenderer spriteRenderer;

    private bool hitInProgress = false;

    void Awake()
    {
        healthSlider.maxValue = healthPoints;
        healthSlider.value = healthPoints;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void InflictDamage(int damage)
    {
        healthPoints -= damage;
        healthSlider.value = healthPoints;

        if (healthPoints <= 0)
        {
            gameObject.SetActive(false);
            ShowGameOverScreen();
        } else
        {
            HandleHit();
        }
    }

    void ShowGameOverScreen()
    {
        gameOverUI.SetActive(true);
    }

    void HandleHit ()
    {
        if (!hitInProgress)
        {
            spriteRenderer.material.SetFloat("_HitFlag", 1);
            hitInProgress = true;
            Invoke("EndHit", hitDuration);
        }
    }

    void EndHit ()
    {
        spriteRenderer.material.SetFloat("_HitFlag", 0);
        hitInProgress = false;
    }
}
