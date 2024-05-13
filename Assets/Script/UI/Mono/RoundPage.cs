using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundPage : MonoBehaviour
{
    public Text title;
    public Text desc;
    public Text target;
    public Text result;

    public void Show()
    {
        int round = DataManager.playerData.currDay;

        if (round == 1)
        {
            title.text = "季度开始";
            desc.text = "你好，作为饭店的顶级大厨，你需要在5个季度后达成以下营收目标，否则将会被解雇,祝你好运。";
            target.text = "营收目标 0/25 ";
            result.text = "";
        }
        else
        {
            var data = DataManager.playerData;
            title.text = "季度财报";
            desc.text = string.Format("本季度共售出荤菜{1:d}份，素菜{2:d}份，累计营收{3:d}。", data.meatCount + data.vegeCount, data.meatCount, data.vegeCount, data.revenue);
            target.text = string.Format("营收目标 {0:d}/{1:d} ", data.revenue, GameConfig.gameTarget);

            result.text = data.revenue >= GameConfig.gameTarget ? "恭喜您达成本季度营收目标，请再接再厉。" : "很遗憾，你没有达成目标，你已被解雇。";
        }
    }
}
