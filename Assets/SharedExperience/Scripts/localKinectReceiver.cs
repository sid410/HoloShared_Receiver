using M2MqttUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtensilExtension;

public class localKinectReceiver : MonoBehaviour
{

    #region data pack classes
    public struct KinectUtensilData
    {
        public UtensilType type;
        public Vector3 position;
        public float orientation;

        public KinectUtensilData(UtensilType type, Vector3 position, float orientation)
        {
            this.type = type;
            this.position = position;
            this.orientation = orientation;
        }
    }
    #endregion
    #region consts
    private const float maxTableDist = 1266.0f;
    private const float maxObjectAngle = 90.0f;
    #endregion

    #region camera calibration and environement variables
    //p00 =        1069  (1068, 1070)
    //   p10 =     -0.5824  (-0.584, -0.5807)
    //   p01 =     0.02914  (0.02692, 0.03135)
    //   p20 =   1.832e-07  (-6.295e-07, 9.958e-07)
    //   p11 =  -2.633e-05  (-2.742e-05, -2.525e-05)
    //   p02 =   -1.11e-06  (-3.074e-06, 8.552e-07)
    private const float p00_Fx = -32.64f; //- 55.89f;
    private const float p10_Fx = 0.584f; //0.7283f;
    private const float p01_Fx = -0.02413f; //- 0.01048f;
    private const float p20_Fx = 1.88e-07f; //9.616e-06f;
    private const float p11_Fx = 2.219e-05f; //3.686e-06f;
    private const float p02_Fx = 4.437e-07f; //1.093e-06f;


    //p00 =       659.2  (658.6, 659.8)
    //p10 =   0.0008737  (-0.0002728, 0.00202)
    //p01 =     -0.5677  (-0.5693, -0.5662)
    //p20 =  -6.268e-07  (-1.182e-06, -7.116e-08)
    //p11 =   3.328e-07  (-4.105e-07, 1.076e-06)
    //p02 =   -2.54e-05  (-2.675e-05, -2.406e-05)
    private const float p00_Fy = 69.87f; //2.549f;
    private const float p10_Fy = -0.001493f; //0.001928f;
    private const float p01_Fy = 0.5732f; //0.7354f;
    private const float p20_Fy = 8.108e-07f; //- 9.061e-07f;
    private const float p11_Fy = -1.521e-06f; //9.17e-06f;
    private const float p02_Fy = 2.178e-05f; //3.99e-06f;




    //constants to reproject the correction of objects with height
    private const float camHeight = 0.63f; //-------------- (IN EXPERIMENT ROOM)
    private const float bottleHeight = 0.305f;
    private const float glassHeight = 0.16f;
    private const float cupHeight = 0.105f;
    private const float dishHeight = 0.08f;
    //camera plane in xy space coordinate, projected to table
    private const float camCenterPixel_x = 960.0f;
    private const float camCenterPixel_y = 540.0f;
    private float camCenterTable_x, camCenterTable_y;
    private float distanceMedian, angleMedian;

    private const float table_X_offset = 190f;
    private const float table_Y_offset = 132.5f;

    #endregion

    public BaseClient baseClient;
    public GameObject stonesOrigin;
    public GameObject tableBoundaryDelimiterPrefab;

    //store the generated items from kinect
    List<GameObject> spawnedUtensils = new List<GameObject>();
    //this is a boolean set to true after an exercice is completed, it then waits for the fina matlab results to set the final score for the utensils
    private bool waiting_for_final_results = false;
    private bool in_exercise = false;

    private KinectResultsAbs exerciseKinectDataHandler = null; //EXERCISE DEPENDANT, handles incoming kinect data
    private void Start()
    {
        //calculate projected camera center on table
        camCenterTable_x = PixelToTableX(camCenterPixel_x, camCenterPixel_y);
        camCenterTable_y = PixelToTableY(camCenterPixel_x, camCenterPixel_y);
        //if (LocalItemSpawner.SINGLE_HOLO_BEHAVIOUR) SpawnDebugBoundaries();
    }


    #region subscriptions
    private void OnEnable()
    {
        baseClient.RegisterTopicHandler("M2MQTT/Matlab/DataResults", HandleMatlabResults);
        EventHandler.OnExerciseLoaded += LoadExerciseHandler; //we get the kinect handler from the exercise data //TODO : also destroy old one
        EventHandler.OnExerciseStepStarted += OnExerciseStepStarted;
        EventHandler.OnExerciseStepOver += OnExerciseStepOver;
        EventHandler.OnItemSpawned += RegisterSpawnedItem; //we register all spawned items in a local list
    }

    private void OnDisable()
    {
        baseClient.UnregisterTopicHandler("M2MQTT/Matlab/DataResults", HandleMatlabResults);
        EventHandler.OnExerciseLoaded -= LoadExerciseHandler;
        EventHandler.OnExerciseStepOver -= OnExerciseStepOver;
        EventHandler.OnExerciseStepStarted -= OnExerciseStepStarted;
        EventHandler.OnItemSpawned -= RegisterSpawnedItem;
    }
    #endregion

    //load the new exercise kinect logic 
    private void LoadExerciseHandler(FullExerciceData fed)
    {
        if (exerciseKinectDataHandler != null) exerciseKinectDataHandler.Cleanup();
        exerciseKinectDataHandler = fed.kinectResultHandler;
        exerciseKinectDataHandler.Init();
    }

