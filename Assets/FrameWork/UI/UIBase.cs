using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYUI
{
    public abstract class UIBase : MonoBehaviour
    {
        //TODOUI基类
        public abstract void OnShow(System.Object param = null);
        public abstract void OnHide();

        // private void OnEnable()
        // {
        //     OnShow();
        // }

        private void OnDisable()
        {
            OnHide();
        }
    }
}
