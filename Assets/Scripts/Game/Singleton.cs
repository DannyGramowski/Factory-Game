using UnityEngine;
namespace Factory.Core {

    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
        private static T instance;
        public static T Instance {
            get {
                if (instance == null) {

                    T[] objs = FindObjectsOfType(typeof(T), true) as T[];
                    if (objs.Length > 0)
                        instance = objs[0];
                    if (objs.Length > 1) {
                        foreach (T t in objs) {
                            Debug.Log("object of " + t);
                        }
                        throw new System.Exception("There is more than one " + typeof(T).Name + " in the scene.");
                    }
                    if (instance == null) {
                        GameObject obj = new GameObject {
                            hideFlags = HideFlags.HideAndDontSave
                        };
                        instance = obj.AddComponent<T>();
                    }
                }
                return instance;
            }
        }
    }

    public class SingletonPersistent<T> : MonoBehaviour where T : MonoBehaviour {
        public static T Instance { get; private set; }

        public virtual void Awake() {
            if (Instance == null) {
                Instance = this as T;
                DontDestroyOnLoad(this);
            } else {
                Destroy(gameObject);
            }
        }
    }
}