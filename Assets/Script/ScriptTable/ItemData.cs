using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptTable/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
    public string itemName;
    public float valueReceive;
    public AudioClip soundCollection, soundHurtCollection;
}
