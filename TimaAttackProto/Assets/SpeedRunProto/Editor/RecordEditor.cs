#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;  // Add this line
using MoreMountains.Tools; // Add this line
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(SpeedRunTimer))]
public class RecordEditor : Editor
{
    private const string BestRecordKey = "BestRecord";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SpeedRunTimer SpeedRunTimer = (SpeedRunTimer)target;
        if (GUILayout.Button("Delete All Records"))
        {
            // Delete all the records saved with MMSaveLoadManager
            MMSaveLoadManager.DeleteSaveFolder("Record/");

            // Optional: Clear the in-memory event list
            SpeedRunTimer.events.Clear();
        }
    }
}
#endif
