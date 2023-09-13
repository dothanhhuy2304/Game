using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Test : MonoBehaviour
{

    [SerializeField] private Transform start;

    [SerializeField] private Transform end;

    [SerializeField] private Ease type;
    // Start is called before the first frame update
    void Start()
    {
        start.DOJump(end.position, 3f, 2, 1).SetLoops(int.MaxValue);
        //point[1] = point[0] +(point[2] -point[0])/2 +Vector3.up *5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
