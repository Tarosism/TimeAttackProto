using UnityEngine;
using MoreMountains.CorgiEngine;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Syntax: PermitAbility(ability, true|false, [subject])
    /// 
    /// Permits or disallows an ability on a character.
    /// </summary>
    public class SequencerCommandPermitAbility : SequencerCommand
    { 

        public void Awake()
        {
            var abilityName = GetParameter(0);
            var value = GetParameterAsBool(1);
            var subject = GetSubject(2, speaker);
            var character = (subject != null) ? subject.GetComponent<Character>() : null;
            var ability = (character != null) ? subject.GetComponent(abilityName) as CharacterAbility : null;
            if (character == null)
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Sequencer: PermitAbility(" + GetParameters() + "): Can't find subject or Character on subject.");
            }
            else if (ability == null)
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Sequencer: PermitAbility(" + GetParameters() + "): Can't find ability " + abilityName + " on " + subject + ".", subject);
            }
            else
            {
                if (DialogueDebug.logInfo) Debug.Log("Dialogue System: Sequencer: PermitAbility(" + ability + ", " + value + ", " + subject + ")", subject);
                ability.PermitAbility(value);
            }
            Stop();
        }
    }
}
