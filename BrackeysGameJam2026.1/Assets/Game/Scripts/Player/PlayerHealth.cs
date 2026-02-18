using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    public float maxHealth  { get ; set ; } = 100;
   public float currentHealth { get ; set; } 

    

   
    void Start()
    {
        currentHealth = maxHealth;
    }

    
    void Update()
    {
        
        if(currentHealth <=0)
        {
            currentHealth = 0;
            Death();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }

    public void Death()
    {
        Destroy(this.gameObject);
    }
}
