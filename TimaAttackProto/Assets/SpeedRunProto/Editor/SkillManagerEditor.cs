#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;  // Add this line
using System.Collections.Generic; // Add this line
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(SkillManager))]
public class SkillManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SkillManager skillManager = (SkillManager)target;
        if (GUILayout.Button("Delete All Skills"))
        {
            // Check if 'skillSets' has been initialized
            if (skillManager.skillSets != null)
            {
                foreach (List<Skill> skillList in skillManager.skillSets)
                {
                    foreach (Skill skill in skillList)
                    {
                        skill.DeleteState();
                    }
                }
            }
        }
    }
}
#endif
