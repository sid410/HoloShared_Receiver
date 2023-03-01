using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class to test out mouvements of the avatar
public class AvatarDebugTest : MonoBehaviour
{
    public AvatarMouvement avatarMouvement;

    void Start()
    {
        StartCoroutine(TestMouvements());
    }

    IEnumerator TestMouvements()
    {
        yield return new WaitForSeconds(1f);
        avatarMouvement.SetDestination(new Vector3(0.473f, 0.224f, -0.288f));
        EventHandler.Instance.DisplayMessage("Hi, i'm bunny chan you guide :3", 3f);
    }
}
