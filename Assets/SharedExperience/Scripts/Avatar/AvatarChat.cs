using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * Handles displaying the text said by the 3D Avatar
 */
public class AvatarChat : MonoBehaviour
{
    private const int MAX_LETTER_PER_LINE = 40;
    private const int LINES = 3;
    private const float letterTimeDelay = 0.06f; //time in seconds between each letter being displayed

    public TextMesh chatDisplaytext;
    public GameObject chatDisplayPanel;

    private Coroutine messageDisplayingCoroutine = null;

    //for audio (following Trever Mock tutorial on youtube)
    [Header("Dialogue sound data")]
    public DialogueAudioData dialogueAudioInfo; //contains data about how the voice sounds
    [Range(0f, 1f)]
    public float volume;
    private AudioSource audioSource; //for character talking


    private void Awake()
    {
        audioSource = this.gameObject.AddComponent<AudioSource>();
        audioSource.volume = volume;
    }
    #region subscriptions
    private void OnEnable()
    {
        EventHandler.OnTutorialStepStarted += DisplayTutorialStepMessage;
        //EventHandler.OnTutorialOver += HideChatBox; //we hide the chatbox if the tutorial is over
        EventHandler.OnExerciseStepStarted += DisplayExerciseStepMessage;
        EventHandler.OnExerciseOver += DisplayExerciseOverMessage;
        EventHandler.displayMessage += DisplayMessage;
    }

    private void OnDisable()
    {
        EventHandler.OnTutorialStepStarted -= DisplayTutorialStepMessage;
        //EventHandler.OnTutorialOver -= HideChatBox; //we hide the chatbox if the tutorial is over
        EventHandler.OnExerciseStepStarted -= DisplayExerciseStepMessage;
        EventHandler.OnExerciseOver -= DisplayExerciseOverMessage;
        EventHandler.displayMessage -= DisplayMessage;
    }

    #endregion

    private void DisplayTutorialStepMessage(TutorialData.TutorialStep step) => DisplayMessage(step.avatarText.message, 0f); //each tutorial step has a message

    private void DisplayExerciseStepMessage(ExerciceData.ExerciceStep step) => DisplayMessage(step.avatarText.message, 3f);

    private void DisplayExerciseOverMessage() => DisplayMessage("Well done ! The exercise is over ! You can see your results on the right", 0f); // we simply say the exercise is over

    //duration after the end of the text display for the text to disapeear
    private void DisplayMessage(string message, float duration)
    {
        if (message == null || message.Length == 0) //if no message, we hide the chatbox
        {
            chatDisplaytext.text = "";
            HideChatBox();
            return;
        }
        if (messageDisplayingCoroutine != null) StopCoroutine(messageDisplayingCoroutine);

        //if this item is disabled for any reason, we don't start the coroutine but set the text directly
        if (gameObject.activeSelf) messageDisplayingCoroutine = StartCoroutine(DisplayMessageWordByWord(message, duration));
        else chatDisplaytext.text = message;
    }

    public void HideChatBox()
    {
        chatDisplayPanel.SetActive(false);
    }

    public void ShowChatBoxIfText()
    {
        if (chatDisplaytext.text.Length == 0) return; //we prevent this is there is no text
        ShowChatBox();
    }
    private void ShowChatBox()
    {
        chatDisplayPanel.SetActive(true);
    }


    //coroutine that handles displaying the message. We display letter by letter and for each end line we handle it differently
    //Unused for now
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
                if ((i + 2 < message.Length && (message[i + 1] == ' ' || message[i + 2] == ' ')) || (i + 2 == message.Length)) //if the next letter (or after that) is a whitespace, we put the actual letter and jump to the next line
                {
                    chatDisplaytext.text += message[++i];

                }
                else if (i + 1 < message.Length)//else we put a - and jump to line and put the letter
                {
                    chatDisplaytext.text += '-';
                }
                else
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


    //different coroutine where words do not get split up if they don't fit. wastes space because of no word splitting but more consistent and easier to make work beautifully.
    IEnumerator DisplayMessageWordByWord(string message, float duration)
    {
        int totalLettersDisplayed = 0;
        int LineLetterENum = 0; //calculates number of letter per line (so to know when to jump to next line)
        chatDisplaytext.text = ""; //we reset the chat box
        string[] words = message.Split(' ');
        ShowChatBox(); //we show the chat box
        for (int i = 0; i < words.Length; i++)
        {
            //we iterate each word
            string actualWord = words[i];

            //if this word will overflow we go to next line
            if (LineLetterENum + actualWord.Length > MAX_LETTER_PER_LINE)
            {
                chatDisplaytext.text += '\n'; //we go to the next line
                LineLetterENum = 0; //we reset the counter
            }

            for (int j = 0; j < actualWord.Length; j++) //we do all letters (with a delay)
            {
                chatDisplaytext.text += actualWord[j];
                LineLetterENum++; totalLettersDisplayed++; //we increase the letter count
                PlayDialogueSound(totalLettersDisplayed, actualWord[j]);
                yield return new WaitForSeconds(letterTimeDelay); //we put the same delay
            }

            //we add a space
            chatDisplaytext.text += ' ';
            LineLetterENum++; totalLettersDisplayed++; //we increase the letter count
        }

        //if a duration is specified we hide the chatbox after that delay
        if (duration > 0f)
        {
            yield return new WaitForSeconds(duration);
            HideChatBox();
        }
        yield break;
    }



    //AUDIO : AUDIO For character speaking

    private void PlayDialogueSound(int currentDisplayedCharacterCount, char currentCharacter)
    {
        // set variables for the below based on our config
        AudioClip[] dialogueTypingSoundClips = dialogueAudioInfo.dialogueTypingSoundClips;
        int frequencyLevel = dialogueAudioInfo.frequencyLevel;
        float minPitch = dialogueAudioInfo.minPitch;
        float maxPitch = dialogueAudioInfo.maxPitch;
        bool stopAudioSource = dialogueAudioInfo.stopAudioSource;

        // play the sound based on the config
        if (currentDisplayedCharacterCount % frequencyLevel == 0)
        {
            if (stopAudioSource)
            {
                audioSource.Stop();
            }

            int hashCode = currentCharacter.GetHashCode();
            // sound clip
            int predictableIndex = hashCode % dialogueTypingSoundClips.Length;
            AudioClip soundClip = dialogueTypingSoundClips[predictableIndex];
            // pitch
            int minPitchInt = (int)(minPitch * 100);
            int maxPitchInt = (int)(maxPitch * 100);
            int pitchRangeInt = maxPitchInt - minPitchInt;
            // cannot divide by 0, so if there is no range then skip the selection
            if (pitchRangeInt != 0)
            {
                int predictablePitchInt = (hashCode % pitchRangeInt) + minPitchInt;
                float predictablePitch = predictablePitchInt / 100f;
                audioSource.pitch = predictablePitch;
            }
            else
            {
                audioSource.pitch = minPitch;
            }

            // play sound
            audioSource.PlayOneShot(soundClip);
        }
    }

}
