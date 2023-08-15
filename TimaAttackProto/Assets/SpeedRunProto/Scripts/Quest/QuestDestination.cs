using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDestination : MonoBehaviour
{
    public int questIndex;
    public int stepIndex;
    public string questName;
    public GameObject questWall;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && QuestManager.Instance.isQuest)
        {
            QuestManager.Instance.CompleteQuestStep(questIndex, stepIndex);
            questWall.gameObject.SetActive(false);
        }
    }
}

