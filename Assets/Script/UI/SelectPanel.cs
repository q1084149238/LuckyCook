
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FYUI;
using System.Collections.Generic;
using System.Linq;
using FYTools;

public class SelectPanel : UIBase
{
	//auto
	[HideInInspector]
	public Button Item1 = null;
	[HideInInspector]
	public Button Item2 = null;
	[HideInInspector]
	public Button Item3 = null;
	[HideInInspector]
	public Image SkipBtn = null;

	public void Awake()
	{
		Item1 = transform.Find("_BG/_ItemList/Item1").GetComponent<Button>();
		Item2 = transform.Find("_BG/_ItemList/Item2").GetComponent<Button>();
		Item3 = transform.Find("_BG/_ItemList/Item3").GetComponent<Button>();
		SkipBtn = transform.Find("_BG/_ToolBtn1/SkipBtn").GetComponent<Image>();
		/*自动生成的代码请勿修改*/
		items.Add(Item1.GetComponent<SelectItem>());
		items.Add(Item2.GetComponent<SelectItem>());
		items.Add(Item3.GetComponent<SelectItem>());

		Item1.onClick.AddListener(() => ItemClick(0));
		Item2.onClick.AddListener(() => ItemClick(1));
		Item3.onClick.AddListener(() => ItemClick(2));
	}

	private List<SelectItem> items = new List<SelectItem>();
	private List<GameMain.FoodMat> shopList = new List<GameMain.FoodMat>();
	public override void OnShow(System.Object param = null)
	{
		ShowShop();
	}

	private void ShowShop()
	{
		GetShop();

		for (int i = 0; i < 3; i++)
		{
			var data = shopList[i];
			var item = items[i];
			item.descText.text = GetDesc(data);
			item.nameText.text = data.name;
			item.icon.overrideSprite = Resource.Load<Sprite>("Icon/Mat/" + data.id);
		}
	}

	private void GetShop()
	{
		shopList.Clear();
		var tempList = GameConfig.foodMats.ToArray().ToList();
		for (int i = 0; i < 3; i++)
		{
			var index = Random.Range(0, tempList.Count);
			shopList.Add(tempList[index]);
			tempList.RemoveAt(index);
		}
	}

	private string GetDesc(GameMain.FoodMat mat)
	{
		var desc = "";
		var type = "";
		switch (mat.type)
		{
			case MatType.Vegetable:

				type = "<color=green>蔬菜</color>";
				break;
			case MatType.Egg:
				type = "<color=#AB7733>蛋类</color>";
				break;
			case MatType.Flour:
				type = "<color=orange>面食</color>";
				break;
			case MatType.fruit:
				type = "<color=#DE3C78>水果</color>";
				break;
			case MatType.Suger:
				type = "<color=#3BC4DD>糖</color>";
				break;
			case MatType.Meat:
				type = "<color=#E71C00>肉类</color>";
				break;
		}

		desc = string.Format("{0:s},基础收入{1:d}。", type, mat.baseValue);
		if (mat.foodID.Count > 0)
		{
			foreach (var id in mat.foodID)
			{
				var food = GameConfig.foods.Find(a => { return a.id == id; });
				GameMain.FoodMat mat2 = new GameMain.FoodMat();
				foreach (var tagID in food.tag)
				{
					if (tagID == mat.id) continue;
					mat2 = GameConfig.foodMats.Find(a => { return a.id == tagID; });
				}
				if (string.IsNullOrEmpty(mat2.name)) mat2 = mat;
				desc += string.Format("与{0:s}合成{1:s}。", mat2.name, food.name);
			}
		}

		return desc;
	}
	public override void OnHide()
	{

	}

	private void ItemClick(int index)
	{
		var data = shopList[index];

		GameMain.GetMat(data);

		UIManager.Instance.ClosePanel(PanelType.Select);
	}
}