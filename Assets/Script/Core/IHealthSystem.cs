
namespace Script.Core
{
    public interface IHealthSystem
    {
        void GetDamage(float damage);
        void Healing(float value);
        void Die();
    }
}
