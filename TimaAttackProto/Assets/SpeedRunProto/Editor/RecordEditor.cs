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
            // Only perform deletion when not in Play Mode
            if (!EditorApplication.isPlaying)
            {
                // Delete all the records saved with MMSaveLoadManager
                MMSaveLoadManager.DeleteSaveFolder("Record/");

                // Optional: Clear the in-memory event list
                SpeedRunTimer.events.Clear();
                Debug.Log("Records Deleted");
            }
            else
            {
                Debug.LogError("This action can only be performed outside of Play Mode.");
            }
        }
    }
}
#endif
