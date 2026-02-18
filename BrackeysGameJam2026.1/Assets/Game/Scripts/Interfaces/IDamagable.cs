


public interface IDamagable 
{
    public float maxHealth {get; set;}
    public float currentHealth {get; set;}

    void TakeDamage(float damage);
}
