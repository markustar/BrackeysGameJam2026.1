using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    public float maxHealth  { get ; set ; } = 100;
    public float currentHealth { get ; set; } 
    [SerializeField] AudioClip[] playerHurt;
    [SerializeField] AudioClip playerDeath;

    [SerializeField] Image healthBarFull;
    [Range(0,1)]
    [SerializeField] float volume;

   
    void Start()
    {
        currentHealth = maxHealth;
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
