using UnityEngine;

namespace Game.Core
{
    public static class BaseObject
    {

        public static void SetTimeAttack(ref float currentTime)
        {
            if (currentTime > 0f)
            {
                currentTime -= Time.deltaTime;
            }
            else
            {
                currentTime = 0f;
            }
        }

    }
}