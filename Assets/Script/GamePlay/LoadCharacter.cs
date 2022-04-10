using UnityEngine;

public class LoadCharacter : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private GameObject[] characters;

    private void Awake()
    {
        characters[playerData.characterSelection].SetActive(true);
    }
}
