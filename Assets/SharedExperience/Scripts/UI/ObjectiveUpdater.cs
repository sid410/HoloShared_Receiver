using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveUpdater : MonoBehaviour
{

    public class ObjectiveData
    {
        public int objectiveIndex; //used to track the objective and correctly recognize the item when ending the objective (must be unique !!)
        public UtensilBehaviour utensilBehaviour;
        public string objectiveText { get; private set; }

        //TODO : deprecate this constructor
        public ObjectiveData(UtensilBehaviour utensilBehaviour)
        {
            this.utensilBehaviour = utensilBehaviour;
            this.objectiveText = "Set " + utensilBehaviour.type.ToString() + " At the correct location";
        }

        public ObjectiveData(int index, string objectiveText)
        {
            this.objectiveIndex = index;
            this.objectiveText = objectiveText;
        }
    }


    #region Private data
    private List<ObjectiveBehaviour> objectiveList = new List<ObjectiveBehaviour>();
    private int completedObjectives = 0; //we track how many objectives have been completed
    #endregion

    public ObjectiveBehaviour objectivePrefab; //prefab for UI visualisation of objectives

    [SerializeField] private GridObjectCollection gridObjectCollection; //used for 3D list isplay of objectives

    private IObjectiveHandler exerciseObjectiveHandler = null; //Class implemented interface, should contain the basic behaviours when spawning objectives
    // Start is called before the first frame update


    private void OnEnable() //todo : remake it into onEnable
    {
        EventHandler.OnExerciseStepStarted += OnExerciseStepStarted;
        EventHandler.OnItemSpawned += OnItemSpawned;
        EventHandler.OnObjectiveCompleted += OnObjectiveCompleted;
        EventHandler.OnObjectiveFailed += OnObjectiveFailed;
        EventHandler.OnAppReset += ResetObjectives;
        
    }

    private void OnDisable()
    {
        EventHandler.OnExerciseStepStarted -= OnExerciseStepStarted;
        EventHandler.OnItemSpawned -= OnItemSpawned;
        EventHandler.OnObjectiveCompleted -= OnObjectiveCompleted;
        EventHandler.OnObjectiveFailed -= OnObjectiveFailed;
        EventHandler.OnAppReset -= ResetObjectives;

    }


    /************************************************************** NEW ******************************************************************/

    //called at the start of the exercise, 
    void OnExerciseStepStarted(ExerciceData.ExerciceStep exerciseStep)
    {
        ResetObjectives(); //we reset all objectives at the start
        IObjectiveHandler objectives = exerciseStep.objectives;
        if (objectives == null) return;
        exerciseObjectiveHandler = objectives;
        ObjectiveData mainObjective = exerciseObjectiveHandler.getExerciseInitObjective(); //we get an objective tied with the start of the exercise

        if (mainObjective == null) return; //if this exercise has no main Objective, we ignore
        InstantiateObjectivePrefab(mainObjective);
        gridObjectCollection.UpdateCollection();
    }

    //called when an Item is instantiated, items decide to announce themSelves as instantiated
    void OnItemSpawned(GameObject spawnedItem)
    {
        if (exerciseObjectiveHandler == null) return;
        ObjectiveData mainObjective = exerciseObjectiveHandler.getObjectiveFromSpawnedItem(spawnedItem); //we get an objective tied to the spawned item

        if (mainObjective == null) return; //if this exercise has no main Objective, we ignore
        InstantiateObjectivePrefab(mainObjective);
        gridObjectCollection.UpdateCollection();
    }

    //declared objective completed by a gameobject, register it
    void OnObjectiveCompleted(int objectiveIndex, GameObject declarer)
    {
        ObjectiveBehaviour relatedObjective = objectiveList.Find(ob => ob.relatedObjective.objectiveIndex == objectiveIndex);
        if (relatedObjective == null || relatedObjective.completed) return;
        completedObjectives++;
        relatedObjective.SetObjectiveAsDone();

        if (completedObjectives >= objectiveList.Count) //if all objectives are done, we trigger the end of the exercice
        {
            EventHandler.Instance.LogMessage("All objectives are completed declaring exercice over in 2 seconds");
            Invoke("InformAllObjectivesCompleted", 2f);
        }
    }

    //Called when an object declares an already completed objective has been failed
    void OnObjectiveFailed(int objectiveIndex, GameObject declarer)
    {
        ObjectiveBehaviour relatedObjective = objectiveList.Find(ob => ob.relatedObjective.objectiveIndex == objectiveIndex);
        if (relatedObjective == null || !relatedObjective.completed) return;
        completedObjectives--;
        relatedObjective.SetObjectiveAsFailed();
    }

    /************************************************************** OLD ******************************************************************/

    //Called when An objective is done, updates the display
    public void SetObjectiveAsComplete(UtensilBehaviour doneUtensil)
    {
        ObjectiveBehaviour relatedObjective = objectiveList.Find(ob => ob.relatedObjective.utensilBehaviour.itemID == doneUtensil.itemID);
        if (relatedObjective == null || relatedObjective.completed) return;
        completedObjectives++;
        relatedObjective.SetObjectiveAsDone();

        if (completedObjectives >= objectiveList.Count) //if all objectives are done, we trigger the end of the exercice
        {
            EventHandler.Instance.LogMessage("All objectives are completed declaring exercice over in 2 seconds");
            Invoke("InformAllObjectivesCompleted", 2f);
        }
    }

    //called when objective failed (for example an item was removed from its slot), we cancel it
    public void SetObjectiveAsFailed(UtensilBehaviour doneUtensil)
    {
        ObjectiveBehaviour relatedObjective = objectiveList.Find(ob => ob.relatedObjective.utensilBehaviour.itemID == doneUtensil.itemID);
        if (relatedObjective == null || !relatedObjective.completed) return;
        completedObjectives--;
        relatedObjective.SetObjectiveAsFailed();
    }

    //registers an new intansiated utensil to generate an appropriate objective related
    public void RegisterObjectiveUtensil(UtensilBehaviour newUtensil)
    {
        InstantiateObjectivePrefab(new ObjectiveData(newUtensil));
        gridObjectCollection.UpdateCollection();
    }

    //called after all objectives have been completed, we inform that the exercice is over
    private void InformAllObjectivesCompleted()
    {
        if (completedObjectives < objectiveList.Count) return;
        EventHandler.Instance.LogMessage("Objective completed ! exercice closing");
        EventHandler.Instance.EndExercise();
    }

    //intantiate an Objective object and append it to the objective list
    private void InstantiateObjectivePrefab(ObjectiveData objectiveData)
    {
        var objectiveInstance = Instantiate(objectivePrefab, gridObjectCollection.transform);
        objectiveInstance.init(objectiveData);
        objectiveList.Add(objectiveInstance);
        //TODO : maybe keep track

    }


    //resets the objectives and the display
    private void ResetObjectives()
    {
        completedObjectives = 0;
        if (objectiveList == null) return;
        foreach(ObjectiveBehaviour objective in objectiveList)
        {
            Destroy(objective.gameObject);
        }
        objectiveList = new List<ObjectiveBehaviour>(); ;
    }
}
