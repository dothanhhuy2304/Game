using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData",menuName = "ScriptTable/PlayerData",order = 1)]
public class PlayerData : ScriptableObject
{
   public int characterSelection;
   public Vector3 position;
   public int currentScenes;
}
