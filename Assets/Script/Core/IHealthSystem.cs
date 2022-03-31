
namespace Game.Core
{
    public interface IHealthSystem
    {
        // public HealthSystem(int healthMax)
        // {
        //     this.healthMax = healthMax;
        //     this.health = healthMax;
        // }
         void GetDamage(float damage);
        //{
            // health -= damage;
            // if (health <= 0) health = 0;
            // OnChange?.Invoke();
        //}
         void Heal(float value);
        //{
            // health += value;
            // if (health > healthMax) health = healthMax;
            // OnChange?.Invoke();
        //}
        void Die();
    }
}
