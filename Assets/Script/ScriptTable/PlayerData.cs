using UnityEngine;

[CreateAssetMenu(fileName = "New Data Player", menuName = "ScriptTable/ScoreData", order = 1)]
public class PlayerData : ScriptableObject
{
    public PlayerDataObj playerDataObj;
    public ScoreDataObj scoreDataObj;
}