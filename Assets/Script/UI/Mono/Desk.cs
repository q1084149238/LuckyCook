using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Desk : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> desks;
    private GameObject miniDesk;
    private void Start()
    {
        miniDesk = gameObject.transform.GetChild(0).gameObject;
        var posList = GetCirclePos(14, 415, new Vector3(0, 0));
        for (int i = 0; i < posList.Count; i++)
        {
            var pos = posList[i];
            var go = Instantiate(miniDesk);
            go.transform.SetParent(transform);
            go.transform.localPosition = pos;

            desks.Add(go);
        }

        var posList2 = GetCirclePos(14, 415, new Vector3(0, 0), 90);
        for (int i = 0; i < posList2.Count; i++)
        {
            var pos = posList2[i];
            var go = Instantiate(miniDesk);
            go.transform.SetParent(transform);
            go.transform.localPosition = pos;
            go.transform.GetChild(0).GetComponent<Image>().color = Color.white;
            go.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.white;

            desks.Add(go);
        }

        miniDesk.gameObject.SetActive(false);

        // TimerManager.Instance.FrameLoop(Rotate);
    }

    public List<Vector3> GetCirclePos(int count, int radius, Vector3 origin, float startAngle = 0)
    {
        var posList = new List<Vector3>();
        for (int i = 0; i < count; i++)
        {
            // 计算旋转角度
            float angle = 360 * 1.0f / count * i + startAngle;
            //x = centerX + radius * cos(angle * 3.14 / 180)
            //y = centerY + radius * sin(angle * 3.14 / 180)
            var pos = new Vector3(radius * Mathf.Cos(angle * Mathf.PI / 180), radius * Mathf.Sin(angle * Mathf.PI / 180), 0);
            posList.Add(origin + pos);
        }

        return posList;
    }
}
