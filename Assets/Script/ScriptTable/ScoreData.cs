using UnityEngine;

[CreateAssetMenu(fileName = "ScoreData",menuName = "ScriptTable/ScoreData",order = 1)]
public class ScoreData : ScriptableObject
{
    public float currentScore;
    public float highScore;
    public float money;
    public float diamond;
}
