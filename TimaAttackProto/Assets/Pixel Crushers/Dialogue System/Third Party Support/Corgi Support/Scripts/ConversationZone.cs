using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelCrushers.DialogueSystem.CorgiEngineSupport
{

    /// <summary>
    /// Add this class to an empty GameObject. It will automatically add a boxcollider2d, set it to "is trigger". 
    /// Then customize the conversation zone through the inspector to run a Dialogue System conversation.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Dialogue System/Third Party/Corgi/Conversation Zone")]
    public class ConversationZone : ButtonActivated
    {

        [Header("Dialogue System")]

        [Tooltip("Player is the actor in this conversation.")]
        public bool playerIsActor = true;

        [Tooltip("Player can move during this conversation.")]
        public bool canMoveWhileTalking = false;

        [Tooltip("If player CANNOT move during conversation, also disable HorizontalMovement component.")]
        public bool alsoDisableMovementComponentWhileTalking = true;

        [Tooltip("Don't freeze player until it it's grounded.")]
        public bool landOnGroundWhileTalking = true;

        [Tooltip("Corgi retains camera control during this conversation.")]
        public bool corgiControlsCameraWhileTalking = false;

        [Tooltip("Disable the inventory UI toggle button during this conversation (if using Inventory Engine).")]
        public bool disableInventoryWhileTalking = true;

        [Tooltip("Pause game while conversation is active.")]
        public bool pauseWhileTalking = false;

        public enum FollowTarget { Nothing, Actor, Conversant }

        [Tooltip("Make the dialogue UI follow a target.")]
        public FollowTarget dialogueUIFollows = FollowTarget.Nothing;

        [Tooltip("If following Actor or Conversant, offset dialogue UI by this distance.")]
        public Vector3 followOffset = Vector3.zero;

        [Tooltip("If Hide Prompt After Use is ticked, allow prompt to appear when re-entering zone.")]
        public bool hidePromptOnlyDuringConversation = true;

        [Header("Save & Load")]

        [Tooltip("Use this dialogue database variable to record the number of activations left.")]
        [VariablePopup]
        public string numActivationsLeftVariable = string.Empty;

        protected ConversationTrigger _conversationTrigger;
        protected ConversationTrigger conversationTrigger
        {
            get
            {
                if (_conversationTrigger == null) _conversationTrigger = GetComponent<ConversationTrigger>();
                return _conversationTrigger;
            }
        }

        protected List<DialogueSystemTrigger> _dialogueSystemTriggers = null;
        protected List<DialogueSystemTrigger> dialogueSystemTriggers
        {
            get
            {
                if (_dialogueSystemTriggers == null) _dialogueSystemTriggers = new List<DialogueSystemTrigger>(GetComponents<DialogueSystemTrigger>());
                return _dialogueSystemTriggers;
            }
        }
        
        protected GameObject _player = null;
        protected GameObject player
        {
            get
            {
                if (_player == null) _player = GameObject.FindGameObjectWithTag("Player");
                return _player;
            }
        }

        protected DialogueActor _dialogueActor = null;
        protected DialogueActor dialogueActor
        {
            get
            {
                if (_dialogueActor == null) _dialogueActor = GetComponent<DialogueActor>();
                return _dialogueActor;
            }
        }

        protected CameraController _cameraController = null;
        protected CameraController cameraController
        {
            get
            {
                if (_cameraController == null) _cameraController = FindObjectOfType<CameraController>();
                return _cameraController;
            }
        }

        protected MoreMountains.InventoryEngine.InventoryInputManager _inventoryInputManager = null;
        protected MoreMountains.InventoryEngine.InventoryInputManager inventoryInputManager
        {
            get
            {
                if (_inventoryInputManager == null) _inventoryInputManager = FindObjectOfType<MoreMountains.InventoryEngine.InventoryInputManager>();
                return _inventoryInputManager;
            }
        }

        protected Transform _followTarget = null;
        protected Transform _ui = null;
        protected bool _isInConversation = false;
        protected float _timeLastConversationEnd = -1;
        protected bool _prevSendNavEvents = false;

        public virtual void Awake()
        {
            if ((dialogueSystemTriggers == null || dialogueSystemTriggers.Count == 0) && 
                conversationTrigger == null)
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: " + name + " is missing a Dialogue System Trigger or Conversation Trigger set to OnUse.", this);
                enabled = false;
            }
        }

        protected override bool CheckConditions(GameObject collider)
        {
            return base.CheckConditions(collider) && !_isInConversation &&
                (!DialogueManager.IsConversationActive || DialogueManager.AllowSimultaneousConversations);
        }

        /// <summary>
        /// When the zone is activated we start the conversation.
        /// </summary>
        protected override void ActivateZone()
        {
            if (_isInConversation) return;
            if (Time.time == _timeLastConversationEnd) return;
            base.ActivateZone();
            if (dialogueSystemTriggers == null && conversationTrigger == null) return;
            var isDialogueSystemTriggerEnabled = dialogueSystemTriggers.Find(x => x.enabled) != null;
            var isConversationTriggerEnabled = (conversationTrigger != null && conversationTrigger.enabled);
            if (!(isDialogueSystemTriggerEnabled || isConversationTriggerEnabled)) return;
            if (playerIsActor)
            {
                if (player == null)
                {
                    if (DialogueDebug.logWarnings) Debug.LogWarning($"Dialogue System: Conversation Zone's Player Is Actor checkbox is ticked, but no GameObject in the scene is tagged 'Player'. Not starting conversation.", this);
                    return;
                }
                if (isDialogueSystemTriggerEnabled)
                {
                    dialogueSystemTriggers.ForEach(x => x.conversationActor = player.transform);
                }
                else
                {
                    conversationTrigger.actor = player.transform;
                }
            }
            if (isDialogueSystemTriggerEnabled)
            {
                dialogueSystemTriggers.ForEach(x => { if (x.enabled) x.OnUse(); });
            }
            else
            {
                conversationTrigger.OnUse();
            }
            if (hidePromptOnlyDuringConversation) _promptHiddenForever = false;
        }

        protected virtual Transform GetConversant()
        {
            foreach (var dst in dialogueSystemTriggers)
            {
                if (dst != null) return dst.conversationConversant;
            }
            if (conversationTrigger != null) return conversationTrigger.conversant;
            return null;
        }

        public virtual void OnConversationStart(Transform actor)
        {
            _isInConversation = true;
            SetCorgiComponents(false);
            _prevSendNavEvents = EventSystem.current.sendNavigationEvents;
            EventSystem.current.sendNavigationEvents = true;
            _followTarget = (dialogueUIFollows == FollowTarget.Actor) 
                ? actor 
                : (dialogueUIFollows == FollowTarget.Conversant) 
                    ? GetConversant()
                    : null;
            if (_followTarget)
            {
                if (dialogueActor != null && 
                    dialogueActor.standardDialogueUISettings.subtitlePanelNumber == SubtitlePanelNumber.Custom &&
                    dialogueActor.standardDialogueUISettings.customSubtitlePanel != null)
                {
                    _ui = dialogueActor.standardDialogueUISettings.customSubtitlePanel.transform;
                }
                else
                {
                    var standardUISubtitlePanel = GetComponentInChildren<StandardUISubtitlePanel>();
                    if (standardUISubtitlePanel != null)
                    {
                        _ui = standardUISubtitlePanel.transform;
                    }
                    else
                    {
                        var controls = GetComponentInChildren<OverrideUnityUIDialogueControls>();
                        _ui = (controls != null) ? controls.transform : null;
                    }
                }
            }
            if (pauseWhileTalking) GameManager.Instance.Pause();
        }

        public virtual void OnConversationEnd(Transform actor)
        {
            if (pauseWhileTalking) GameManager.Instance.UnPause();
            EventSystem.current.sendNavigationEvents = _prevSendNavEvents;
            _timeLastConversationEnd = Time.time;
            _isInConversation = false;
            _followTarget = null;
            StartCoroutine(EnableCorgiComponentsAfterOneFrame());
            if (hidePromptOnlyDuringConversation) ShowPrompt();
        }

        // Wait one frame so Corgi components don't interpret current frame's conversation input as its own move/jump input.
        protected IEnumerator EnableCorgiComponentsAfterOneFrame()
        {
            yield return null;
            SetCorgiComponents(true);
        }

        protected bool _wasMovementAllowed;

        protected virtual void SetCorgiComponents(bool value)
        {
            if (!canMoveWhileTalking)
            {
                if (value == false)
                {
                    DisallowMovement();
                }
                else
                {
                    UndoDisallowMovement();
                }
            }
            if (!corgiControlsCameraWhileTalking)
            {
                if (cameraController != null) cameraController.enabled = value;
            }
            if (disableInventoryWhileTalking)
            {
                EventSystem.current.sendNavigationEvents = !value;
                if (inventoryInputManager != null) inventoryInputManager.enabled = value;
            }
        }

        public virtual void DisallowMovement()
        {
            if (player == null) return;
            var controller = player.GetComponent<CorgiController>();
            if (controller == null || controller.State.IsGrounded || !landOnGroundWhileTalking)
            {
                MoreMountains.CorgiEngine.LevelManager.Instance.FreezeCharacters();
            }
            else
            {
                StartCoroutine(FreezeWhenGrounded(controller));
            }
            var horizMovement = player.GetComponent<CharacterHorizontalMovement>();
            _wasMovementAllowed = (horizMovement != null && horizMovement.enabled && !horizMovement.MovementForbidden);
            if (_wasMovementAllowed)
            {
                horizMovement.MovementForbidden = true;
                //--- Wait 1 frame to stop feedbacks.
                //---if (alsoDisableMovementComponentWhileTalking) horizMovement.enabled = false;
                if (alsoDisableMovementComponentWhileTalking) StartCoroutine(DisableComponentAfterFrame(horizMovement));
            }
            if (ShouldUpdateState)
            {
                var character = (_characterButtonActivation != null) ? _characterButtonActivation.GetComponent<Character>()
                    : (DialogueManager.currentActor != null) ? DialogueManager.currentActor.GetComponent<Character>() : null;
                if (character != null && character.MovementState != null)
                {
                    character.MovementState.ChangeState(CharacterStates.MovementStates.Idle);
                }
            }
        }

        protected virtual IEnumerator FreezeWhenGrounded(CorgiController controller)
        {
            while (!controller.State.IsGrounded)
            {
                yield return null;
            }
            MoreMountains.CorgiEngine.LevelManager.Instance.FreezeCharacters();
        }

        protected virtual IEnumerator DisableComponentAfterFrame(MonoBehaviour monoBehaviour)
        {
            yield return null;
            if (monoBehaviour != null) monoBehaviour.enabled = false;
        }

        public virtual void UndoDisallowMovement()
        {
            if (player == null) return;
            MoreMountains.CorgiEngine.LevelManager.Instance.UnFreezeCharacters();
            if (_wasMovementAllowed)
            {
                var horizMovement = player.GetComponent<CharacterHorizontalMovement>();
                if (alsoDisableMovementComponentWhileTalking) horizMovement.enabled = true;
                horizMovement.MovementForbidden = false;
            }
        }

        protected virtual void Update()
        {
            if (_followTarget == null || _ui == null) return;
            _ui.position = _followTarget.position + followOffset;
        }

        //// Pre-Corgi 6.6 version:
        //protected override void Update()
        //{
        //    base.Update();
        //    if (_followTarget == null || _ui == null) return;
        //    _ui.position = _followTarget.position + followOffset;
        //}

        protected override void OnEnable()
        {
            base.OnEnable();
            PersistentDataManager.RegisterPersistentData(this.gameObject);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PersistentDataManager.UnregisterPersistentData(this.gameObject);
        }

        protected override void TriggerExit(GameObject collider)
        {
            base.TriggerExit(collider);
            CharacterButtonActivation characterButtonActivation = collider.gameObject.MMGetComponentNoAlloc<CharacterButtonActivation>();
            if (characterButtonActivation != null && characterButtonActivation.InButtonActivatedZone && characterButtonActivation.ButtonActivatedZone == this)
            {
                characterButtonActivation.InButtonActivatedZone = false;
                characterButtonActivation.ButtonActivatedZone = null;
            }
        }

        protected virtual void OnRecordPersistentData()
        {
            if (string.IsNullOrEmpty(numActivationsLeftVariable)) return;
            DialogueLua.SetVariable(numActivationsLeftVariable, _numberOfActivationsLeft);
        }

        protected virtual void OnApplyPersistentData()
        {
            if (string.IsNullOrEmpty(numActivationsLeftVariable)) return;
            if (!DialogueLua.DoesVariableExist(numActivationsLeftVariable)) return;
            _numberOfActivationsLeft = DialogueLua.GetVariable(numActivationsLeftVariable).AsInt;
        }

    }
}