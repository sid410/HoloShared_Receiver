using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//for 3D display of text on top of 3D items
public class ItemTextHandler : MonoBehaviour
{

    public TextMesh textDisplay;

    private const int MAX_LETTER_PER_LINE = 25;

    //splits text and displays it to fit
    public void DisplayText(string text)
    {
        string[] words = text.Split(' ');
        int lineLetterEnum = 0;
        textDisplay.text = "";
        for (int i = 0; i < words.Length; i++)
        {
            //we iterate each word
            string actualWord = words[i];

            //if this word will overflow we go to next line
            if (lineLetterEnum + actualWord.Length > MAX_LETTER_PER_LINE)
            {
                textDisplay.text += '\n'; //we go to the next line
                lineLetterEnum = 0; //we reset the counter
            }
            else
            {
                textDisplay.text += " "; //otherwise we add a space between the words
            }

            textDisplay.text += actualWord; //we add the current word to the text
            lineLetterEnum += actualWord.Length; //we add the current word length to the total
        }
    }


}
