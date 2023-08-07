using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;
namespace MoreMountains.CorgiEngine
{
    public class FinishLevel_BossDefeat : Health
    {
        public string LevelName;
        public int BossName;

        public GameObject gatePrefab;
        public GameObject currentGate;
        public Vector3 gatePosition;
        public LevelSelector levelSelector;

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
            if (gatePrefab != null)
            {
                currentGate = Instantiate(gatePrefab, gatePosition, Quaternion.identity);

                FinishLevel finishLevel = currentGate.GetComponent<FinishLevel>();
                if (finishLevel != null)
                {
                    finishLevel.LevelName = LevelName;
                    //MMEventManager.TriggerEvent(new MMGameEvent("Save"));
                }
            }
            else return;
        }
    }
}
