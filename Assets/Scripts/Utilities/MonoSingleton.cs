using UnityEngine;

namespace YesChef.Utilities
{
    /// <summary>
    /// Shared singleton base for scene-level services.
    /// Centralizing the duplicate protection keeps bootstrap code small
    /// and makes singleton behaviour consistent across manager classes.
    /// </summary>
    /// <typeparam name="T">Concrete MonoBehaviour singleton type.</typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<T>();
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this as T;
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
