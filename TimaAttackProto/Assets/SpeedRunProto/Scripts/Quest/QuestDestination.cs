using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDestination : MonoBehaviour
{
    public QuestManager questManager;
    public int questIndex;
    public int stepIndex;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && questManager.isQuest)
        {
            questManager.CompleteQuestStep(questIndex, stepIndex);
        }
    }
}

