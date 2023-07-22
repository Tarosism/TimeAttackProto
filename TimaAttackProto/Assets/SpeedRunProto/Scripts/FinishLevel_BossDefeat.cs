using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;
namespace MoreMountains.CorgiEngine
{
    public class FinishLevel_BossDefeat : Health
    {
        public string LevelName;
        public string BossName;

        public GameObject gatePrefab;
        public GameObject currentGate;
        public Vector3 gatePosition;
        public LevelSelector levelSelector;

        public override void Kill()
        {
            base.Kill();
            SpeedRunTimer.Instance.OnEventFinished(BossName);

            if (BossName != "scene02")
            {
                OpenNextGate();
                SkillManager.Instance.InitializeSkillButtons();
            }
            else
            {
                levelSelector.RestartLevel();
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
