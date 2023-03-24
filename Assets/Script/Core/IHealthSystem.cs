
namespace Script.Core
{
    public interface IHealthSystem
    {
        void GetDamage(float damage);
        void Heal(float value);
        void Die();
    }
}
