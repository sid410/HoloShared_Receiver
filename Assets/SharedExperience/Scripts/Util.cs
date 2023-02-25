using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    //constants for calculating the duration a message should be displayed
    private const float timePerLetter = 0.1f;
    private const float extraTime = 2f;


    public static string FormatTimeFloat(float timeTaken)
    {
        Debug.Log("time taken is " + timeTaken);
        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(timeTaken);
        return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }



    //attempt to calculate appropriate time for a message to be displayed before disapeearing
    public static float CalculateMessageDisplayTime(string message)
    {
        return message.Length * timePerLetter + extraTime;
    }
}
