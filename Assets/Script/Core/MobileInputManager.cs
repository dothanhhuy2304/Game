using UnityEngine;

public class MobileInputManager : MonoBehaviour
{
    public VariableJoystick joystick;
    public UnityEngine.UI.Button btnJump;
    public UnityEngine.UI.Button btnShot;
    public UnityEngine.UI.Button btnDash;
    [SerializeField] private GameObject objMobileInput;


    private void Awake()
    {
#if UNITY_STANDALONE
        objMobileInput.SetActive(false);
#elif UNITY_ANDROID|| UNITY_IOS
        objMobileInput.SetActive(true);
#endif
    }
}
