using System.Collections.Generic;
using UnityEngine;

namespace Script.ScriptTable
{
    [CreateAssetMenu(fileName = "New Character", menuName = "ScriptTable/Character", order = 1)]
    public class CharacterScriptTableObject : ScriptableObject
    {
        public List<GameObject> character;
    }
}