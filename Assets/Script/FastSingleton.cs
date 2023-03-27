using UnityEngine;

/// <summary>
/// FastSingleton MonoBehaviour ---> For fasted reference purpose
/// Update:2020-04-21 by hungtx
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class FastSingleton<T> : MonoBehaviour where T : FastSingleton<T> {
    /// <summary>
    /// The public static reference to the instance
    /// Call if you sure the reference is available
    /// </summary>
    public static T instance;
    private static bool instantiated; //checking bool is faster than checking null
    /// <summary>
    /// This is safe reference
    /// Call to get reference in Awake Function
    /// </summary>
    public static T instanceSafe {
        get {
            if (instantiated) {
                return instance;
            }
            instance = (T) FindObjectOfType(typeof(T));
            if (!instance) {
                GameObject gameObject = new GameObject(typeof(T).ToString());
                instance = gameObject.AddComponent<T>();
            }
            if (instance) {
                instantiated = true;
            }
            return instance;
        }
    }
    protected virtual void Awake() {
        // Make instance in Awake to make reference performance uniformly.
        if (!instance) {
            instance = (T) this;
            instantiated = true;
        }
        // If there is an instance already in the same scene, destroy this script.
        else if (instance != this) {
            Debug.Log("Singleton " + typeof(T) + " is already exists.");
            Destroy(gameObject);
        }
    }
    protected virtual void OnDestroy() {
        if (instance == this) {
            instance = null;
            instantiated = false;
        }
    }
}