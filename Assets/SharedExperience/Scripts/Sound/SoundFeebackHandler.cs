using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Triggers sound effects depending on the events
 */

public class SoundFeebackHandler : MonoBehaviour
{
    //used for referencing dictionary of sounds
    [System.Serializable]
    public class AudioSound
    {
        public SoundClipType soundType;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume;

        [HideInInspector]
        public AudioSource soundSource; //this is generated at runtime and play the sounds independently
    }

    //enum for categorizing sound clip types
    [System.Serializable]
    public enum SoundClipType
    {
        OBJECTIVE_DONE, EXERCISE_STARTED, EXERCISE_OVER, AVATAR_TALKING
    }
    
    public List<AudioSound> soundsList = new List<AudioSound>(); //database for all sounds
    // Start is called before the first frame update

    private void Awake()
    {
        //we create an audioSource for each sound, that way sounds can overlap
        foreach (AudioSound audioSound in soundsList)
        {
            audioSound.soundSource = gameObject.AddComponent<AudioSource>();
            audioSound.soundSource.clip = audioSound.clip;
            audioSound.soundSource.volume = audioSound.volume;
        }
    }

    #region subscriptions
    private void OnEnable()
    {
        EventHandler.OnObjectiveCompleted += PlayObjectiveCompletedSound;
    }

    private void OnDisable()
    {
        EventHandler.OnObjectiveCompleted -= PlayObjectiveCompletedSound;
    }

    #endregion
    //plays a small sound to announce an objective was completed
    private void PlayObjectiveCompletedSound(int a, GameObject b) => PlaySound(SoundClipType.OBJECTIVE_DONE);



    //plays a sound
    private void PlaySound(SoundClipType soundType)
    {
        AudioSound audioData = soundsList.Find(sound => sound.soundType.Equals(soundType));
        if (audioData == null || audioData.soundSource == null)
        {
            Debug.LogWarning("Tried to find sound of type " + soundType + " but failed");
            return;
        }

        audioData.soundSource.Play();
    }
}
