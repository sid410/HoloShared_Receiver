using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * Handles displaying the text said by the 3D Avatar
 */
public class AvatarChat : MonoBehaviour
{
    private const int MAX_LETTER_PER_LINE = 42;
    private const int LINES = 3;
    private const float letterTimeDelay = 0.06f; //time in seconds between each letter being displayed

    public TextMesh chatDisplaytext;
    public GameObject chatDisplayPanel;

    private Coroutine messageDisplayingCoroutine = null;



    #region subscriptions
    private void OnEnable()
    {
        EventHandler.OnTutorialStepStarted += DisplayTutorialStepMessage;
        EventHandler.OnTutorialOver += HideChatBox; //we hide the chatbox if the tutorial is over
        EventHandler.OnExerciseOver += DisplayExerciseOverMessage;
    }

    private void OnDisable()
    {
        EventHandler.OnTutorialStepStarted -= DisplayTutorialStepMessage;
        EventHandler.OnTutorialOver -= HideChatBox; //we hide the chatbox if the tutorial is over
        EventHandler.OnExerciseOver -= DisplayExerciseOverMessage;
    }

    #endregion
     
    private void DisplayTutorialStepMessage(TutorialData.TutorialStep step) => DisplayMessage(step.message); //each tutorial step has a message

    private void DisplayExerciseOverMessage() => DisplayMessage("Well done ! The exercise is over ! You can see your results on the right"); // we simply say the exercise is over

    private void DisplayMessage(string message)
    {
        if (message == null || message.Length == 0) //if no message, we hide the chatbox
        {
            HideChatBox();
            return;
        }
        if (messageDisplayingCoroutine != null) StopCoroutine(messageDisplayingCoroutine);
        messageDisplayingCoroutine = StartCoroutine(DisplayMessageStepByStep(message));
    }

    private void HideChatBox()
    {
        chatDisplayPanel.SetActive(false);
    }

    private void ShowChatBox()
    {
        chatDisplayPanel.SetActive(true);
    }
    //coroutine that handles displaying the message. We display letter by letter and for each end line we handle it differently
    IEnumerator DisplayMessageStepByStep(string message)
    {
        int LineLetterENum = 0; //calculates number of letter per line (so to know when to jump to next line)
        chatDisplaytext.text = ""; //we reset the chat box
        ShowChatBox(); //we show the chat box
        for (int i = 0; i < message.Length; i++)
        {
            chatDisplaytext.text += message[i];
            LineLetterENum++; //we increase the letter count
            yield return new WaitForSeconds(letterTimeDelay);

            //We check how we want to end the line
            if (LineLetterENum + 1 == MAX_LETTER_PER_LINE) //this letter is going to be the last letter of the line, 
            {
                if ( (i+2 < message.Length && (message[i+1] == ' ' || message[i + 2] == ' ')) || (i+2 == message.Length )) //if the next letter (or after that) is a whitespace, we put the actual letter and jump to the next line
                {
                    chatDisplaytext.text += message[++i];

                } else if (i+1 < message.Length)//else we put a - and jump to line and put the letter
                {
                    chatDisplaytext.text += '-';
                } else
                {
                    chatDisplaytext.text += message[++i]; // the next letter is the final one, we just put it and break (close the coroutine)
                    yield break;
                }
                chatDisplaytext.text += '\n'; //we go to the next line
                LineLetterENum = 0; //we reset the counter
                yield return new WaitForSeconds(letterTimeDelay); //we put the same delay
            }
        }
        yield break;
    }

    
}