    private void OnExerciseStepOver(){waiting_for_final_results = true; in_exercise = true;} //when exercise is over, we wait for the final kinect results before going over to results, this is helpful for countdown exercises that need accurate final results

    private void OnExerciseStepStarted(ExerciceData.ExerciceStep es) => in_exercise = true;

    private void RegisterSpawnedItem(GameObject item) => spawnedUtensils.Add(item.gameObject);

    private void HandleMatlabResults(string topic, string message)
    {
        EventHandler.Instance.LogMessage("Matlab results came in !");
        //ClearKinectUtensils(); //we clear the old utensils
        if (!waiting_for_final_results && !in_exercise) return; //TODO : enable this, block any kinect result usage if its not an exercice
        EventHandler.Instance.LogMessage("Matlab results accepted !");
        EventHandler.Instance.TriggerBeforeMatlabReceived(); //we inform listeners that matlab results are going to be parsed
        string[] results = message.Split('\n');

        //we parse the data and pass it to the listener if needed
        if (exerciseKinectDataHandler != null)
        {
            List<KinectUtensilData> parsedData = new List<KinectUtensilData>();
            foreach (string res in results)
            {
                if (res == "") continue;

                string[] data = res.Split(';');
                //PairRealToVirtualObjects(data, virtualObjectsList);
                parsedData.Add(ParseKinectData(data));
            }
            exerciseKinectDataHandler.HandleKinectData(parsedData);

        }

        //different behaviour for the final wave of results received from the kinect.
        if (waiting_for_final_results)
        {
            waiting_for_final_results = false;
            EventHandler.Instance.TriggerFinalMatlabReceived(); //used to process the data received 
            EventHandler.Instance.TriggerFinalMatlabProcessed(); //used to announce data was processed (for results)
            return;
        }
        EventHandler.Instance.TriggerAfterMatlabReceived(); //we inform that matlab results have been received and all utensils updated
    }


    //DEPRECATED AND IS NOW IN UTENSILKINECTHANDLER (for the basic utensil exercise only)
    private KinectUtensilData ParseKinectData(string[] uData)
    {
        float origin_Xmm = PixelToTableX(float.Parse(uData[0]), float.Parse(uData[1]));
        float origin_Ymm = PixelToTableY(float.Parse(uData[0]), float.Parse(uData[1]));
        float axis_Degrees = float.Parse(uData[2]) + 90.0f; //because pixel to table is offset by 90 deg rot in Y
        String itemTypeStr = uData[3].Trim();
        UtensilType itemType = UtensilUtil.getUtensilFromStr(itemTypeStr);

        Vector3 realPos;
        switch (itemType)
        {
            case UtensilType.CUP:
                realPos = CorrectProjectionObjectsWithHeight(origin_Xmm, origin_Ymm, cupHeight);
                break;
            case UtensilType.DISH:
                realPos = CorrectProjectionObjectsWithHeight(origin_Xmm, origin_Ymm, dishHeight);
                break;
            case UtensilType.BOTTLE:
                realPos = CorrectProjectionObjectsWithHeight(origin_Xmm, origin_Ymm, bottleHeight);
                break;
            case UtensilType.GLASS:
                realPos = CorrectProjectionObjectsWithHeight(origin_Xmm, origin_Ymm, glassHeight);
                break;
            default:
                realPos = new Vector3(origin_Xmm, 0, origin_Ymm);
                break;
        }


        return new KinectUtensilData(itemType, realPos, axis_Degrees);
    }
    

    #region calculations
    private float PixelToTableX(float x_pixel, float y_pixel)
    {
        float x_table = p00_Fx + (p10_Fx * x_pixel) + (p01_Fx * y_pixel) + (p20_Fx * x_pixel * x_pixel) + (p11_Fx * x_pixel * y_pixel) + (p02_Fx * y_pixel * y_pixel);
        x_table = (x_table - table_X_offset) / 1000;
        return x_table;
    }
    private float PixelToTableY(float x_pixel, float y_pixel)
    {
        float y_table = p00_Fy + (p10_Fy * x_pixel) + (p01_Fy * y_pixel) + (p20_Fy * x_pixel * x_pixel) + (p11_Fy * x_pixel * y_pixel) + (p02_Fy * y_pixel * y_pixel);
        y_table = (y_table - table_Y_offset) / -1000;
        return y_table;
    }
    private Vector3 CorrectProjectionObjectsWithHeight(float x_uncorrected, float y_uncorrected, float h_object)
    {
        float correctionRatio = 1 - (h_object / camHeight);
        float x_corrected = ((x_uncorrected - camCenterTable_x) * correctionRatio) + camCenterTable_x;
        float y_corrected = ((y_uncorrected - camCenterTable_y) * correctionRatio) + camCenterTable_y;

        return new Vector3(x_corrected, 0, y_corrected);
    }

    #endregion


    #region deprecated

    //generates delimiters for the table limits
    private void SpawnDebugBoundaries()
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                GameObject delimiter = Instantiate(tableBoundaryDelimiterPrefab);
                delimiter.transform.parent = stonesOrigin.transform;
                delimiter.transform.localPosition = new Vector3(i, 0, -j * 0.5f);
                delimiter.name = "Delimiter_" + i + "_" + j;
            }
        }
    }
    #endregion
}
