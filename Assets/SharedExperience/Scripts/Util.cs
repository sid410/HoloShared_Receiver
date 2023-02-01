using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static string FormatTimeFloat(float timeTaken)
    {
        Debug.Log("time taken is " + timeTaken);
        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(timeTaken);
        return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }
}
