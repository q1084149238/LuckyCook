using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 对象池
/// </summary>
public class ObjectPool
{
    public static Dictionary<string, List<GameObject>> prefabDic = new Dictionary<string, List<GameObject>>();
    // private static Dictionary<string, Transform> parentDic = new Dictionary<string, Transform>();
    private static Transform bin
    {
        get
        {
            if (!_bin)
            {
                _bin = new GameObject().transform;
                _bin.name = "bin";
            }

            return _bin;
        }
    }
    private static Transform _bin;

    private const string prefabPath = "Prefab/{0:s}";

    private static int prefabID = 0;
    /// <summary>
    /// 从对象池中获取对象
    /// </summary>
    public static GameObject GetPrefab(string key)
    {
        string name = key.Split('/').Last();

        List<GameObject> list = null;
        GameObject go = null;
        // Transform parent = null;
        if (prefabDic.TryGetValue(name, out list))
        {
            if (list.Count == 0)
            {
                go = FYTools.Resource.Instantiate<GameObject>(string.Format(prefabPath, key));
            }
            else
            {
                go = list[0];
                list.RemoveAt(0);
            }
        }
        else
        {
            list = new List<GameObject>();
            prefabDic.Add(name, list);
            go = FYTools.Resource.Instantiate<GameObject>(string.Format(prefabPath, key));
        }

        go.name = name + "-" + prefabID;
        go.SetActive(true);

        prefabID++;

        return go;
    }

    /// <summary>
    /// 回收对象
    /// </summary>
    public static void SetPrefab(GameObject prefab)
    {
        if (prefab == null) return;

        string key = prefab.name.Split('-')[0];

        List<GameObject> list = null;
        if (!prefabDic.TryGetValue(key, out list))
        {
            list = new List<GameObject>();
            prefabDic.Add(key, list);
        }

        prefab.transform.position = Vector3.zero;
        prefab.SetActive(false);
        prefab.transform.SetParent(bin);
        list.Add(prefab);
    }
}
