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
    public static List<Event> events = new List<Event>();

    // 특정 이벤트 완료 플래그
    private bool isEventFinished;

    public TextMeshProUGUI realTimeText;
    public TextMeshProUGUI[] eventNameTexts = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] eventTimeTexts = new TextMeshProUGUI[4];

    // 타이머 활성화 플래그
    public bool isTimerActive = true;
    //save, load를 위한 key
    private const string BestRecordKey = "BestRecord";
    //시간을 초기화하기 위한
    private float sessionStartTime;
    //메모리 절약을 위해 이전 기록을 start에서 먼저 불러옴
    public static List<Event> lastRecords = null;

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

        for (int i = 0; i < Mathf.Min(4, initialEventNames.Count); i++)
        {
            eventNameTexts[i].text = $"{initialEventNames[i]}";
        }

        realTimeText.text = $"{startTime:F2}";

        // Load the last records at the start of the game
        lastRecords = MMSaveLoadManager.Load(typeof(List<Event>), BestRecordKey, "Record/") as List<Event>;

        //DisplayLastRecord();
        startTime = 0f;
        sessionStartTime = Time.time;
    }

    // 이벤트가 끝났을 때 호출
    public void OnEventFinished(int eventIndex)
    {
        float eventEndTime = Time.time - sessionStartTime;
        Event newEvent = events[eventIndex];
        newEvent.endTime = eventEndTime;

        eventTimeTexts[eventIndex].text = $"{newEvent.endTime:F2}";


        // Use the loaded lastRecords
        if (lastRecords != null)
        {
            Event previousEvent = lastRecords.Find(e => e.eventName == newEvent.eventName);
            Debug.Log($"이전 이벤트 = {previousEvent.eventName} , {previousEvent.endTime:F2}");
            Debug.Log($"이것은 {newEvent.endTime - previousEvent.endTime:F2}의 차이다");
            Debug.Log($"새 이벤트 = {newEvent.eventName} , {newEvent.endTime:F2}");
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
    }

    void CheckBestRecord()
    {
        MMSaveLoadManager.Save(events, BestRecordKey, "Record/");
    }

    void DisplayLastRecord()
    {
        // 게임 시작시, 이전 게임의 기록을 불러옴
        //var lastRecords = MMSaveLoadManager.Load(typeof(List<Event>), BestRecordKey, "Record/") as List<Event>;

    }
}