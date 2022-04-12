using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptTable/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
    public string itemName;
    public float valueReceive;
    public float scoreReceive;
    public float moneyReceive;
    public float diamondReceive;
}
