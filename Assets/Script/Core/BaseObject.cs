using UnityEngine;

namespace Game.Core
{
    public abstract class BaseObject : MonoBehaviour
    {
        protected Rigidbody2D body;
        protected bool hasInteracted;
        private const float Radius = 35f;

        protected virtual void Start()
        {
            body = GetComponent<Rigidbody2D>();
        }

        protected virtual void CheckDistance(Vector2 player, Vector2 trans)
        {
            var distance = Vector2.Distance(player, trans);
            hasInteracted = distance <= Radius;
        }

        protected static float SetTimeAttack(ref float currentTime)
        {
            if (currentTime > 0f)
            {
                currentTime -= Time.deltaTime;
            }
            else
            {
                currentTime = 0f;
            }

            return currentTime;
        }
    }
}