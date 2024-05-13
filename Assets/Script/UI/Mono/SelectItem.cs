using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectItem : MonoBehaviour
{
    [HideInInspector]
    public Text nameText;
    [HideInInspector]
    public Text descText;
    [HideInInspector]
    public Image icon;
    public struct SelectData
    {
        public string name;
        public string desc;
    }

    void Awake()
    {
        nameText = transform.Find("_Name").GetComponent<Text>();
        descText = transform.Find("_Desc").GetComponent<Text>();
        icon = transform.Find("_IconBG/_Icon").GetComponent<Image>();
    }

    public void Refresh(SelectData data)
    {
        nameText.text = data.name;
        descText.text = data.desc;
    }
}
