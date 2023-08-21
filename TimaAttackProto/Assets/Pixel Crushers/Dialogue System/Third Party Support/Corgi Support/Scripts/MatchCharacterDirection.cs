using MoreMountains.CorgiEngine;
using UnityEngine;

namespace PixelCrushers.DialogueSystem.CorgiEngineSupport
{

    /// <summary>
    /// Add to a UI child of a character (such as ConversationZone) to make it 
    /// match the character's direction regardless of whether the character flips
    /// using scale or rotation.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Dialogue System/Third Party/Corgi/Match Character Direction")]
    public class MatchCharacterDirection : MonoBehaviour
    {
        Character character;

        void Start()
        {
            character = GetComponentInParent<Character>();
        }

        void Update()
        {
            if (character.FlipModelOnDirectionChange)
            {
                transform.localScale = new Vector3(Mathf.Sign(character.transform.localScale.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (character.RotateModelOnDirectionChange)
            {
                transform.localRotation = Quaternion.Euler(transform.localRotation.x, character.transform.localRotation.eulerAngles.y, transform.localRotation.z);
            }
        }
    }
}