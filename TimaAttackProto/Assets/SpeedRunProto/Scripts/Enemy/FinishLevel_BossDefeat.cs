using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    public class FinishLevel_BossDefeat : Health
    {
        public string LevelName;
        public int BossName;

        // 게이트 게임 오브젝트에 대한 참조를 추가합니다.
        public GameObject gateGameObject;

        public override void Kill()
        {
            base.Kill();
            SpeedRunTimer.Instance.OnEventFinished(BossName);

            if (BossName != 1)
            {
                OpenNextGate();
            }
            else
            {
                SpeedRunTimer.Instance.isTimerActive = false;
                SkillManager.Instance.InitializeSkillButtons();
            }
        }

        protected void OpenNextGate()
        {
            // gateGameObject가 null이 아니라면, 게이트를 활성화합니다.
            if (gateGameObject != null)
            {
                gateGameObject.SetActive(true);
                FinishLevel finishLevel = gateGameObject.GetComponent<FinishLevel>();
                if (finishLevel != null)
                {
                    finishLevel.LevelName = LevelName;
                    //MMEventManager.TriggerEvent(new MMGameEvent("Save"));
                }
            }
            else
            {
                Debug.LogWarning("Gate GameObject is not assigned.");
            }
        }
    }
}
