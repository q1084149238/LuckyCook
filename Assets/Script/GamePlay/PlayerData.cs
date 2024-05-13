using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[SerializeField]
public class PlayerData
{
    public int coin;
    public int revenue;
    public int currDay = 1;
    public int currRound = 1;

    public int meatCount;
    public int vegeCount;

    [SerializeField]
    public List<int> foodMats = new List<int>();
}

public class DataManager
{
    /// <summary>
    /// 玩家金币变化回调
    /// </summary>
    private static Action<int> OnCoinChange;
    public static PlayerData _playerData;
    public static PlayerData playerData
    {
        get
        {
            if (_playerData == null)
            {
                _playerData = new PlayerData();
            }

            return _playerData;
        }
    }

    public static void GetCoin(int value)
    {
        playerData.coin += value;
        if (value > 0) playerData.revenue += value;
        playerData.coin = Mathf.Clamp(playerData.coin, 0, 9999999);

        OnCoinChange?.Invoke(value);
    }

    /// <summary>
    /// 添加食材
    /// </summary>
    public static void GetMat(int id)
    {
        playerData.foodMats.Add(id);
    }
    /// <summary>
    /// 注册金币回调
    /// </summary>
    public static void Register(Action<int> coinCallback)
    {
        OnCoinChange += coinCallback;
    }
    public static void Remove(Action<int> coinCallback)
    {
        OnCoinChange -= coinCallback;
    }
}
