using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MoreMountains.Tools;


[System.Serializable]
public class Event
{
    public string eventName;
    public float endTime;

    public Event(string _eventName, float _endTime)
    {
        eventName = _eventName;
        endTime = _endTime;
    }
}
public class SpeedRunTimer : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static SpeedRunTimer Instance { get; private set; }

    // 게임 시작부터 지난 시간
    private float startTime;

    // 특정 이벤트 완료까지 걸린 시간 리스트
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
        events.Clear();
        realTimeText.text = $"{startTime:F2}";
        DisplayBestRecord();
        startTime = 0f;
        sessionStartTime = Time.time;
    }

    // 이벤트가 끝났을 때 호출
    public void OnEventFinished(string BossName)
    {
        float eventEndTime = Time.time - sessionStartTime;
        events.Insert(0, new Event(BossName, eventEndTime));
        isEventFinished = true;
        if (events.Count > 10)
        {
            events.RemoveAt(10);
        }

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
        //*이전에 기록된 리스트의 endTime과 비교해, 현재 List에 추가된 기록이 더 빠르면 해당 기록을 bestRecord로 만든다
        var savedEvents = MMSaveLoadManager.Load(typeof(List<Event>), BestRecordKey, "Record/") as List<Event>;

        if (savedEvents == null || savedEvents.Count == 0)
        {
            // 이전 기록이 없으면 현재 이벤트가 최고 기록이 됨
            MMSaveLoadManager.Save(events, BestRecordKey, "Record/");
        }
        else
        {
            // 현재 이벤트와 이전 기록을 비교
            for (int i = 0; i < events.Count; i++)
            {
                if (i >= savedEvents.Count || events[i].endTime < savedEvents[i].endTime)
                {
                    // 현재 이벤트가 이전 기록보다 빠르면 최고 기록을 업데이트
                    MMSaveLoadManager.Save(events, BestRecordKey, "Record/");
                    break;
                }
            }
        }
    }

    void DisplayBestRecord()
    {
        //*CheckBestRecord에 의해 BestRecord가 된 기록 List를 게임이 시작하면 불러온다
        var bestRecords = MMSaveLoadManager.Load(typeof(List<Event>), BestRecordKey, "Record/") as List<Event>;
        if (bestRecords != null)
        {
            // 이전의 최고 기록을 출력
            for (int i = 0; i < Mathf.Min(4, bestRecords.Count); i++)
            {
                Debug.Log($"{bestRecords[i].eventName}의 최고기록은 {bestRecords[i].endTime:F2}");
            }
        }
    }
}