using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectiveUpdater;

/**
 * Handles the behaviour of an objective and its text
 * */
public class ObjectiveBehaviour : MonoBehaviour
{
    public TextMesh textdisplay;
    //public GameObject DoneQuestDisplay;
    public Color doneColor;
    public ObjectiveData relatedObjective { get; private set; }
    public bool completed { private set; get; } //we treack if the quest was ever completed

    private const int MAX_LETTERS_PER_LINE = 39;

    public void init(ObjectiveData objectiveData)
    {
        this.relatedObjective = objectiveData;
        //this.DoneQuestDisplay.SetActive(false);
        this.completed = false;
        textdisplay.color = Color.white;
        //textdisplay.text = objectiveData.objectiveText;
        FormatText(objectiveData.objectiveText);
    }

    //splits the text to not overflow the length of the borders
    private void FormatText(string objectiveText)
    {
        string[] words = objectiveText.Split(' ');
        int lineLetterEnum = 0;
        textdisplay.text = "";
        for (int i = 0; i < words.Length; i++)
        {
            //we iterate each word
            string actualWord = words[i];

            //if this word will overflow we go to next line
            if (lineLetterEnum + actualWord.Length > MAX_LETTERS_PER_LINE)
            {
                textdisplay.text += '\n'; //we go to the next line
                lineLetterEnum = 0; //we reset the counter
            } else
            {
                textdisplay.text += " "; //otherwise we add a space between the words
            }

            textdisplay.text += actualWord; //we add the current word to the text
            lineLetterEnum += actualWord.Length; //we add the current word length to the total
        }
    }

    //Sets the objective as done by slashing it
    public void SetObjectiveAsDone()
    {
        completed = true;
        textdisplay.color = doneColor;
        //DoneQuestDisplay.SetActive(true);
    }

    public void SetObjectiveAsFailed()
    {
        completed = false;
        textdisplay.color = Color.white;
        //DoneQuestDisplay.SetActive(false);
    }
}
