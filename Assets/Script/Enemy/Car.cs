using System.Collections;
using UnityEngine;
using Game.Core;

public class Car : BaseObject
{
    private PlayerHealth playerHealth;
    [SerializeField] private GameObject[] carTrans;
    [SerializeField] private Vector3[] currentPos;

    protected override void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>().GetComponent<PlayerHealth>();
        currentPos[0] = carTrans[0].transform.position;
        currentPos[1] = carTrans[1].transform.position;
        currentPos[2] = carTrans[2].transform.position;
    }

    private void Update()
    {
        if (!playerHealth.PlayerIsDeath()) return;
        StartCoroutine(nameof(WaitingPlayerRespawn), 3f);
    }

    private IEnumerator WaitingPlayerRespawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        carTrans[0].transform.position = currentPos[0];
        carTrans[1].transform.position = currentPos[1];
        carTrans[2].transform.position = currentPos[2];
    }
}