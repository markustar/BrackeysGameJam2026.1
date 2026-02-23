using UnityEngine;

public class MainMenuHitBoxLogic : MonoBehaviour
{
    private GameObject enemy;

    public void DealDamageToEnemy()
    {
        if (enemy != null)
        {
            enemy.GetComponent<MainMenuEnemyLogic>().TakeDamage();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("MainMenuEnemy"))
        {
            enemy = collider.gameObject;
        }
    }
}
