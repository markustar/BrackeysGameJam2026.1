using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamagable
{
    [SerializeField] private float initialHealth = 100f;
    [SerializeField] private AudioClip[] enemyhurt;
    [Range(0,1)]
    [SerializeField] private float volume = 0.5f;
    
    public float maxHealth { get; set; }
    public float currentHealth { get; set; }

    void Start()
    {
        maxHealth = initialHealth;
        currentHealth = maxHealth;
        Debug.Log($"[EnemyHealth] START - Enemy: {gameObject.name}, Initial: {initialHealth}, Max: {maxHealth}, Current: {currentHealth}");
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            Debug.Log($"[EnemyHealth] DESTROYED - Enemy: {gameObject.name} died. CurrentHealth was: {currentHealth}");
            currentHealth = 0;
            Destroy(this.gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        HitStop hitStop = this.gameObject.GetComponent<HitStop>();
        if (hitStop != null)
        {
            hitStop.StopTime(0.1f);
        }

        DamageFlash damageFlash = this.gameObject.GetComponentInChildren<DamageFlash>();
        if(damageFlash != null)
        {   
            damageFlash.CallCouroutine();
        }

        SoundFXManager.instance.PlayRandomSoundFXClip(enemyhurt, this.transform, volume);
    }
}
