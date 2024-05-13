using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public delegate void onKeyDownAction(List<List<KeyCode>> keys);
public delegate void onKeyUpAction(KeyCode key);
public delegate void onTouchAction(Vector2 pos);
public class InputManager : MonoSingleTon<InputManager>
{
    /// <summary>
    /// 输入委托
    /// </summary>
    private onKeyUpAction actions;

    private bool isPause = false;

    // private Dictionary<string, List<KeyCode>> downKeys = new Dictionary<string, List<KeyCode>>();
    private Dictionary<string, onKeyDownAction> downActions = new Dictionary<string, onKeyDownAction>();
    private Dictionary<string, onKeyUpAction> upActions = new Dictionary<string, onKeyUpAction>();

    private Dictionary<string, onTouchAction> touchActions = new Dictionary<string, onTouchAction>();

    private List<List<KeyCode>> tempKeys = new List<List<KeyCode>>() { new List<KeyCode>(), new List<KeyCode>(), new List<KeyCode>(), new List<KeyCode>() };
    private bool emptyInvoke = false;
    private bool isDrag = false;
    private void Update()
    {
        if (isPause)
        {
            return;
        }

        //键盘按键按键
        //TODO按键整理
        for (int i = 0; i < 26; i++)
        {
            KeyCode key = i + KeyCode.A;
            if (Input.GetKey(key))
            {
                tempKeys[0].Add(key);
            }
        }
        //空格
        if (Input.GetKeyDown(KeyCode.Space))
        {
            tempKeys[1].Add(KeyCode.Space);
        }
        //方向键
        for (int i = 0; i < 4; i++)
        {
            KeyCode key = i + KeyCode.UpArrow;
            if (Input.GetKey(key))
            {
                tempKeys[2].Add(key);
            }
        }
        if (tempKeys[0].Count > 0 || tempKeys[1].Count > 0 || tempKeys[2].Count > 0)
        {
            OnkeyDown(InputType.Keyboard, tempKeys);
            foreach (var keys in tempKeys)
            {
                keys.Clear();
            }
            emptyInvoke = false;
        }
        else if (!emptyInvoke)
        {
            //TODO待定
            //所有按键抬起时触发一次空按键输入。
            emptyInvoke = true;
            OnkeyDown(InputType.Keyboard, tempKeys);
        }

        #region 触控
        if (isPause && isDrag)
        {
            isDrag = false;
            OnEndDrag(Input.mousePosition);
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            isDrag = true;
            OnBeginDrag(Input.mousePosition);
        }

        if (isDrag)
        {
            OnDrag(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDrag = false;
            OnEndDrag(Input.mousePosition);
        }

        #endregion
    }

    private void LateUpdate()
    {
        for (int i = 0; i < 26; i++)
        {
            KeyCode key = i + KeyCode.A;
            if (Input.GetKeyUp(key))
            {
                // downKeys[InputType.Keyboard].Remove(key);
            }
        }

        for (int i = 0; i < 4; i++)
        {
            KeyCode key = i + KeyCode.UpArrow;
            if (Input.GetKeyUp(key))
            {
                OnKeyUp(InputType.Move, key);
            }
        }
    }

    private void OnKeyUp(string tag, KeyCode key)
    {
        if (!upActions.ContainsKey(tag))
        {
            return;
        }
        upActions[tag]?.Invoke(key);
    }
    private void OnkeyDown(string tag, List<List<KeyCode>> keys)
    {
        if (!downActions.ContainsKey(tag))
        {
            return;
        }
        downActions[tag]?.Invoke(keys);
    }

    public void OnBeginDrag(Vector2 pos)
    {
        TouchEvent(InputType.OnTouchStart, pos);
    }

    public void OnDrag(Vector2 pos)
    {
        TouchEvent(InputType.OnTouchMove, pos);
    }

    public void OnEndDrag(Vector2 pos)
    {
        TouchEvent(InputType.OnTouchEnd, pos);
    }

    private void TouchEvent(string tag, Vector2 pos)
    {
        onTouchAction tempAcions = null;
        switch (tag)
        {
            case InputType.OnTouchMove:
                if (touchActions.TryGetValue(InputType.OnTouchMove, out tempAcions))
                    tempAcions?.Invoke(pos);
                break;
            case InputType.OnTouchStart:
                if (touchActions.TryGetValue(InputType.OnTouchStart, out tempAcions))
                    tempAcions?.Invoke(pos);
                break;
            case InputType.OnTouchEnd:
                if (touchActions.TryGetValue(InputType.OnTouchEnd, out tempAcions))
                    tempAcions?.Invoke(pos);
                break;
        }
    }


    /*TODO增加多种输入监听类型，每次只暂停需要停止的部分，
    不同情景下的输入分别处理*/
    public void Pause()
    {
        isPause = true;
    }
    public void Resume()
    {
        isPause = false;
    }

    /// <summary>
    /// 注册按键点击事件
    /// </summary>
    /// <param name="tag">按键标签</param>
    /// <param name="action">注册事件</param>
    public void Register(string tag, onKeyDownAction action)
    {
        if (!downActions.ContainsKey(tag))
        {
            downActions.Add(tag, action);
        }
        else
        {
            downActions[tag] += action;
        }
    }
    /// <summary>
    /// 注册按键抬起事件
    /// </summary>
    /// <param name="tag">按键标签</param>
    /// <param name="action">注册事件</param>
    public void Register(string tag, onKeyUpAction action)
    {
        if (!upActions.ContainsKey(tag))
        {
            upActions.Add(tag, action);
        }
        else
        {
            upActions[tag] += action;
        }
    }
    /// <summary>
    /// 注册触摸事件
    /// </summary>
    public void Register(string tag, onTouchAction action)
    {
        onTouchAction tempAcions = null;
        switch (tag)
        {
            case InputType.OnTouchMove:
                if (!touchActions.TryGetValue(InputType.OnTouchMove, out tempAcions))
                    touchActions.Add(InputType.OnTouchMove, action);
                break;
            case InputType.OnTouchStart:
                if (!touchActions.TryGetValue(InputType.OnTouchStart, out tempAcions))
                    touchActions.Add(InputType.OnTouchStart, action);
                break;
            case InputType.OnTouchEnd:
                if (!touchActions.TryGetValue(InputType.OnTouchEnd, out tempAcions))
                    touchActions.Add(InputType.OnTouchEnd, action);
                break;
        }
        tempAcions += action;
    }
    /// <summary>
    /// 移除按键点击监听
    /// </summary>
    public void Remove(string tag, onKeyDownAction action)
    {
        Debug.Assert(downActions.ContainsKey(tag), string.Format("{0:s}按键事件未注册，无法删除！", tag));
        downActions[tag] -= action;
    }
    /// <summary>
    /// 移除按键抬起监听
    /// </summary>
    public void Remove(string tag, onKeyUpAction action)
    {
        Debug.Assert(upActions.ContainsKey(tag), string.Format("{0:s}按键事件未注册，无法删除！", tag));
        upActions[tag] -= action;
    }
    /// <summary>
    /// 清空监听
    /// </summary>
    public void Clear(string tag)
    {
        Debug.Assert(downActions.ContainsKey(tag), string.Format("{0:s}按键事件未注册，无法清空！", tag));
        downActions.Remove(tag);
    }
    /// <summary>
    /// 清空所有监听
    /// </summary>
    public void ClearAll()
    {
        downActions.Clear();
    }

    public class InputType
    {
        public const string Keyboard = "Keyboard";
        public const string Move = "Move";
        public const string OnTouchMove = "OnTouchMove";
        public const string OnTouchStart = "OnTouchStart";
        public const string OnTouchEnd = "OnTouchEnd";
    }
}