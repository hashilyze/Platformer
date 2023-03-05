using UnityEngine;

namespace Utility.Singleton
{
    /// <summary>
    /// Singleton pattern for Component
    /// Guarantee there is only one component in the scene
    /// And the GameObject attached it 'don't be destoryed on load'
    /// </summary>
    public abstract class SingletonComponent<T> : MonoBehaviour where T : SingletonComponent<T>
    {
        private static T _instance;
        private static bool _isShutDown = false;
        private static readonly object _lock = new object();

        public static T Instance
        {
            get
            {
                if (_isShutDown)
                {
                    return null;
                }

                EnsureInstance(_instance);

                return _instance;
            }
        }

        private static void EnsureInstance (T instance)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    // Set given instance to singleton
                    _instance = instance;

                    if (_instance == null)
                    {
                        // Search Instance
                        _instance = FindObjectOfType<T>();
                    }

                    if (_instance == null)
                    {
                        // Create Instance
                        GameObject singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                    }

                    _instance.InitializeInstance();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                else
                {
                    // Ensure only one instance in scene
                    if (_instance != instance)
                    {
                        Destroy(instance);
                    }
                }
            }
        }


        protected virtual void Awake ()
        {
            EnsureInstance((T)this);
        }

        protected virtual void OnApplicationQuit ()
        {
            _isShutDown = true;
        }
        protected virtual void OnDestroy ()
        {
            _isShutDown = true;
        }


        protected abstract void InitializeInstance ();
    }
}