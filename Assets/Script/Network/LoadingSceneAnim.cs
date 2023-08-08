using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class LoadingSceneAnim : MonoBehaviour
{
    [Header("Loading")] public float speed = 360f;
    public float radius = 1f;
    public GameObject particles;
    public Vector3 offset;
    public Transform partialSystemTrans;
    public bool isAnim;
    public TMP_Text feedBackText;
    public string[] levelGame;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (!isAnim) return;
        particles.transform.Rotate(0f, 0f, Time.deltaTime * speed);
        partialSystemTrans.localPosition = Vector3.MoveTowards(partialSystemTrans.localPosition, offset, 0.5f * Time.deltaTime);
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
        partialSystemTrans.localPosition = Vector3.zero;
        particles.SetActive(false);
    }
    /// <summary>
    /// Using to log message when next state
    /// </summary>
    /// <param name="message">Content message</param>
    public void LogFeedback(string message, bool reset = false)
    {
        feedBackText.text += "Connect status:" + PhotonNetwork.NetworkClientState;
    }
}
