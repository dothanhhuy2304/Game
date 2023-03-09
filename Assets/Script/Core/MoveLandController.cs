using DG.Tweening;
using UnityEngine;

public abstract class MoveLandController : MonoBehaviour
{
    [SerializeField] protected float timeEndAction;
    protected int numberLoop;

    protected static void MoveLandNormal(Transform startPosition, Vector2 endPosition, float timeDuration, int loop)
    {
        startPosition.DOMove(endPosition, timeDuration).SetEase(Ease.Linear).SetLoops(loop, LoopType.Yoyo);
    }
}
