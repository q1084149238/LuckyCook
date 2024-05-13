using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using FYTools;
using WeChatWASM;
using System.Linq;
using DG.Tweening;
using System;

public delegate void gameCallback();
public class GameMain : MonoBehaviour
{
    public GameObject startNode;
    /// <summary>
    /// 棋盘
    /// </summary>
    public List<GameObject> matFrames = new List<GameObject>();
    /// <summary>
    /// 入场的食材
    /// </summary>
    private List<MatItem> mats = new List<MatItem>();
    /// <summary>
    /// 食材列表
    /// </summary>
    private static List<FoodMat> playerMats = new List<FoodMat>();

    /// <summary>
    /// 游戏状态，0游戏中，1游戏结束
    /// </summary>
    private int gameState = 1;

    public static Action onRoundEnd;
    private const string MATPATH = "Mat";
    private const string FOODPATH = "Icon/Food/";
    private const string MATICONPATH = "Icon/Mat/";
    private const string COIN = "Coin";
    private const int FRAMECOUNT = 14;

    private List<int> ints = new List<int>();
    private List<int> tempInts = new List<int>();
    void Start()
    {
        for (int i = 0; i < FRAMECOUNT; i++)
        {
            var index = i;
            ints.Add(index);
        }
        for (int i = 0; i < FRAMECOUNT; i++)
        {
            var index = i;
            mats.Add(new MatItem() { index = index, isEmpty = true });
        }

        gameCallback callback = GameStart;
        UIManager.Instance.OpenPanel(PanelType.Game, callback);

        onRoundEnd?.Invoke();
    }

    public void GameStart()
    {
        //重置轮次
        DataManager.playerData.currDay = 1;

        if (gameState == 0)
        {
            //已在游戏中，说明进入下一季度
            RoundStart();
            return;
        }

        gameState = 0;
        //游戏开局，玩家随机获得食材。
        var tempList = GameConfig.foodMats.ToArray().ToList<FoodMat>();
        for (int i = 0; i < 3; i++)
        {
            var index = UnityEngine.Random.Range(0, tempList.Count);
            GetMat(tempList[index]);
            tempList.RemoveAt(index);
        }
        //重置轮次
        DataManager.playerData.currDay = 1;
        RoundStart();
    }

    /// <summary>
    /// 获得食材
    /// </summary>
    public static void GetMat(FoodMat mat)
    {
        playerMats.Add(mat);
        DataManager.GetMat(mat.id);
    }

    /// <summary>
    /// 注册回合事件
    /// </summary>
    public static void Register(Action action)
    {
        onRoundEnd += action;
    }
    public static void Remove(Action action)
    {
        onRoundEnd -= action;
    }

    #region 结算流程
    /// <summary>
    /// 回合开始
    /// </summary>
    public void RoundStart()
    {
        MatRandom();
        //2秒后结算
        StartCoroutine(FoodCook());
    }

    public Transform coinNode;
    /// <summary>
    /// 材料随机入场
    /// </summary>
    private void MatRandom()
    {
        for (int i = 0; i < mats.Count; i++)
        {
            mats[i].Clear();
        }

        tempInts = ints.ToArray().ToList();
        if (playerMats.Count > FRAMECOUNT)
        {
            playerMats.Sort((a, b) => { return UnityEngine.Random.Range(0, 10 > 5 ? 1 : -1); });
            //食材数量超过棋盘数量，随机分配。
            for (int i = 0; i < FRAMECOUNT; i++)
            {
                CreateMat(i);
            }
        }
        else
        {
            for (int i = 0; i < playerMats.Count; i++)
            {
                CreateMat(i);
            }
        }
    }
    private void CreateMat(int i)
    {
        var deskIndex = UnityEngine.Random.Range(0, tempInts.Count);
        var matItem = mats[tempInts[deskIndex]];
        matItem.data = playerMats[i];

        for (int j = 0; j < matItem.data.count; j++)
        {
            var go = ObjectPool.GetPrefab(MATPATH);
            var image = go.transform.GetChild(0).GetComponent<Image>();
            var name = go.transform.GetChild(1).GetComponent<Text>();
            matItem.go.Add(go);
            matItem.image.Add(image);
            matItem.name.Add(name);
            matItem.animation.Add(go.GetComponent<Animation>());

            //物品移动
            go.transform.SetParent(transform, false);
            go.transform.localPosition = startNode.transform.localPosition;
            go.transform.SetParent(matFrames[tempInts[deskIndex]].transform, true);
            go.transform.localScale = Vector3.one;
            var end = matFrames[tempInts[deskIndex]].transform.position;
            end.y += 24 + (j * 12);
            end.z = 0;
            MoveAction(go, end);

            image.overrideSprite = Resource.Load<Sprite>(MATICONPATH + playerMats[i].id);
            name.text = playerMats[i].name;
            matItem.isEmpty = false;
        }

        tempInts.RemoveAt(deskIndex);
    }
    private void MoveAction(GameObject go, Vector3 end)
    {
        go.transform.DOLocalMove(end, 0.7f);
    }

