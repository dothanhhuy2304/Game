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