using System;
using System.Collections.Generic;

using UnityEngine;

public class GameRunningState
{
    public enum RunningState
    {
        Running,
        SpeedUp,
        Pause
    }

    public static RunningState State { get; private set; }

    public static void Pause()
    {
        Time.timeScale = 0;
    }
    public static void SpeedUp(float speed)
    {
        Time.timeScale = speed;
    }
    public static void Run()
    {
        Time.timeScale = 1;
    }

}
