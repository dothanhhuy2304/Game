using UnityEngine;

namespace Script.ScriptTable
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptTable/DataGame", order = 1)]
    public class Data : ScriptableObject
    {
        public float movingSpeed;
        public float jumpForce;
        public float dashSpeed;
        public float heathDefault;
        public float currentHealth;
        public float maxHealth;
        public float hpIc;
        public float damageAttack;
    }
}