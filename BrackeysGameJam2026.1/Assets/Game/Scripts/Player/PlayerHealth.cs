using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    public float maxHealth  { get ; set ; } = 1000;
    public float currentHealth { get ; set; } 
    [SerializeField] AudioClip[] playerHurt;
    [SerializeField] AudioClip playerDeath;
    [SerializeField] float timeStopDuration;

    [SerializeField] Image healthBarFull;
    [Range(0,1)]
    [SerializeField] float volume;

    PlayerMovement player;

   
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
        player.Addvelocity(0);
        DamageFlash.Instance.CallCouroutine();
        HitStop.Instance.StopTime(timeStopDuration);
        SoundFXManager.instance.PlayRandomSoundFXClip(playerHurt, this.transform, volume);
        healthBarFull.fillAmount = currentHealth / maxHealth;
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
