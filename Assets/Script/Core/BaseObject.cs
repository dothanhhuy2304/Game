using UnityEngine;

namespace Game.Core
{
    public abstract class BaseObject : MonoBehaviour
    {
        protected Rigidbody2D body;
        protected bool isVisible;

        protected virtual void Start()
        {
            body = GetComponent<Rigidbody2D>();
        }

        protected static void SetTimeAttack(ref float currentTime)
        {
            if (currentTime > 0f)
            {
                currentTime -= Time.deltaTime;
            }
            else
            {
                currentTime = 0f;
            }
        }

        private void OnBecameVisible()
        {
            isVisible = true;
        }

        private void OnBecameInvisible()
        {
            isVisible = false;
        }

    }
}