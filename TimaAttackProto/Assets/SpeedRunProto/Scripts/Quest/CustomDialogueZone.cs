using UnityEngine;
using System.Collections;

namespace MoreMountains.CorgiEngine
{
    public class CustomDialogueZone : DialogueZone
    {
        public int questIndexToStart; // 시작할 퀘스트의 인덱스

        // 대화가 끝나면 퀘스트를 시작합니다.
        protected override IEnumerator PlayNextDialogue()
        {
            // 부모 클래스의 로직을 실행합니다.
            yield return base.PlayNextDialogue();

            // 대화가 끝났는지 확인합니다.
            if (_currentIndex >= Dialogue.Length)
            {
                // 퀘스트를 시작합니다.
                QuestManager.Instance.StartQuest(questIndexToStart);
            }
        }
    }
}