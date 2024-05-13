using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace FYUI
{
    public class AutoUIScript : EditorWindow
    {

        private static List<GameObject> panelArray = new List<GameObject>();
        private static int arrayCount = 5;
        private static List<UINode> nodeList;

        AutoUIScript()
        {
            titleContent = new GUIContent("UI脚本生成工具");
        }

        [MenuItem("Tool/生成（刷新）界面")]
        public static void Open()
        {
            AutoUIScript window = EditorWindow.GetWindow<AutoUIScript>();
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Space(5);

            for (int i = 0; i < arrayCount; i++)
            {
                panelArray.Add(null);
                panelArray[i] = (GameObject)EditorGUILayout.ObjectField(string.Format("Page{0:d}", i + 1), panelArray[i], typeof(GameObject), true);
            }

            GUILayout.Label("Demo版本功能效果均不完善，请期待后续版本。");

            GUILayout.Space(30);
            if (GUILayout.Button("生成脚本"))
            {
                Build();
            }
        }

        #region  生命周期
        //更新
        void Update()
        {

        }

        void OnFocus()
        {
            // Debug.Log("当窗口获得焦点时调用一次");
        }

        void OnLostFocus()
        {
            // Debug.Log("当窗口丢失焦点时调用一次");
        }

        void OnHierarchyChange()
        {
            // Debug.Log("当Hierarchy视图中的任何对象发生改变时调用一次");
        }

        void OnProjectChange()
        {
            // Debug.Log("当Project视图中的资源发生改变时调用一次");
        }

        void OnInspectorUpdate()
        {
            //Debug.Log("窗口面板的更新");
            //这里开启窗口的重绘，不然窗口信息不会刷新
            this.Repaint();
        }

        void OnSelectionChange()
        {
            //当窗口出去开启状态，并且在Hierarchy视图中选择某游戏对象时调用
            foreach (Transform t in Selection.transforms)
            {
                //有可能是多选，这里开启一个循环打印选中游戏对象的名称
                Debug.Log("OnSelectionChange" + t.name);
            }
        }

        void OnDestroy()
        {
            // Debug.Log("当窗口关闭时调用");
        }

        #endregion

        private static void Refresh()
        {
            for (int i = 0; i < arrayCount; i++)
            {
                panelArray[i] = (GameObject)EditorGUILayout.ObjectField(string.Format("Page{0:d}", i + 1), panelArray[i], typeof(GameObject), true);
            }
        }

        /// <summary>
        /// 脚本生成
        /// </summary>
        public static void Build()
        {
            foreach (GameObject go in panelArray)
            {
                if (!go) continue;

                nodeList = new List<UINode>();

                UINode node = new UINode();
                //选择的物体
                node.tran = go.transform;
                node.path = "";
                FindNode(node);

                string folderPath = string.Format("{0:s}/Script/UI", Application.dataPath);
                string scriptPath = string.Format("{0:s}/Script/UI/{1:s}.cs", Application.dataPath, node.tran.name);
                string classStr = "";

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                //如果已经存在了脚本，则只替换//auto下方的字符串
                if (File.Exists(scriptPath))
                {
                    FileStream classfile = new FileStream(scriptPath, FileMode.Open);
                    StreamReader read = new StreamReader(classfile);
                    classStr = read.ReadToEnd();
                    read.Close();
                    classfile.Close();
                    File.Delete(scriptPath);

                    string splitStr = "//auto";
                    string splitStr2 = "自动生成的代码请勿修改";
                    string unchangeStr = Regex.Split(classStr, splitStr, RegexOptions.IgnoreCase)[0];
                    string changeStr = Regex.Split(UIClass, splitStr, RegexOptions.IgnoreCase)[1];
                    string unchangeStr2 = Regex.Split(classStr, splitStr2, RegexOptions.IgnoreCase)[1];
                    changeStr = Regex.Split(changeStr, splitStr2, RegexOptions.IgnoreCase)[0];

                    StringBuilder build = new StringBuilder();
                    build.Append(unchangeStr);
                    build.Append(splitStr);
                    build.Append(changeStr);
                    build.Append(splitStr2);
                    build.Append(unchangeStr2);
                    classStr = build.ToString();
                }
                else
                {
                    classStr = UIClass;
                }

                string loadedContant = "\t";
                string memberString = "\t";
                foreach (UINode item in nodeList)
                {
                    memberString += string.Format("[HideInInspector]\r\n\tpublic {0:s} {1:s} = null; \r\n\t", item.type, item.tran.name);
                    if (item.type == "Transform")
                    {
                        loadedContant += string.Format("{0:s} = transform.Find(\"{1:s}\"); \r\n\t\t", item.tran.name, item.path, item.type);
                        loadedContant += string.Format("{0:s}.GetComponent<UseNode>().enabled = false; \r\n\t\t", item.tran.name, item.path, item.type);
                    }
                    else
                    {
                        loadedContant += string.Format("{0:s} = transform.Find(\"{1:s}\").GetComponent<{2:s}>(); \r\n\t\t", item.tran.name, item.path, item.type);
                    }
                }

                classStr = classStr.Replace("#类名#", node.tran.name);
                classStr = classStr.Replace("#查找#", loadedContant);
                classStr = classStr.Replace("#成员#", memberString);

                FileStream file = new FileStream(scriptPath, FileMode.CreateNew);
                StreamWriter fileW = new StreamWriter(file, Encoding.UTF8);
                fileW.Write(classStr);
                fileW.Flush();
                fileW.Close();
                file.Close();

                Debug.Log("创建脚本 " + Application.dataPath + "/Scripts/UI/" + node.tran.name + ".cs 成功!");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// 递归查找子节点物体
        /// </summary>
        private static void FindNode(UINode parent)
        {
            foreach (Transform obj in parent.tran)
            {
                UINode node = new UINode();

                //记录节点和路径
                node.tran = obj;
                node.path = parent.path.Equals("") ? obj.name : string.Format("{0:s}/{1:s}", parent.path, obj.name);

                //下划线代表忽略节点
                if (obj.name.Contains("_"))
                {
                    FindNode(node);
                    continue;
                }

                //TODO组件扩充
                if (node.tran.GetComponent<Button>())
                {
                    node.type = "Button";
                }
                else if (node.tran.GetComponent<FYButton>())
                {
                    node.type = "FYButton";
                }
                else if (node.tran.GetComponent<Image>())
                {
                    node.type = "Image";
                }
                else if (node.tran.GetComponent<Text>())
                {
                    node.type = "Text";
                }
                else if (node.tran.GetComponent<UseNode>())
                {
                    node.type = "Transform";
                }
                else
                {
                    FindNode(node);
                    continue;
                }

                //递归查找子节点
                FindNode(node);

                nodeList.Add(node);
            }
        }

        /// <summary>
        /// UI节点
        /// </summary>
        struct UINode
        {
            public Transform tran;
            public string path;

            public string type;
        }

        /// <summary>
        /// 自动生成模板类
        /// </summary>
        private const string UIClass =
    @"
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using FYUI;

public class #类名# : UIBase
{
    //auto
#成员#
    public void Awake()
    {
    #查找#/*自动生成的代码请勿修改*/
    }

    public override void OnShow(System.Object param = null)
    {
        
    }
    public override void OnHide()
    {

    }
}";
    }
}