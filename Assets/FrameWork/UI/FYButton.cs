using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FYUI
{
    public class FYButton : Button
    {
        /// <summary>
        /// 按键延迟，避免按钮连点
        /// </summary>
        private const float clickDelay = 0.2f;
        private bool isClicked = false;

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (isClicked) return;
            isClicked = true;
            TimerManager.Instance.Once(DelayReset, clickDelay);

            base.OnPointerClick(eventData);
        }
        private void DelayReset()
        {
            isClicked = false;
        }
    }
}
