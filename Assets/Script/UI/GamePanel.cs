
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using FYUI;

public class GamePanel : UIBase
{
	//auto
	[HideInInspector]
	public Image toolBtn1 = null;
	[HideInInspector]
	public Button continueBtn = null;
	[HideInInspector]
	public Image toolBtn2 = null;
	[HideInInspector]
	public Text coinText = null;
	[HideInInspector]
	public Button SkipBtn = null;
	[HideInInspector]
	public Image RoundPage = null;
	[HideInInspector]
	public Text roundText = null;
	[HideInInspector]
	public Text roundTarget = null;

	public void Awake()
	{
		toolBtn1 = transform.Find("_Bottom/_ToolBtn1/toolBtn1").GetComponent<Image>();
		continueBtn = transform.Find("_Bottom/_ToolBtn2/continueBtn").GetComponent<Button>();
		toolBtn2 = transform.Find("_Bottom/_ToolBtn3/toolBtn2").GetComponent<Image>();
		coinText = transform.Find("_Coin/coinText").GetComponent<Text>();
		SkipBtn = transform.Find("RoundPage/_BG/_ToolBtn1/SkipBtn").GetComponent<Button>();
		RoundPage = transform.Find("RoundPage").GetComponent<Image>();
		roundText = transform.Find("_roundText/roundText").GetComponent<Text>();
		roundTarget = transform.Find("_roundText/roundTarget").GetComponent<Text>();
		/*自动生成的代码请勿修改*/

		SkipBtn.onClick.AddListener(comfirmBtnClick);
		DataManager.Register(RefreshCoin);
		GameMain.Register(DayEnd);

		roundPage = RoundPage.GetComponent<RoundPage>();
	}

	private RoundPage roundPage;
	private gameCallback callback;

	public override void OnShow(System.Object param = null)
	{
		callback = (gameCallback)param;
		RefreshCoin(0);
	}
	private void DayEnd()
	{
		roundText.text = string.Format("第{0:d}/5轮", DataManager.playerData.currDay);
		roundTarget.text = string.Format("目标：{0:d}", GameConfig.gameTarget);

		if (DataManager.playerData.currDay > 5 || DataManager.playerData.currDay == 1)
		{
			RoundPage.gameObject.SetActive(true);
			roundPage.Show();
			
			DataManager.playerData.currRound++;
			roundText.text = string.Format("第1/5轮");
			roundTarget.text = string.Format("目标：{0:d}", GameConfig.gameTarget);

			return;
		}
	}

	private void comfirmBtnClick()
	{
		RoundPage.gameObject.SetActive(false);

		callback?.Invoke();
	}

	private void RefreshCoin(int value)
	{
		coinText.text = DataManager.playerData.coin.ToString();
	}
	public override void OnHide()
	{
		DataManager.Remove(RefreshCoin);
		GameMain.Remove(DayEnd);
	}
}