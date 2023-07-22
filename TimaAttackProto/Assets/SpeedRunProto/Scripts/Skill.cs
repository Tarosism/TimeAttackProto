using UnityEngine;
using System.Collections.Generic;
using System;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;

[Serializable]
public abstract class Skill
{
    public string Name { get; private set; }
    public bool IsUnlocked { get; private set; }
    public Action OnApply { get; set; } // 이것은 Apply 함수가 호출될 때 실행할 액션을 저장합니다.

    protected Skill(string name)
    {
        Name = name;
        IsUnlocked = false;
    }

    public void Unlock()
    {
        IsUnlocked = true;
        // 저장 로직
        SaveState();
    }

    public void Apply()
    {
        OnApply?.Invoke(); // 여기서 delegate를 호출하여 실제 동작을 수행합니다.
    }

public void SaveState()
{
    SerializedSkill skillToSave = new SerializedSkill();
    skillToSave.SkillName = this.Name;
    skillToSave.IsUnlocked = this.IsUnlocked;

    // 스킬을 저장. 세 번째 인자로 폴더 이름을 전달.
    MMSaveLoadManager.Save(skillToSave, "Skill_" + this.Name, "MyFolder/");
}


public void LoadState()
{
    SerializedSkill loadedSkill = (SerializedSkill)MMSaveLoadManager.Load(typeof(SerializedSkill), "Skill_" + this.Name, "MyFolder/");

    if (loadedSkill != null)
    {
        this.IsUnlocked = loadedSkill.IsUnlocked;
        Debug.Log("Skill " + this.Name + " is loaded. Unlocked status: " + this.IsUnlocked);
    }
}

public void DeleteState()
{
    MMSaveLoadManager.DeleteSave("Skill_" + this.Name, "MyFolder/");
}


}

public class YesDebug : Skill
{


    public YesDebug()
        : base("YesDebug")
    {
        OnApply += YesLog;
    }

    void YesLog()
    {
        Debug.Log("yes");
    }
}


public class NoDebug : Skill
{


    public NoDebug()
        : base("NoDebug")
    {
        OnApply += NoLog;
    }

    void NoLog()
    {
        Debug.Log("no");
    }


}

public class YeeDebug : Skill
{
    public YeeDebug() : base("YeeDebug")
    {
        OnApply += YeeLog;
    }

    void YeeLog()
    {
        Debug.Log("yee");
    }
}

public class YoyoDebug : Skill
{
    public YoyoDebug() : base("YoyoDebug")
    {
        OnApply += YoyoLog;
    }

    void YoyoLog()
    {
        Debug.Log("yoyo");
    }
}

[Serializable]
public class SerializedSkill
{
    public string SkillName;
    public bool IsUnlocked;
}
