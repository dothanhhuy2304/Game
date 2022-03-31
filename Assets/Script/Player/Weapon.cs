using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Player
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private GameObject[] bulletHolder;
        [SerializeField] private AudioClip clip;
        [SerializeField] private Vector2 offset;
        private PlayerHealth playerHealth;

        private void Awake()
        {
            playerHealth = GetComponent<PlayerHealth>();
        }

        private void LateUpdate()
        {
            if (EventSystem.current.IsPointerOverGameObject() || playerHealth.PlayerIsDeath()) return;
            if (!Input.GetMouseButtonDown(0) && !Input.GetKeyDown(KeyCode.KeypadEnter)) return;
            //Instantiate(fireObj, transform.TransformPoint(offset), transform.rotation);
            bulletHolder[FindBullet()].transform.position = transform.TransformPoint(offset);
            bulletHolder[FindBullet()].transform.rotation = transform.rotation;
            bulletHolder[FindBullet()].GetComponent<FireProjectile>().SetActives();
            AudioSource.PlayClipAtPoint(clip, transform.position, 1f);
        }

        private int FindBullet()
        {
            for (var i = 0; i < bulletHolder.Length; i++)
            {
                if (!bulletHolder[i].activeInHierarchy)
                {
                    return i;
                }
            }

            return 0;
        }

    }
}