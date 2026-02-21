using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int damage = 15;
    EnemyHealth enemyHealth;
    void Awake()
    {
        enemyHealth = FindAnyObjectByType<EnemyHealth>();
    }
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
        if(enemy)
        {
            enemyHealth.TakeDamage(damage);
        }
    }
}
