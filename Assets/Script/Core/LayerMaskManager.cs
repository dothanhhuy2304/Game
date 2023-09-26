using UnityEngine;

public class LayerMaskManager : FastSingleton<LayerMaskManager>
{
    public LayerMask playerMask;
    public LayerMask enemyMask;
    public LayerMask onlyPlayerMask;

    protected override void Awake()
    {
        base.Awake();
    }
}
