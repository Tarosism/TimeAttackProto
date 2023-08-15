using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public Quest[] quests;
    public bool isQuest;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        isQuest = false;
        quests[0].IsCompleted = false;
    }

    // 퀘스트를 시작합니다.
    public void StartQuest(int questIndex)
    {
        isQuest = true;
        quests[questIndex].StartQuest();
    }

    // 퀘스트의 특정 단계를 완료합니다.
    public void CompleteQuestStep(int questIndex, int stepIndex)
    {
        quests[questIndex].CompleteStep(stepIndex, isQuest);
    }

    public void QuestCompleted(string questName)
    {
        Debug.Log(questName + " has been completed!");
    }
}

[System.Serializable]
public class Quest
{
    public string questName;
    public QuestStep[] steps;
    private int currentStep = 0;

    // 퀘스트 완료 상태를 나타내는 변수
    public bool IsCompleted { get; set; } = false;

    public void StartQuest()
    {
        // 이미 완료된 퀘스트는 다시 시작하지 않습니다.
        if (IsCompleted)
        {
            Debug.Log(questName + " has already been completed!");
            return;
        }
        Debug.Log(questName + " has started!");
        steps[0].StartStep();
    }

    public void CompleteStep(int stepIndex, bool isEnd)
    {
        if (IsCompleted)
        {
            return;
        }

        if (stepIndex == currentStep && steps[stepIndex].Complete())
        {
            Debug.Log(steps[stepIndex].description + " completed!");
            currentStep++;
            if (currentStep < steps.Length && isEnd)
            {
                steps[currentStep].StartStep();
            }
            else
            {
                Debug.Log("quest complete!");
                IsCompleted = true; // 퀘스트를 완료로 표시
            }
        }
    }
}


[System.Serializable]
public class QuestStep
{
    public string description;
    public int requiredItemCount;
    public string requiredItemName;
    public int requiredKillCount;
    public string requiredKillEnemyName;

    public void StartStep()
    {
        Debug.Log("New step started: " + description);
    }

    public bool Complete()
    {
        // 여기서 필요한 아이템 수집, 적 처치 등의 조건을 확인합니다.
        // 예를 들어, 플레이어의 인벤토리에서 아이템을 확인하거나, 특정 적을 처치한 횟수를 확인할 수 있습니다.

        // 모든 조건이 충족되면 true를 반환합니다.

        return true;
    }
}
