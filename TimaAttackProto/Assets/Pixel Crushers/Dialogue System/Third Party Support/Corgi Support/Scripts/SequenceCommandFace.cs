using UnityEngine;
using MoreMountains.CorgiEngine;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Syntax: Face([target], [subject])
    /// 
    /// - target: GameObject, or speaker to listener. Default: listener.
    /// - subject: GameObject, or speaker or listener. Default: speaker.
    /// 
    /// Makes the subject face the target by flipping the Corgi character if necessary.
    /// If target and subject are both omitted, makes both speaker and listener face each other.
    /// </summary>
    public class SequencerCommandFace : SequencerCommand
    {

        public void Awake()
        {
            var both = (Parameters.Length == 0);
            var target = GetSubject(0, listener);
            var subject = GetSubject(1, speaker);
            var targetCharacter = (listener != null) ? GameObjectUtility.GetComponentAnywhere<Character>(target.gameObject) : null;
            var subjectCharacter = (subject != null) ? GameObjectUtility.GetComponentAnywhere<Character>(subject.gameObject) : null;
            if (both && subjectCharacter == null && targetCharacter != null)
            {
                subjectCharacter = targetCharacter;
                targetCharacter = null;
                var temp = target;
                target = subject;
                subject = temp;
            }
            if (target == null)
            { 
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Sequencer: Face(" + GetParameters() + "): Can't find target.");
            }
            else if (subjectCharacter == null)
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Sequencer: Face(" + GetParameters() + "): Can't find subject or Character on subject.");
            }
            else
            {
                if (DialogueDebug.logInfo) Debug.Log("Dialogue System: Sequencer: Face(" + target + ", " + subject + ")", subject);
                if (!IsFacing(target, subjectCharacter))
                {
                    subjectCharacter.Flip();
                }
                if (both && targetCharacter != null && !IsFacing(subject, targetCharacter))
                {
                    targetCharacter.Flip();
                }
            }
            Stop();
        }

        private bool IsFacing(Transform target, Character character)
        {
            return (character.IsFacingRight && character.transform.position.x < target.position.x) ||
                (!character.IsFacingRight && character.transform.position.x > target.position.x);
        }
    }
}
