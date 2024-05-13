using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static GameMain;

public class GameConfig
{
    public static List<FoodMat> foodMats = new List<FoodMat>()
    {
        new(){id = 0 , name = "果酱" , cookName = "果酱" , type = MatType.fruit , baseValue = 2,foodID = new List<int>(){101},count = 4},
        new(){id = 1 , name = "白菜" , cookName = "白菜卷" , type = MatType.Vegetable , baseValue = 2,foodID = new List<int>(){},count = 2},
        new(){id = 2 , name = "猪肉" , cookName = "炸猪排" ,type = MatType.Meat , baseValue = 4,foodID = new List<int>(){104,100},count = 1},
        new(){id = 3 , name = "青苹果" , cookName = "红苹果" ,type = MatType.fruit , baseValue = 2,foodID = new List<int>(){102},count = 2},
        new(){id = 4 , name = "面包" , cookName = "烤面包" ,type = MatType.Flour , baseValue = 2,foodID = new List<int>(){101},count = 2},
        new(){id = 5 , name = "鸡蛋" , cookName = "荷包蛋" ,type = MatType.Egg , baseValue = 2,foodID = new List<int>(){103},count = 1},
    };
    public static List<Food> foods = new List<Food>()
    {
        new(){id = 0,name = "果酱",tag = new List<int>(){0},foodType = FoodType.Vege,tasteType = TasteType.sweet,baseValue = 2},
        new(){id = 1,name = "生菜沙拉",tag = new List<int>(){1},foodType = FoodType.Vege,tasteType = TasteType.sweet,baseValue = 2},
        new(){id = 2,name = "炸猪排",tag = new List<int>(){2},foodType = FoodType.Meat,tasteType = TasteType.salty,baseValue = 4},
        new(){id = 3,name = "红苹果",tag = new List<int>(){3},foodType = FoodType.Vege,tasteType = TasteType.sweet,baseValue = 2},
        new(){id = 4,name = "烤面包",tag = new List<int>(){4},foodType = FoodType.Vege,tasteType = TasteType.sweet,baseValue = 2},
        new(){id = 5,name = "荷包蛋",tag = new List<int>(){5},foodType = FoodType.Vege,tasteType = TasteType.salty,baseValue = 2},
        new(){id = 100,name = "饺子",tag = new List<int>(){1,2},foodType = FoodType.Meat,tasteType = TasteType.salty,baseValue = 6} ,
        new(){id = 101,name = "果酱面包",tag = new List<int>(){0,4},foodType = FoodType.Vege,tasteType = TasteType.sweet,baseValue = 4} ,
        new(){id = 102,name = "苹果派",tag = new List<int>(){3,4},foodType = FoodType.Vege,tasteType = TasteType.sweet,baseValue = 4} ,
        new(){id = 103,name = "蛋挞",tag = new List<int>(){5,4},foodType = FoodType.Vege,tasteType = TasteType.sweet,baseValue = 4} ,
        new(){id = 104,name = "培根",tag = new List<int>(){2,2},foodType = FoodType.Vege,tasteType = TasteType.salty,baseValue = 8} ,
    };

    public static Dictionary<MatType, List<Food>> combineDic = new()
    {
        {MatType.Vegetable,new List<Food>()
        {
            new(){id = 100,name = "饺子",tag = new List<int>(){1,2},foodType = FoodType.Meat,tasteType = TasteType.salty,baseValue = 6} ,
        }},
        {MatType.fruit,new List<Food>()
        {
            new(){id = 101,name = "果酱面包",tag = new List<int>(){0,4},foodType = FoodType.Vege,tasteType = TasteType.sweet,baseValue = 4} ,
            new(){id = 102,name = "苹果派",tag = new List<int>(){3,4},foodType = FoodType.Vege,tasteType = TasteType.sweet,baseValue = 4} ,
        }},
        {MatType.Egg,new List<Food>()
        {
            new(){id = 103,name = "蛋挞",tag = new List<int>(){5,4},foodType = FoodType.Vege,tasteType = TasteType.sweet,baseValue = 4} ,
        }},
        {MatType.Meat,new List<Food>()
        {
            new(){id = 104,name = "培根",tag = new List<int>(){2,2},foodType = FoodType.Vege,tasteType = TasteType.salty,baseValue = 8} ,
        }},
        {MatType.Flour,new List<Food>()
        {

        }},
        {MatType.Suger,new List<Food>()
        {

        }},
    };

    // 0 1 2 3 4
    //  5 6 7 8
    // 9 a b z d
    /// <summary>
    /// 棋盘关联
    /// </summary>
    public static int[][] FieldContact =
    {
        new int[]{0,1,5},
        new int[]{1,0,2,5,6},
        new int[]{2,1,3,6,7},
        new int[]{3,2,4,7,8},
        new int[]{4,3,8},
        new int[]{5,0,1,6,9,10},
        new int[]{6,1,2,5,7,10,11},
        new int[]{7,2,3,6,8,11,12},
        new int[]{8,3,4,7,12,13},
        new int[]{9,5,10},
        new int[]{10,5,6,9,11},
        new int[]{11,6,7,10,12},
        new int[]{12,7,8,11,13},
        new int[]{13,8,12},
    };

    public static int gameTarget
    {
        get
        {
            return ((DataManager.playerData.currRound - 1) * 20) + 25;
        }
    }
}