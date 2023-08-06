using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MoreMountains.Tools;


[System.Serializable]
public class Event
{
    public string eventName;
    public float endTime = float.MaxValue; // 최대값으로 초기화

    public Event(string _eventName)
    {
        eventName = _eventName;
    }
}
public class SpeedRunTimer : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static SpeedRunTimer Instance { get; private set; }

    // 게임 시작부터 지난 시간
    private float startTime;

    // 특정 이벤트 완료까지 걸린 시간 리스트
    public List<string> initialEventNames = new List<string>(); // 각 보스 이름을 여기에 미리 채워 넣으세요
    public List<Event> events = new List<Event>();

    // 특정 이벤트 완료 플래그
    private bool isEventFinished;

    public TextMeshProUGUI realTimeText;
    public TextMeshProUGUI[] eventTimeTexts = new TextMeshProUGUI[4];

    // 타이머 활성화 플래그
    public bool isTimerActive = true;
    //save, load를 위한 key
    private const string BestRecordKey = "BestRecord";
    //시간을 초기화하기 위한
    private float sessionStartTime;

    // 싱글톤 인스턴스 설정
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        MMSceneLoadingManager.OnLoadingStarted += () => isTimerActive = false;
        MMSceneLoadingManager.OnLoadingCompleted += () => isTimerActive = true;
    }

    // 시작 시간 초기화
    void Start()
    {
        foreach (var eventName in initialEventNames)
        {
            events.Add(new Event(eventName));
        }

        realTimeText.text = $"{startTime:F2}";
        DisplayLastRecord();
        startTime = 0f;
        sessionStartTime = Time.time;
    }

    // 이벤트가 끝났을 때 호출
    public void OnEventFinished(int eventIndex)
    {
        float eventEndTime = Time.time - sessionStartTime;
        Event newEvent = events[eventIndex];
        newEvent.endTime = eventEndTime;

        Event previousEvent = events.Find(e => e.eventName == newEvent.eventName);

        if (previousEvent != null)
        {
            if (previousEvent.endTime != Mathf.Infinity)
            {
                Debug.Log($"{previousEvent.eventName}의 기록은 {previousEvent.endTime:F2}, 차이는 {previousEvent.endTime - newEvent.endTime:F2}");
            }
            else
            {
                Debug.Log($"{previousEvent.eventName}의 기록은 없음");
            }
        }
        else
        {
            Debug.Log($"{newEvent.eventName}의 기록은 처음입니다.");
        }

        isEventFinished = true;
        CheckBestRecord();
    }



    // 특정 이벤트 완료 시간 출력
    void Update()
    {
        if (isTimerActive)
        {
            startTime += Time.deltaTime;
            realTimeText.text = $"{startTime:F2}";
        }

        if (isEventFinished)
        {
            for (int i = 0; i < Mathf.Min(4, events.Count); i++)
            {
                eventTimeTexts[i].text = $"{events[i].eventName} {events[i].endTime:F2}";
            }
            isEventFinished = false;
        }
    }

    void CheckBestRecord()
    {
        MMSaveLoadManager.Save(events, BestRecordKey, "Record/");
    }

    void DisplayLastRecord()
    {
        // 게임 시작시, 이전 게임의 기록을 불러옴
        var lastRecords = MMSaveLoadManager.Load(typeof(List<Event>), BestRecordKey, "Record/") as List<Event>;
        if (lastRecords != null)
        {
            // 이전 게임의 기록을 출력
            for (int i = 0; i < Mathf.Min(4, lastRecords.Count); i++)
            {
                float timeDifference = lastRecords[i].endTime - events[i].endTime; // 시간 차이 계산
                Debug.Log($"{lastRecords[i].eventName}의 기록은 {lastRecords[i].endTime:F2}, 차이는 {timeDifference:F2}");
            }
        }
    }
}