
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace FYTools
{
    /// <summary>
    /// 资源工具
    /// </summary>
    public class Resource
    {
        private static Dictionary<string, Object> loadedDict = new Dictionary<string, Object>();

        /// <summary>
        /// 加载Resources文件夹下的资源
        /// </summary>
        /// <param name="path">路径</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns>加载的资源</returns>
        public static T Load<T>(string path) where T : Object
        {
            Object obj = null;
            if (!loadedDict.TryGetValue(path, out obj))
            {
                obj = Resources.Load<T>(path);
                loadedDict.Add(path, obj);
            }

            return (T)obj;
        }

        /// <summary>
        /// 加载并实例化Resources文件夹下的资源
        /// </summary>
        /// <param name="path">路径</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns>实例化对象</returns>
        public static T Instantiate<T>(string path) where T : Object
        {
            var obj = Load<T>(path);
            var target = GameObject.Instantiate(obj);

            return target;
        }

        /// <summary>
        /// 贴图加载为精灵
        /// </summary>
        public static Sprite LoadTexture2D(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }

    }
    /// <summary>
    /// 坐标工具
    /// </summary>
    public class Ordinate
    {
        /// <summary>
        /// 世界坐标转UI坐标
        /// </summary>
        public static Vector3 WorldPosToCanvasPos(Vector3 worldPos, RectTransform canvas = null, Camera cam = null)
        {
            Debug.Assert(Camera.main != null, "未找到场景摄像机！！！！");
            if(canvas == null)
            {
                canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
            }

            Vector3 originOff;  // 当前UI系统(0,0)点 相对于屏幕左下角(0, 0)点的偏移量
            originOff = new Vector3(-Screen.width / 2, -Screen.height / 2);
            Vector3 position = Camera.main.WorldToScreenPoint(worldPos) + originOff;
            position.z = 0;
            RectTransform rt = canvas.transform.GetComponent<RectTransform>();
            Vector2 pivot = rt.pivot;
            switch (pivot.x)
            {
                case 1:
                    position.x += rt.sizeDelta.x / 2;
                    break;
                case 0:
                    position.x -= rt.sizeDelta.x / 2;
                    break;
            }
            switch (pivot.y)
            {
                case 1:
                    position.y += rt.sizeDelta.y / 2;
                    break;
                case 0:
                    position.y -= rt.sizeDelta.y / 2;
                    break;
            }
            return position;
        }

        /// <summary>
        /// UI坐标转世界坐标
        /// </summary>
        public static Vector3 UIPosToWordPos(Vector3 uiPos)
        {
            Vector3 ptScreen = RectTransformUtility.WorldToScreenPoint(Camera.main, uiPos);
            ptScreen.z = 0;
            // ptScreen.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
            Vector3 ptWorld = Camera.main.ScreenToWorldPoint(ptScreen);
            return ptWorld;
        }

        /// <summary>
        /// 求垂直向量
        /// 两个向量垂直，点积为0。
        /// 有两个
        /// </summary>
        public static Vector2 GetVerticalVectorFrom(Vector2 from)
        {
            //from.x * -from.y + from.y * from.x
            // return new Vector2(from.y,-from.x);
            return new Vector2(-from.y, from.x);
        }

    }

    /// <summary>
    /// 权重工具
    /// </summary>
    public class weightTool
    {
        /// <summary>
        /// 权重随机
        /// </summary>
        /// <param name="weights">权重数组</param>
        /// <returns>随机得到的索引值</returns>
        public static int WeightRandom(int[] weights)
        {
            List<int> weightArray = new List<int>();
            int w = 0;
            foreach (var weight in weights)
            {
                w += weight;
                weightArray.Add(w);
            }
            int randomNum = Random.Range(0, w + 1);

            int left = 0;
            int right = weights.Length;
            int mid = 0;
            while (left < right)
            {
                mid = (left + right) / 2;
                if (weightArray[mid] > randomNum)
                {
                    right = mid;
                }
                else if (weightArray[mid] < randomNum)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid;
                }
            }

            return left;
        }
    }

    /// <summary>
    /// 存储工具
    /// </summary>
    public class DataSaveHelper
    {
        public static void Save(System.Object saveData, string path)
        {
            string output = JsonUtility.ToJson(saveData);
            File.WriteAllText(path, output);
        }
        public static T Load<T>(string fileName)
        {
            string json = string.Empty;
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    json = sr.ReadToEnd().ToString();
                }
            }
            return JsonUtility.FromJson<T>(json);
        }
    }

    /// <summary>
    /// 贝塞尔工具
    /// </summary> <summary>
    /// 
    /// </summary>
    public class BeizerCurveTool
    {
        /// <summary>
        /// 获取二阶贝塞尔曲线集合
        /// </summary>
        /// <param name="start"></param>
        /// <param name="mid"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static List<Vector3> GetBeizerCurve(Vector3 start, Vector3 mid, Vector3 end)
        {
            List<Vector3> list = new List<Vector3>();

            for (float i = 0.06f; i <= 1; i += 0.06f)
            {
                Vector3 p0p1 = (1 - i) * start + i * mid;
                Vector3 p1p2 = (1 - i) * mid + i * end;
                Vector3 temp = (1 - i) * p0p1 + i * p1p2;

                list.Add(temp);
            }

            return list;
        }
    }

    public class TweenTool
    {
        public static void DoMove(Transform transform, Vector3 pos, float time)
        {
            var timer = default(Timer);
            var times = Mathf.FloorToInt((time / 0.2f) * 10);
            timer = TimerManager.Instance.FrameLoop(() =>
            {
                transform.position = Vector3.Lerp(transform.position, pos, 0.2f);
                times--;
                if (times <= 0)
                {
                    transform.position = pos;
                    TimerManager.Instance.ClearTimer(timer);
                }
            });
        }
    }
}