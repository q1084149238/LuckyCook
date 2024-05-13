using System.Collections.Generic;
using static GameMain;

public class Combine
{
    public static Food CombineCheck(List<MatItem> foodMats, out MatItem combineMat)
    {
        var combineList = GameConfig.combineDic[foodMats[0].data.type];
        var baseID = foodMats[0].data.id;
        Food food = new Food();
        combineMat = foodMats[0];

        var maxValue = 0;
        foreach (var combineFood in combineList)
        {
            if (!combineFood.tag.Contains(baseID)) continue;

            for (int i = 1; i < foodMats.Count; i++)
            {
                if (combineFood.tag.Contains(foodMats[i].data.id))
                {
                    if (maxValue < combineFood.baseValue)
                    {
                        //优先合成最大价值菜品
                        maxValue = combineFood.baseValue;
                        food = combineFood;
                        combineMat = foodMats[i];
                    }
                }
            }
        }

        if (string.IsNullOrEmpty(food.name))
        {
            food = GameConfig.foods.Find(a => { return a.tag[0] == baseID; });
        }

        //没有可以合成的菜品，返回1材料的菜品。
        if (food.foodType == FoodType.Vege) DataManager.playerData.vegeCount++;
        if (food.foodType == FoodType.Meat) DataManager.playerData.meatCount++;
        return food;
    }
}