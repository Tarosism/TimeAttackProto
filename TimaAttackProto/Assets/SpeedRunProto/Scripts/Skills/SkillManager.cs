using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.CorgiEngine;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;
using TMPro;
using PixelCrushers.DialogueSystem;
using MoreMountains.InventoryEngine;

using System.Linq;
using System;
[System.Serializable]
public class WeaponData
{
    public string WeaponName;
    public float AttackPower;
}

public class SkillManager : MonoBehaviour
{
    public List<List<Skill>> skillSets;
    public List<Skill> currentSkills;
    public static SkillManager Instance { get; private set; }

    public Button skillButton1;
    public Button skillButton2;
    public GameObject changeEnemy;
    public GameObject swimGlitch;

    public Inventory inventory { get; set; }
    public InventoryInputManager inventoryInputManager { get; set; }

    public SkillManager()
    {
        skillSets = new List<List<Skill>>();
    }

    void Awake()
    {
        InitializeSingleton();
    }

    void Start()
    {
        InitializeSkillSets();
        InitializeEnemyLayer();
    }

    void InitializeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        inventory = FindObjectOfType<Inventory>();
        inventoryInputManager = FindObjectOfType<InventoryInputManager>();
    }

    void InitializeSkillSets()
    {
        skillButton1.gameObject.SetActive(false);
        skillButton2.gameObject.SetActive(false);
        TextMeshProUGUI skill1Text = skillButton1.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI skill2Text = skillButton2.GetComponentInChildren<TextMeshProUGUI>();

        skillSets = new List<List<Skill>>
        {
            new List<Skill> { new YesDebug(), new NoDebug() },
            new List<Skill> { new YeeDebug(), new YoyoDebug() }
        };

        int loadedSkillSetIndex = PlayerPrefs.GetInt("CurrentSkillSetIndex", 0);
        currentSkills = skillSets[loadedSkillSetIndex];

        foreach (List<Skill> skillList in skillSets)
        {
            foreach (Skill skill in skillList)
            {
                skill.LoadState();
            }
        }

        skill1Text.text = currentSkills[0].Name;
        skill2Text.text = currentSkills[1].Name;
    }

    public void InitializeSkillButtons()
    {
        skillButton1.gameObject.SetActive(true);
        skillButton2.gameObject.SetActive(true);

        skillButton1.onClick.RemoveAllListeners();
        skillButton2.onClick.RemoveAllListeners();

        string skillName1 = currentSkills[0].Name;
        string skillName2 = currentSkills[1].Name;

        skillButton1.onClick.AddListener(() => UnlockSkill(skillButton1, skillName1));
        skillButton2.onClick.AddListener(() => UnlockSkill(skillButton2, skillName2));
    }


    // 다음 스킬 세트로 넘어가는 메서드
    public void AdvanceToNextSkillSet()
    {
        string _saveFolderName = "InventoryEngine/";

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
        MMSaveLoadManager.DeleteSaveFolder(_saveFolderName);
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

    public static void DestroySingletons()
    {
        Destroy(TimeTextParent.instance.gameObject);
        Destroy(SpeedRunTimer.Instance.gameObject);
        Destroy(Instance.gameObject);
    }

    void InitializeEnemyLayer()
    {
        // 모든 스킬 세트에서 NoDebug 스킬을 찾습니다.
        Skill noDebugSkill = null;
        foreach (var skillList in skillSets)
        {
            noDebugSkill = skillList.Find(skill => skill.Name == "NoDebug");
            if (noDebugSkill != null) break;
        }

        // NoDebug 스킬의 잠금 해제 상태에 따라 레이어를 설정합니다.
        if (noDebugSkill != null && !noDebugSkill.IsUnlocked)
        {
            changeEnemy.layer = LayerMask.NameToLayer("Default");
        }
        else
        {
            DialogueSystemTrigger isEnemy = changeEnemy.gameObject.GetComponent<DialogueSystemTrigger>();
            isEnemy.enabled = false; // BoxCollider2D를 비활성화합니다.
        }
    }

    //!현재 skillManager를 짬통으로 쓰는 중(gameManager 역할) 추후 정리필요
    public void ScheduleInventoryClose(float delay, InventoryPickableItem picked)
    {
        StartCoroutine(CloseInventoryAfterRealTime(delay, picked));
    }

    IEnumerator CloseInventoryAfterRealTime(float delay, InventoryPickableItem picked)
    {
        inventoryInputManager.ToggleInventory();
        InventoryItem findItem = inventory.Content.FirstOrDefault(item => item.ItemID == picked.Item.ItemID);
        int idx = Array.FindIndex(inventory.Content, item => item.ItemID == picked.Item.ItemID);
        inventory.EquipItem(findItem, idx, null);
        yield return new WaitForSecondsRealtime(delay);
        inventoryInputManager.ToggleInventory();
    }
    //!위 코루틴은 AutoEquipWeapon class 에서 온 새끼임;;
}

