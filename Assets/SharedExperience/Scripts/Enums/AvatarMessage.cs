using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//data needed for a message said by the avatar
[System.Serializable]
public class AvatarMessage
{
    public Vector3 position; //where the avatar must move, set to Vector3.Zero for default position (edge of table

    [TextArea(3, 3)]
    public string message; //message displayed

    public AvatarMessage(string message, Vector3 position)
    {
        this.message = message;
        this.position = position;
    }

    public AvatarMessage(string message)
    {
        this.message = message;
        this.position = Vector3.zero;
    }
}
