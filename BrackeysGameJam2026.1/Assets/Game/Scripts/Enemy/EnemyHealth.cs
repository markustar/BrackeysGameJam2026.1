using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamagable
{
    public float maxHealth { get; set; }
    public float currentHealth { get; set; }

    void Start()
    {
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
