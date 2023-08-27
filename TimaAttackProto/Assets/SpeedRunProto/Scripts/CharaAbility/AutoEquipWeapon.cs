using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.InventoryEngine;
using MoreMountains.CorgiEngine;
public class AutoEquipWeapon : MonoBehaviour
{
    private bool shouldAutoEquip = true;
    private void Awake()
    {
        //scene로딩으로 들어갈 때는 인벤토리를 안 엽니다
        MMSceneLoadingManager.OnLoadingStarted += () => shouldAutoEquip = false;
        MMSceneLoadingManager.OnLoadingCompleted += () => shouldAutoEquip = true;
    }
    private void OnDisable()
    {
        if (!shouldAutoEquip) return;
        SkillManager.Instance.ScheduleInventoryClose(0.1f, gameObject.GetComponent<InventoryPickableItem>());
    }
}
