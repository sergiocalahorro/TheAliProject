using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // Check if it's about to be destroyed
    private static bool _shuttingDown = false;
    private static object _lock = new object();
    private static T _instance;

    /// <summary>
    /// Access to Singleton instance
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_shuttingDown)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                                 "' already destroyed. Returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    // Search for existing instance
                    _instance = (T)FindObjectOfType(typeof(T));

                    // Create new instance if one doesn't already exist
                    if (_instance == null)
                    {
                        // Need to create a new GameObject to attach the Singleton to
                        GameObject singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";

                        // Make instance persistent
                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return _instance;
            }
        }
    }

    // Function called before the application is quit
    private void OnApplicationQuit()
    {
        _shuttingDown = true;
    }

    // Function called when the MonoBehaviour will be destroyed
    private void OnDestroy()
    {
        _shuttingDown = true;
    }
}
