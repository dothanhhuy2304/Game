using UnityEngine;

public class SawPatrol : MonoBehaviour
{
    [SerializeField] private bool useLerp;
    [SerializeField] private Vector2[] listPoint;
    private int currentPoint;
    [SerializeField] private float speed = 2f;

    private void Update()
    {
        if (useLerp)
        {
            transform.position = Vector2.Lerp(transform.position, listPoint[currentPoint], speed * Time.deltaTime);
        }
        else
        {
            transform.position =
                Vector2.MoveTowards(transform.position, listPoint[currentPoint], speed * Time.deltaTime);
        }

        if (Vector2.Distance(transform.position, listPoint[currentPoint]) < 0.1f)
        {
            GetCurrentPoint();
        }
    }

    private void GetCurrentPoint()
    {
        if (listPoint.Length - 1 > currentPoint)
        {
            currentPoint++;
        }
        else
        {
            currentPoint = 0;
        }
    }
}
