using UnityEngine;

namespace Game.Core
{
    public class BaseObject : MonoBehaviour
    {
        protected Rigidbody2D body;

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {
            body = GetComponent<Rigidbody2D>();
        }

        protected virtual void Update()
        {

        }

        protected virtual void FixedUpdate()
        {

        }

        protected static float CheckDistance(Vector3 trans, Vector3 target)
        {
            return Vector3.Distance(trans, target);
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