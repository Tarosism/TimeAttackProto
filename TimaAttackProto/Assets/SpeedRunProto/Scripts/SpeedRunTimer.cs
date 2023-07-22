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
        startTime = 0f;
        realTimeText.text = $"{startTime:F2}";
    }

    // 이벤트가 끝났을 때 호출
    public void OnEventFinished(string BossName)
    {
        float eventEndTime = Time.time;
        events.Insert(0, new Event(BossName, eventEndTime));
        isEventFinished = true;
        if (events.Count > 10) // Keep the list to max 10 elements
        {
            events.RemoveAt(10);
        }
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
                eventTimeTexts[i].text = $"Event {events[i].eventName} End Time: {events[i].endTime:F2}";
            }
            isEventFinished = false;
        }
    }
}



