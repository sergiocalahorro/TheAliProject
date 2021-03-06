using UnityEngine;

// Inherit to create single, global-accessible instance of a class, available at all times
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;

    /// <summary>
    /// Access to Singleton instance
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // Search for existing instance
                _instance = FindObjectOfType<T>();

                // Create new instance if one doesn't already exist
                if (_instance == null)
                {
                    // Need to create a new GameObject to attach the Singleton to
                    GameObject singletonObject = new GameObject();
                    singletonObject.name = typeof(T).ToString();
                    _instance = singletonObject.AddComponent<T>(); 
                }
            }

            return _instance;
        }
    }

    // Awake is called when the script instance is being loaded
    public virtual void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = GetComponent<T>();

        // Make instance persistent
        DontDestroyOnLoad(gameObject);

        if (_instance == null)
        {
            return;
        }
    }
}