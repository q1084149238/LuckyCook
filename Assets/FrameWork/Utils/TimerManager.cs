using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void timerDel();

public class TimerManager : MonoSingleTon<TimerManager>
{
    /// <summary>
    /// 单次计时器列表
    /// </summary>
    private List<Timer> onceTimerList = new List<Timer>();
    /// <summary>
    /// 循环计时器列表
    /// </summary>
    private List<Timer> loopTimerList = new List<Timer>();
    /// <summary>
    /// 帧循环计时器列表
    /// </summary>
    private List<Timer> frameLoopTimerList = new List<Timer>();
    /// <summary>
    /// later帧循环计时器列表
    /// </summary>
    private List<Timer> laterFrameLoopTimerList = new List<Timer>();
    /// <summary>
    /// 清理计时器列表
    /// </summary>
    private List<Timer> clearTimerList = new List<Timer>();
    /// <summary>
    /// 添加计时器列表
    /// /// </summary>
    private List<Timer> AddTimerList = new List<Timer>();

    public const float IntervalTime = 0.02f;


    private void FixedUpdate()
    {
        foreach (var timer in onceTimerList)
        {
            timer.Action();
        }

        foreach (var timer in loopTimerList)
        {
            timer.Action();
        }
    }

    private void Update()
    {
        if (isPause) return;

        foreach (var timer in frameLoopTimerList)
        {
            timer.Action();
        }
    }

    private bool isPause = false;
    public void Pause()
    {
        isPause = true;
    }
    public void Resume()
    {
        isPause = false;
    }

    private void LateUpdate()
    {
        if (isPause) return;

        foreach (var timer in laterFrameLoopTimerList)
        {
            timer.Action();
        }

        /*在LateUpdate中执行操作，
        避免仍在遍历列表时就已经修改了计时器列表*/
        if (AddTimerList.Count > 0)
        {
            foreach (var timer in AddTimerList)
            {
                switch (timer.type)
                {
                    case TimerType.Loop:
                        loopTimerList.Add(timer);
                        break;
                    case TimerType.FrameLoop:
                        frameLoopTimerList.Add(timer);
                        break;
                    case TimerType.Once:
                        onceTimerList.Add(timer);
                        break;
                    case TimerType.LaterFrameLoop:
                        laterFrameLoopTimerList.Add(timer);
                        break;
                }
            }
            AddTimerList.Clear();
        }

        if (clearTimerList.Count > 0)
        {
            for (int i = 0; i < clearTimerList.Count; i++)
            {
                var timer = clearTimerList[i];
                if (timer == null) continue;

                switch (timer.type)
                {
                    case TimerType.Loop:
                        loopTimerList.Remove(timer);
                        break;
                    case TimerType.FrameLoop:
                        frameLoopTimerList.Remove(timer);
                        break;
                    case TimerType.Once:
                        onceTimerList.Remove(timer);
                        break;
                    case TimerType.LaterFrameLoop:
                        laterFrameLoopTimerList.Remove(timer);
                        break;
                }

                timer = null;
            }
            clearTimerList.Clear();
        }
    }

    /// <summary>
    /// 一次性计时器
    /// </summary>
    public Timer Once(timerDel function, float interVal)
    {
        Timer timer = new Timer(interVal, function, TimerType.Once);
        AddTimerList.Add(timer);

        return timer;
    }
    /// <summary>
    /// 循环计时器
    /// </summary>
    public Timer Loop(timerDel function, float interVal)
    {
        Timer timer = new Timer(interVal, function, TimerType.Loop);
        AddTimerList.Add(timer);

        return timer;
    }
    /// <summary>
    /// 帧循环计时器
    /// </summary>
    public Timer FrameLoop(timerDel function)
    {
        Timer timer = new Timer(0, function, TimerType.FrameLoop);
        AddTimerList.Add(timer);

        return timer;
    }
    /// <summary>
    /// 帧循环计时器
    /// </summary>
    public Timer LateFrameLoop(timerDel function)
    {
        Timer timer = new Timer(0, function, TimerType.LaterFrameLoop);
        AddTimerList.Add(timer);

        return timer;
    }

    /// <summary>
    /// 清除计时器
    /// </summary>
    public void ClearTimer(Timer timer)
    {
        clearTimerList.Add(timer);
    }
    /// <summary>
    /// 清除全部计时器
    /// </summary>
    public void ClearAllTimer()
    {
        loopTimerList.Clear();
        onceTimerList.Clear();
    }
}

/// <summary>
/// 计时器
/// </summary>
public class Timer
{
    public timerDel function;
    public float interval;

    public float currInterval;

    public TimerType type;

    public Timer(float interval, timerDel function, TimerType type)
    {
        this.interval = interval;
        this.currInterval = this.interval;
        this.function = function;
        this.type = type;
    }

    public void Action()
    {
        currInterval -= TimerManager.IntervalTime;
        if (currInterval <= 0)
        {
            function?.Invoke();

            if (type != TimerType.Once)
            {
                currInterval = interval;
            }
            else
            {
                TimerManager.Instance.ClearTimer(this);
            }
        }
    }
}

public enum TimerType
{
    Loop = 0,
    Once = 1,
    FrameLoop = 2,
    LaterFrameLoop = 3,
}
