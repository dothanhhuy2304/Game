using UnityEngine;

public class CheckDisplayEnemy : MonoBehaviour
{
    [SerializeField] private GameObject environment;


    private void OnBecameVisible()
    {
        environment.SetActive(true);
    }


    private void OnBecameInvisible()
    {
        environment.SetActive(false);
    }
}