    private List<MatItem> matsList = new List<MatItem>();
    /// <summary>
    /// 剩余的食材
    /// </summary>
    private List<MatItem> leftMats = new List<MatItem>();
    /// <summary>
    /// 合成
    /// </summary>
    /// <returns></returns>
    private IEnumerator FoodCook()
    {
        yield return new WaitForSeconds(1);

        var seconds = playerMats.Count > 8 ? new Vector3(0.06f, 0.12f, 0) : new Vector3(0.12f, 0.24f, 0.12f);
        for (int i = 0; i < mats.Count; i++)
        {
            if (mats[i].isEmpty || (mats[i].isCombine && mats[i].go.Count == 1)) continue;
            var contactList = GameConfig.FieldContact[mats[i].index];
            foreach (var index in contactList)
            {
                if (!mats[index].isEmpty)
                {
                    matsList.Add(mats[index]);
                }
            }

            MatItem matItem;
            var food = Combine.CombineCheck(matsList, out matItem);
            if (food.id == mats[i].data.id)
            {
                //没有合成，遍历结束后统一处理
                if (!leftMats.Contains(matItem)) leftMats.Add(matItem);
            }
            else
            {
                //能够合成，合并，合成。
                combineAni(matItem, mats[i], seconds);
                yield return new WaitForSeconds(seconds.x);
                ShowFood(food, mats[i]);
                yield return new WaitForSeconds(seconds.y);
                ShowCoin(food, mats[i].go.Last().transform);
            }

            yield return new WaitForSeconds(seconds.z);
            matsList.Clear();
        }

        yield return new WaitForSeconds(seconds.z);
        foreach (var mat in leftMats)
        {
            if (mat.isEmpty || mat.isCombine) continue;
            var food = GameConfig.foods.Find(a => { return a.id == mat.data.id; });
            foreach (var animation in mat.animation)
            {
                animation.Play();
            }

            yield return new WaitForSeconds(seconds.x);
            ShowFood(food, mat);
            yield return new WaitForSeconds(seconds.y);
            ShowCoin(food, mat.go.Last().transform);
            yield return new WaitForSeconds(seconds.z);
        }
        //合成结束
        CombineEnd();
    }
    /// <summary>
    /// 合成动效
    /// </summary>
    private void combineAni(MatItem start, MatItem end, Vector3 seconds)
    {
        //移动最上层食材
        var go = start.isCombine ? start.go[0] : start.go.Last();
        var endGo = end.go[end.go.Count - 1];

        go.transform.SetParent(endGo.transform.parent, true);
        // go.transform.DOScaleY(0, seconds.x / 2).OnComplete(() =>
        // {
        //     go.transform.DOScaleY(1, seconds.x / 2);
        // });

        go.transform.DOLocalMove(endGo.transform.position, seconds.x).OnComplete(() =>
        {
            end.animation.Last().Play();

            TimerManager.Instance.Once(() =>
            {
                ObjectPool.SetPrefab(go);
                // ObjectPool.SetPrefab(endGo);
                start.go.Remove(go);
                // end.go.Remove(endGo);
                end.isCombine = true;
                //食材用尽，移除
                if (start.go.Count == 0) start.Clear();
                // if (end.go.Count == 0) end.Clear();
            }, seconds.y);
        });
    }
    /// <summary>
    /// 显示食物信息
    /// </summary>
    /// <param name="food">
    private void ShowFood(Food food, MatItem item)
    {
        foreach (var image in item.image)
        {
            image.overrideSprite = Resource.Load<Sprite>(FOODPATH + food.id);
        }
        foreach (var name in item.name)
        {
            name.text = food.name;
        }
    }
    private void CombineEnd()
    {
        // int coin = 0;
        for (int i = 0; i < coins.Count; i++)
        {
            var item = coins[i];
            // coin += item.baseValue;
            item.Show(-1);
            item.transform.SetParent(coinNode, true);
            TweenCallback onComplete = () =>
            {
                ObjectPool.SetPrefab(item.gameObject);
                //动画结束，获得金币。
                DataManager.GetCoin(item.baseValue);
            };
            if (i == coins.Count - 1) onComplete += () => TimerManager.Instance.Once(CurrDayEnd, 1);

            item.transform.DOLocalMove(Vector3.zero, (i + 14) * 0.06f).OnComplete(onComplete);
        }

        coins.Clear();
    }
    /// <summary>
    /// 当前轮次结束
    /// </summary>
    private void CurrDayEnd()
    {
        var value = DataManager.playerData.currDay + 1;
        DataManager.playerData.currDay = value >= 6 ? 6 : value;

        if (DataManager.playerData.currDay < 6) UIManager.Instance.OpenPanel(PanelType.Select);

        //轮次全部完成，回合结束。
        onRoundEnd?.Invoke();
    }
    private List<Coin> coins = new List<Coin>();
    /// <summary>
    /// 金币结算
    /// </summary>
    private void ShowCoin(Food food, Transform parent)
    {
        var coin = ObjectPool.GetPrefab(COIN);
        coin.transform.SetParent(parent);
        coin.transform.localScale = Vector3.one;
        coin.transform.localPosition = Vector3.zero;
        //TODO获得金币
        var script = coin.GetComponent<Coin>();
        script.Show(food.baseValue);
        coins.Add(script);
    }

