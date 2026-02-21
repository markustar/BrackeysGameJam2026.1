using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    public float maxHealth  { get ; set ; } = 100;
    public float currentHealth { get ; set; } 
    [SerializeField] AudioClip[] playerHurt;
    [SerializeField] AudioClip playerDeath;
    [SerializeField] float timeStopDuration;

    [SerializeField] Image healthBarFull;
    [Range(0,1)]
    [SerializeField] float volume;

    PlayerMovement player;

   HitStop hit;
    void Awake()
    {
        hit = FindFirstObjectByType<HitStop>();
    }
    void Start()
    {
        currentHealth = maxHealth;
        player = GetComponent<PlayerMovement>();
    }

    
    void Update()
    {
        DebugTakeDamage();
        if(currentHealth <=0)
        {
            currentHealth = 0;
            Death();
        }
        
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (player != null) player.Addvelocity(0);
        
        if (DamageFlash.Instance != null) DamageFlash.Instance.CallCouroutine();
        if (hit != null) hit.StopTime(timeStopDuration);
        if (SoundFXManager.instance != null) SoundFXManager.instance.PlayRandomSoundFXClip(playerHurt, this.transform, volume);
        if (healthBarFull != null) healthBarFull.fillAmount = currentHealth / maxHealth;
    }

    public void Death()
    {   
        SoundFXManager.instance.PlaySoundFXClip(playerDeath, this.transform, volume);
        Destroy(this.gameObject);
    }

    void DebugTakeDamage()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            TakeDamage(25);
        }
    }


}
