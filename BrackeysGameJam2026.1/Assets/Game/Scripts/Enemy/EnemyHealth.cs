using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamagable
{
    [SerializeField] private float initialHealth = 100f;
    
    public float maxHealth { get; set; }
    public float currentHealth { get; set; }

    void Start()
    {
        maxHealth = initialHealth;
        currentHealth = maxHealth;
    }

    void Update()
    {

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Destroy(this.gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }
}
