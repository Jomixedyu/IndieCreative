using UnityEngine;

public class GameRunningState
{
    public enum RunningState
    {
        Running,
        Pause
    }
    public enum SpeedState
    {
        Normal,
        Speed,
    }

    public static RunningState Running { get; private set; }
    public static SpeedState Speed { get; private set; }

    public static void Run()
    {
        Time.timeScale = 1;
        Running = RunningState.Running;
    }
    public static void Pause()
    {
        Time.timeScale = 0;
        Running = RunningState.Pause;
    }

    public static void NormalSpeed()
    {
        Time.timeScale = 1;
        Speed = SpeedState.Normal;
    }
    public static void SpeedUp(float speed)
    {
        Time.timeScale = speed;
        Speed = SpeedState.Speed;
    }


}
