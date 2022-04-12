using System;
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

        protected virtual float CheckDistance(Vector3 trans, Vector3 target)
        {
            return Vector3.Distance(trans, target);
        }
    }
}