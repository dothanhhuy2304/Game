using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUi : MonoBehaviour
{
    public float speed = 360f;
    public float radius = 1f;
    public GameObject particles;
    public Vector3 offset;
    public Transform particleSystemTrans;
    public bool isAnim;
    public string[] levelGame;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void Update()
    {
        if (!isAnim) return;
        particles.transform.Rotate(0f, 0f, Time.deltaTime * speed);
        particleSystemTrans.localPosition = Vector3.MoveTowards(particleSystemTrans.localPosition, offset, 0.5f * Time.deltaTime);
    }

    public void StartAnimationLoading()
    {
        isAnim = true;
        offset = new Vector3(radius, 0, 0);
        particles.SetActive(true);
    }

    public void StopAnimationLoading()
    {
        isAnim = false;
        offset = Vector3.zero;
        particles.transform.Rotate(Vector3.zero);
        particleSystemTrans.localPosition = Vector3.zero;
        particles.SetActive(false);
    }
    
    
}
