using UnityEngine;

public class MainMenuHitBoxLogic : MonoBehaviour
{
    private GameObject enemy;
    [SerializeField] private AudioClip[] EnemyHurtSounds;
    [Range(0, 1)][SerializeField] private float volume = 0.5f;

    public void DealDamageToEnemy()
    {
        if (enemy != null)
        {
            if (SoundFXManager.instance != null && EnemyHurtSounds != null && EnemyHurtSounds.Length > 0)
            {
                SoundFXManager.instance.PlayRandomSoundFXClip(EnemyHurtSounds, transform, volume);
            }
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
