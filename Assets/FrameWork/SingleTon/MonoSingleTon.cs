using UnityEngine;

public class MonoSingleTon<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _Instance = default(T);
    public static T Instance
    {
        get
        {
            if (_Instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = typeof(T).ToString();
                _Instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);
            }
            return _Instance;
        }
    }
}