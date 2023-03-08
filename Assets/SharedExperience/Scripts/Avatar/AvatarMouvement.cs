using BoingKit;
using M2MqttUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



/**
 * The avatar uses the Boing mouvement asset from the asset store (paying but I already had it so I used it)
 * This class extends the basic controller to set destinations to the avatar for the mouvement
 * The avatar does not have an animator but has a bouncy idle animatio,
 */
public class AvatarMouvement : UFOController
{

    [Header("Avatar destination and behaviour")]
    [SerializeField] private Vector3 defaultLocation;

    [Header("(optional) Chat box, used if you have a chat tied to the avatar (put a chat panel inside the UI)")]
    [SerializeField] private AvatarChat avatarChatPanel;
    [SerializeField] private Vector3 default_chatPanel_localpos; //default location under the avatar
    [SerializeField] private Vector3 alternate_chatPanel_localpos; //alternate position on the side of the avatar

    //margin before we consider destination as reached
    float distanceMargin = 0.04f;

    //used for moving to a destination ! when we go to a gameobject, we use world position. When moving to a Vector3 position, we use local position
    Vector3 Destination = Vector3.zero;     //sets the destination of the Avatar ! This is a local position relative to the stones origin
    bool localDestination = true; 

    Camera mainCamera; //camera used to always look at
    BaseClient mttqclient; //client to communicate with webapp, this allows us to disable hints

    //For hints
    List<AvatarHint> avatarHints = new List<AvatarHint>(); //used for in game hints, avatar periodically shows hints if user needs them
    private AvatarHint actualHint = null; //we keep track of the actual hint since it takes time to display
    float timeSinceExerciseStart = 0f;

    

    //for hints
    private void Awake()
    {
        if (avatarChatPanel != null) avatarChatPanel.transform.localPosition = default_chatPanel_localpos;
        mainCamera = Camera.main;
        mttqclient = GameObject.Find("BaseClient").GetComponent<BaseClient>();
        transform.LookAt(mainCamera.transform); //we look at the player
    }

    protected new void OnEnable()
    {
        base.OnEnable();
        //EventHandler.OnExerciseStepStarted += LoadMapHints;
        EventHandler.OnExerciseStepOver += FullReset;
        EventHandler.OnNewAvatarDestination += SetDestination;
        EventHandler.OnMapSpawned += LoadMapHints;
        EventHandler.OnTutorialOver += FullReset;
        EventHandler.OnAppReset += FullReset;
    }

    private void OnDisable()
    {
        EventHandler.OnExerciseStepOver -= FullReset;
        EventHandler.OnMapSpawned -= LoadMapHints;
        EventHandler.OnNewAvatarDestination -= SetDestination;
        EventHandler.OnTutorialOver -= FullReset;
        EventHandler.OnAppReset -= FullReset;

    }

    #region Hints and hints handlers
    
    //gets the spawned map's hints.
    private void LoadMapHints(GameObject map)
    {
        if (AppStateHandler.appState != AppStateHandler.AppState.EXERCICE) return; //we are only interested in exercises
        avatarHints.AddRange(map.transform.Find("Hints").GetComponentsInChildren<AvatarHint>().ToList()); //we add the list of possibles hints here
    }

    //coroutine, resets the avatar after the hint has been displayed long enough
    IEnumerator EndHintAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); //we wait until the messagei s read
        EventHandler.Instance.DisplayMessage("", 0f);
        actualHint = null; //we reset the hint
        ResetPosition(); //we go back to the source
    }
    #endregion
    #region destination settings
    public void SetDestination(Vector3 destination)
    {
        localDestination = true; //we use local position
        SetupDestination(destination);
    }

    public void SetDestination(GameObject target)
    {
        localDestination = false; //we use world position
        SetupDestination(target.transform.position);
    }

    //private function to avoid duplicate code for destinations
    private void SetupDestination(Vector3 destination)
    {
        this.Destination = this.Destination = destination;
        transform.LookAt(destination);
        if (avatarChatPanel != null) //we put the chatbox in an alternate position to not block sight
        {
            avatarChatPanel.transform.localPosition = alternate_chatPanel_localpos;
            avatarChatPanel.HideChatBox();
        }
    }

    //puts the avatar back to default position
    public void ResetPosition()
    {
        this.Destination = defaultLocation; //we set the location as the default location
        if (avatarChatPanel != null)
        {
            avatarChatPanel.transform.localPosition = default_chatPanel_localpos;
            avatarChatPanel.HideChatBox();
        }
        localDestination = true;
    }
    #endregion


    private void Update()
    {
            //we check if we need to trigger the avatar to show a new hint or not
            if (avatarHints == null || avatarHints.Count == 0 || (ExerciseSettings.Instance != null
                && !ExerciseSettings.Instance.hintsEnabled )) return;
        timeSinceExerciseStart += Time.deltaTime;


        if (actualHint != null) return; //we stop double hints colliding
        AvatarHint triggeredHint = avatarHints.FirstOrDefault(hint => hint.countdown <= timeSinceExerciseStart);
        if (triggeredHint == null) return; //if no hint is ready we return

        //trigger hint here
        actualHint = triggeredHint; //we save the hint
        SetDestination(triggeredHint.gameObject); //we set the new destination for the avatar, it will move to the position of the hint to display it
        avatarHints.Remove(triggeredHint); //we remove the hint afterwards

    }

    
    void FixedUpdate()
    {
        if (Destination == Vector3.zero)
        {
            transform.LookAt(mainCamera.transform);
            return;
        }
        Vector3 linearInputVec = Destination - (localDestination ?  transform.localPosition : transform.position);
        if (linearInputVec.magnitude <= distanceMargin) 
        {   //the avatar has reached its destination ! it will now do what must be done there
            Destination = Vector3.zero;
            if (avatarChatPanel != null) avatarChatPanel.ShowChatBoxIfText();
            transform.LookAt(mainCamera.transform); //we look at the user
            if (EventHandler.Instance != null) EventHandler.Instance.InformAvatarReachedDestination(); //we inform anyone that needs to know that the avatar reached the destination

            if (actualHint != null)
            {
                EventHandler.Instance.DisplayMessage(actualHint.message, 0f); //if this was for a hint, the hint is now displayed

                //we start a coroutine to stop the hint after a delay
                StartCoroutine(EndHintAfterDelay(Util.CalculateMessageDisplayTime(actualHint.message)));
            }

            return;
        }
        CalculateSimpleMovement(linearInputVec);
    }

    

    #region reset

    private void ResetHints()
    {
        avatarHints.Clear();
        StopAllCoroutines();
        actualHint = null;
        timeSinceExerciseStart = 0f;
    }

    private void FullReset()
    {
        ResetHints();
        ResetPosition();
    }
    #endregion
}
