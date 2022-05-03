using System;
using UnityEngine;

namespace Game.Enemy
{
    public class CheckEnemyAttack : MonoBehaviour
    {

        [HideInInspector] public bool canAttack;
        private Collider2D col;
        [SerializeField] private Vector3 size = Vector3.zero;
        [SerializeField] private Vector3 direction = Vector3.zero;

        private void Start()
        {
            col = GetComponent<Collider2D>();
        }


        private void Update()
        {
            //canAttack = Physics2D.OverlapBox(transform.position, size, 0, 1 << LayerMask.NameToLayer("Player"));
        }


        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     if (other.CompareTag("Player"))
        //     {
        //         canAttack = true;
        //     }
        // }
        //
        // private void OnTriggerExit2D(Collider2D other)
        // {
        //     if (other.CompareTag("Player"))
        //     {
        //         canAttack = false;
        //     }
        // }
        //
        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(transform.position,size);
        }
    }
}