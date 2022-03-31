using UnityEngine;

namespace Game.Core
{
    public class BaseObject : MonoBehaviour
    {
        protected Rigidbody2D body;
        public virtual void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }
    }
}