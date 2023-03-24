using System;
using System.Collections;
using System.Collections.Generic;
using Script.Player;
using UnityEngine;

public class Test3 : MonoBehaviour
{

    private Collider2D collider2Ds;
    private Rigidbody2D rb;
    [SerializeField] private Transform target;

    private bool wasRangerAttack;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = (target.position - transform.position).magnitude;

        if (distance < 2)
        {
            collider2Ds.isTrigger = true;
            wasRangerAttack = true;
        }
        else
        {
            wasRangerAttack = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            wasRangerAttack = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (wasRangerAttack)
        {
            if (other.CompareTag("Player"))
            {
                PlayerHealth.instance.GetDamage(20f);
            }
            else
            {
                return;
            }
        }
    }
}
