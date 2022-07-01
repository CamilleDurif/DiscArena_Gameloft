using UnityEngine;
using UnityEngine.UI;

public class DestroyableObstacle : Obstacle
{ 
    [SerializeField] private Image healthBarfill;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private Animator obstacleAnimator;
    [SerializeField] private float maxHealth = 4;
    
    private float currentHealth;
    private float currentPuckDamage;
    
    void Start()
    {
        healthBar.SetActive(false);
        particles.Stop();
        currentHealth = maxHealth;
        currentPuckDamage = GameManager.Instance.puckPrefab.damage;
    }
    
    public override void HittingObstacle()
    {
        base.HittingObstacle();
        currentHealth -= currentPuckDamage;
        healthBarfill.fillAmount = currentHealth / maxHealth;

        if (obstacleAnimator != null)
        {
            obstacleAnimator.SetTrigger("Scale");
        }

        if (currentHealth < 1)
        {
            GameManager.Instance.OnObstacleDestroy(this);
        }
    }

    public void ShowHealthBar(bool isVisible)
    {
        if (healthBar.activeSelf == isVisible)
        {
            return;
        }
        healthBar.SetActive(isVisible);
    }
}
