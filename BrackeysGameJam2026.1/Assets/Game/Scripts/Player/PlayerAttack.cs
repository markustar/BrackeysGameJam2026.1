using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int damage = 15;
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
            Debug.Log("Enemy Hit" + damage);
        }
    }
}