    #endregion

    #region 数据结构
    /// <summary>
    /// 食材
    /// </summary>
    public struct FoodMat
    {
        public int id;
        public List<int> foodID;
        public int count;
        public string name;
        public string cookName;
        public MatType type;
        public int baseValue;
    }
    /// <summary>
    /// 菜品
    /// </summary>
    public struct Food
    {
        /// <summary>
        /// 合成标签
        /// </summary>
        public List<int> tag;
        public int id;
        public string name;
        public TasteType tasteType;
        public FoodType foodType;
        public int baseValue;
    }
    /// <summary>
    /// 棋子
    /// </summary>
    public class MatItem
    {
        public bool isCombine = false;
        public bool isEmpty;
        public List<GameObject> go = new List<GameObject>();
        public List<Animation> animation = new List<Animation>();
        public int index;
        public List<Image> image = new List<Image>();
        public List<Text> name = new List<Text>();
        public FoodMat data;

        public void Clear()
        {
            int count = go.Count;

            for (int i = 0; i < count; i++)
            {
                ObjectPool.SetPrefab(go[i]);
            }

            go.Clear();
            // index = -1;
            name.Clear();
            image.Clear();
            animation.Clear();
            data = default;
            isEmpty = true;
            isCombine = false;
        }
    }
    #endregion
}

public enum MatType
{
    Vegetable = 0,
    Meat = 1,
    Flour = 2,
    fruit = 3,
    Egg = 4,
    Suger = 5,
}

/// <summary>
/// 味道分类
/// </summary>
public enum TasteType
{
    //酸
    Sour = 0,
    //甜
    sweet = 1,
    //苦
    bitter = 2,
    //辣
    spicy = 3,
    //咸
    salty = 4
}
/// <summary>
/// 菜品分类
/// </summary>
public enum FoodType
{
    Vege = 0,
    Meat = 1,
}
