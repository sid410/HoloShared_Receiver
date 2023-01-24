using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TextAppearAnimation : MonoBehaviour
{
    private const float totalTime = 0.8f;

    public List<TextMesh> animatedTexts = new List<TextMesh>();
    // Start is called before the first frame update
    void OnEnable()
    {
        Debug.Log("enabled animator");
        StartCoroutine(AnimateTexts());
    }

    IEnumerator AnimateTexts()
    {
        float timer = 0.0f;
        float prcent = (timer / totalTime);
        while (timer <= totalTime)
        {
            animatedTexts.ForEach(tm => tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, prcent)); //we increase the alpha step by step
            yield return new WaitForSeconds(0.02f);
            prcent += 0.02f / totalTime;
        }
    }
}
