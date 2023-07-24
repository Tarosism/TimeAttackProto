using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.CorgiEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class WeaponData
{
    public string WeaponName;
    public float AttackPower;
    // 필요한 만큼 다른 속성을 추가할 수 있습니다.
}
public class SkillManager : MonoBehaviour
{
    public List<List<Skill>> skillSets; // 각 단계별 스킬 세트를 담을 수 있는 리스트

    public SkillManager()
    {
        skillSets = new List<List<Skill>>();
    }
    public List<Skill> currentSkills; // 현재 단계에서 사용가능한 스킬 세트
    public static SkillManager Instance { get; private set; }

    public Button skillButton1;
    public Button skillButton2;

    // Start 메서드 이전에 실행됩니다.
    void Awake()
    {
        // 싱글톤 인스턴스가 이미 있다면 새로운 인스턴스를 파괴합니다.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 아니라면 싱글톤 인스턴스를 이 객체로 설정합니다.
        Instance = this;

        // 씬이 변경되어도 파괴되지 않도록 설정합니다.
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        skillButton1.gameObject.SetActive(false);
        skillButton2.gameObject.SetActive(false);

        skillSets = new List<List<Skill>>
        {
            new List<Skill> { new YesDebug(), new NoDebug() },
            new List<Skill> { new YeeDebug(), new YoyoDebug() }
            // 필요한만큼 더 추가할 수 있습니다.
        };
        int loadedSkillSetIndex = PlayerPrefs.GetInt("CurrentSkillSetIndex", 0); // default value is 0

        currentSkills = skillSets[loadedSkillSetIndex]; // 처음에는 첫 번째 스킬 세트로 시작

        foreach (List<Skill> skillList in skillSets)
        {
            foreach (Skill skill in skillList)
            {
                skill.LoadState();
            }
        }
    }

    public void InitializeSkillButtons() //특정 행동이 일어난 후 스킬트리 초기화
    {
        skillButton1.gameObject.SetActive(true);
        skillButton2.gameObject.SetActive(true);

        // Remove previous listeners
        skillButton1.onClick.RemoveAllListeners();
        skillButton2.onClick.RemoveAllListeners();

        // Assign new listeners
        string skillName1 = currentSkills[0].Name;
        string skillName2 = currentSkills[1].Name;

        skillButton1.onClick.AddListener(() => UnlockSkill(skillButton1, skillName1));
        skillButton2.onClick.AddListener(() => UnlockSkill(skillButton2, skillName2));
    }


    // 다음 스킬 세트로 넘어가는 메서드
    public void AdvanceToNextSkillSet()
    {
        int nextSkillSetIndex = skillSets.IndexOf(currentSkills) + 1;
        if (nextSkillSetIndex < skillSets.Count)
        {
            currentSkills = skillSets[nextSkillSetIndex];
            PlayerPrefs.SetInt("CurrentSkillSetIndex", nextSkillSetIndex);//playerprefs로 간결하게 숫자 저장

            // 여기서 skillButton1과 skillButton2에 대한 클릭 이벤트를 새로운 스킬로 설정합니다.
            string newSkillName1 = currentSkills[0].Name;
            string newSkillName2 = currentSkills[1].Name;

            skillButton1.onClick.RemoveAllListeners();
            skillButton1.onClick.AddListener(() => UnlockSkill(skillButton1, newSkillName1));

            skillButton2.onClick.RemoveAllListeners();
            skillButton2.onClick.AddListener(() => UnlockSkill(skillButton2, newSkillName2));
        }
        skillButton1.gameObject.SetActive(false);
        skillButton2.gameObject.SetActive(false);

        DestroySingletons();
        SceneManager.LoadScene(0);
    }



    // 현재 스킬 세트에서 스킬을 잠금 해제하는 메서드
    public void UnlockSkill(Button button, string skillName)
    {
        Skill skillToUnlock = currentSkills.Find(skill => skill.Name == skillName);
        if (skillToUnlock != null && !skillToUnlock.IsUnlocked) // If the skill is not already unlocked
        {
            skillToUnlock.Unlock();
            AdvanceToNextSkillSet();
        }
        else
        {
            Debug.Log("스킬을 이미 얻으셨습니다!");
        }
    }

    // // 현재 스킬 세트에서 스킬의 효과를 적용하는 메서드
    // public void ApplySkill(string skillName)
    // {
    //     Skill skillToApply = currentSkills.Find(skill => skill.Name == skillName);
    //     // skillToApply.Apply();

    //     if (skillToApply != null && skillToApply.IsUnlocked)
    //     {
    //         skillToApply.Apply();
    //     }
    // }

    public static void DestroySingletons()
    {
        Destroy(TimeTextParent.instance.gameObject);
        Destroy(SpeedRunTimer.Instance.gameObject);
        Destroy(Instance.gameObject);
    }
}

