
using System.Collections.Generic;
using FYUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject Tip;
    public Text tipText;
    public static UIManager Instance;
    public List<UIBase> list = new List<UIBase>();

    public GameObject mask;

    public void Awake()
    {
        Instance = this;
    }

    public void OpenPanel(PanelType type, System.Object param = null)
    {
        list[(int)type].gameObject.SetActive(true);
        list[(int)type].OnShow(param);
    }

    public void ClosePanel(PanelType type)
    {
        list[(int)type].gameObject.SetActive(false);
        list[(int)type].OnHide();
    }

    private Timer tipTimer;
    public void ShowTip(string str, float time = 0.6f)
    {
        if (tipTimer != null)
        {
            Tip.SetActive(false);
            TimerManager.Instance.ClearTimer(tipTimer);
        }

        tipText.text = str;
        Tip.SetActive(true);
        tipTimer = TimerManager.Instance.Once(() =>
        {
            tipTimer = null;
            Tip.SetActive(false);
        }, time);
    }

    public void ShowMask(float time)
    {
        mask.SetActive(true);
        TimerManager.Instance.Once(() =>
        {
            mask.SetActive(false);
        }, time);
    }
}

public enum PanelType
{
    Select = 0,
    Game = 1,
    End,
    Fail
}