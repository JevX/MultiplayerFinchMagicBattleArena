using UnityEngine;
 using System.Collections;
 
 public class CustomLog : MonoBehaviour
 {
     private static CustomLog _instance = null; 

     string myLog;
     Queue myLogQueue = new Queue();
 
    void Awake()
    {
        _instance = this;
    }

     void Start()
     {
        _instance = this;
         //Debug.Log("Log1");
         //Debug.Log("Log2");
         //Debug.Log("Log3");
         //Debug.Log("Log4");
     }
 
     void OnEnable () {
         //Application.logMessageReceived += HandleLog;
     }
     
     void OnDisable () {
         //Application.logMessageReceived -= HandleLog;
     }




    public static void Log(string logString)
    {
        Log(logString, "", LogType.Log);
    }
    public static void Log(string logString, string stackTrace, LogType type)
    {
        if (_instance == null)
            return;
        _instance.HandleLog(logString, stackTrace, type);
    }

    private void HandleLog(string logString, string stackTrace, LogType type){
         myLog = logString;
         string newString = "\n [" + type + "] : " + myLog;
         myLogQueue.Enqueue(newString);
         if (type == LogType.Exception)
         {
             newString = "\n" + stackTrace;
             myLogQueue.Enqueue(newString);
         }
         myLog = string.Empty;
         foreach(string mylog in myLogQueue){
             myLog += mylog;
         }
     }
 
     void OnGUI () {
         GUILayout.Label(myLog);
     }
 }