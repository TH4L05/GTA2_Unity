/// <author>Thoams Krahl</author>

using System;
using UnityEngine;

public class Frenzy : MonoBehaviour
{
    public static Action<int> FrenzyStart;
    public static Action<float,int> FrenzyUpdate;
    public static Action<bool> FrenzyEnd;

    private float duration;
    private float time;
    private bool failed;
    private int killAmount;
    private int currentKills;

    public void StartFrenzy(float duration, int killAmount)
    {
        currentKills = 0;
        failed = false;

        this.killAmount = killAmount;
        this.duration = duration;
        InvokeRepeating("UpdateTime", 0f, 1f);

    }

    private void UpdateTime()
    {
        time += 1f;
        FrenzyUpdate?.Invoke(duration, currentKills);

        if (time >= duration)
        {
            failed = true;
            FrenzyExit();
            return;
        }

        if (currentKills >= killAmount)
        {
            failed = false;
            FrenzyExit();
        }   
    }

    private void FrenzyExit()
    {
        CancelInvoke("UpdateTime");
        FrenzyEnd?.Invoke(failed);
    }
}
