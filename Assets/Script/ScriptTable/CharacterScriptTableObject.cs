using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "ScriptTable/Character", order = 1)]
public class CharacterScriptTableObject : ScriptableObject
{
    public List<GameObject> character;
}
