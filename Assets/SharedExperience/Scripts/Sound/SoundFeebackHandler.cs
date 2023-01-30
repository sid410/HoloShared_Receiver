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
        public bool loop = false;

        [HideInInspector]
        public AudioSource soundSource; //this is generated at runtime and play the sounds independently
    }

    //enum for categorizing sound clip types
    [System.Serializable]
    public enum SoundClipType
    {
        OBJECTIVE_DONE, EXERCISE_STARTED, EXERCISE_OVER, STAR_OBTAINED, SCORE_INCREASE, AVATAR_TALKING
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
        EventHandler.OnExerciseOver += PlayExerciseCompletedSound;
        EventHandler.OnStarAcquired += PlayStarAcquiredSound;
        EventHandler.OnScoreIncreaseStarted += StartScoreIncreasingSound;
        EventHandler.OnScoreIncreaseEnded += EndScoreIncreasingSound;
    }

    private void OnDisable()
    {
        EventHandler.OnObjectiveCompleted -= PlayObjectiveCompletedSound;
        EventHandler.OnExerciseOver -= PlayExerciseCompletedSound;
        EventHandler.OnStarAcquired -= PlayStarAcquiredSound;
        EventHandler.OnScoreIncreaseStarted -= StartScoreIncreasingSound;
        EventHandler.OnScoreIncreaseEnded -= EndScoreIncreasingSound;
    }

    #endregion
    //plays a small sound to announce an objective was completed
    private void PlayObjectiveCompletedSound(int a, GameObject b) => PlaySound(SoundClipType.OBJECTIVE_DONE);

    private void PlayStarAcquiredSound() => PlaySound(SoundClipType.STAR_OBTAINED);

    private void PlayExerciseCompletedSound() => PlaySound(SoundClipType.EXERCISE_OVER);

    private void StartScoreIncreasingSound() => PlaySound(SoundClipType.SCORE_INCREASE);

    private void EndScoreIncreasingSound() => StopSound(SoundClipType.SCORE_INCREASE);
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

    private void StopSound(SoundClipType soundType)
    {
        AudioSound audioData = soundsList.Find(sound => sound.soundType.Equals(soundType));
        if (audioData == null || audioData.soundSource == null)
        {
            Debug.LogWarning("Tried to find sound of type " + soundType + " but failed");
            return;
        }
        audioData.soundSource.loop = audioData.loop;
        audioData.soundSource.Stop();
    }
}
