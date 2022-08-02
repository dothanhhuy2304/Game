using UnityEngine;
using Game.Player;

public class Car : MonoBehaviour
{
    [SerializeField] private Transform[] carTrans;
    [SerializeField] private Vector3[] currentPos;

    private void Start()
    {
        currentPos[0] = carTrans[0].position;
        currentPos[1] = carTrans[1].position;
        currentPos[2] = carTrans[2].position;
    }

    private void Update()
    {
        if (HuyManager.PlayerIsDeath())
        {
            StartCoroutine(nameof(WaitingPlayerRespawn), 4f);
        }
    }

    private System.Collections.IEnumerator WaitingPlayerRespawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        carTrans[0].position = currentPos[0];
        carTrans[1].position = currentPos[1];
        carTrans[2].position = currentPos[2];
    }
}