using DG.Tweening;
using UnityEngine;

namespace Script.Core
{
    public abstract class MoveLandController : MonoBehaviour
    {
        [SerializeField] protected float timeEndAction;
        protected int numberLoop;
        private int currentAnimation;

        protected static void MoveLandNormal(Transform startPosition, Vector2 endPosition, float timeEndActions,
            int loop)
        {
            startPosition.DOMove(endPosition, timeEndActions).SetEase(Ease.Linear).SetLoops(loop, LoopType.Yoyo);
        }

        protected void MoveLandNormalWithAnimation(Transform startPosition, Vector2 endPosition, float timeEndActions,
            int loop, Animator animator, string anim1, string anim2)
        {
            startPosition.DOMove(endPosition, timeEndAction)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    if (currentAnimation == 0)
                    {
                        animator.Play(anim1);
                        currentAnimation++;
                    }
                    else
                    {
                        animator.Play(anim2);
                        currentAnimation = 0;
                    }
                }).SetLoops(loop);
        }
    }
}
