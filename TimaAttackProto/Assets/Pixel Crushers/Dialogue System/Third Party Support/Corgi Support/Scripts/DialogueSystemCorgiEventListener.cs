using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace PixelCrushers.DialogueSystem.CorgiEngineSupport
{

    /// <summary>
    /// Adds option to integrate Dialogue System saving with Corgi SaveLoadManager.
    /// Also provides methods to pause and unpause Corgi.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Dialogue System/Third Party/Corgi/Dialogue System Corgi Event Listener")]
    public class DialogueSystemCorgiEventListener : MonoBehaviour, MMEventListener<MMGameEvent>, MMEventListener<CorgiEngineEvent>
    {

        [Tooltip("Save & load Dialogue System data when More Mountains SaveLoadManager requests. If using DialogueSystemInventoryEventListener, UNtick this on one or the other.")]
        public bool handleMMSaveLoadEvents = false;

        protected bool _prevSendNavEvents;

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
        /// <summary>
        /// On enable, we start listening for MMGameEvents.
        /// </summary>
        protected virtual void OnEnable()
        {
            MMEventManager.AddListener<CorgiEngineEvent>(this);
            if (handleMMSaveLoadEvents) this.MMEventStartListening<MMGameEvent>();
            SceneNotifier.willUnloadScene += OnWillUnloadScene;
        }

        /// <summary>
        /// On disable, we stop listening for MMGameEvents.
        /// </summary>
        protected virtual void OnDisable()
        {
            MMEventManager.RemoveListener<CorgiEngineEvent>(this);
            if (handleMMSaveLoadEvents) this.MMEventStopListening<MMGameEvent>();
            SceneNotifier.willUnloadScene -= OnWillUnloadScene;
        }

        private void OnWillUnloadScene(int sceneIndex)
        {
            if (FindObjectOfType<MoreMountains.CorgiEngine.LevelManager>() != null)
            {
                CorgiEngineEvent.Trigger(CorgiEngineEventTypes.LevelEnd);
                MMGameEvent.Trigger("Save");
                CorgiEngineEvent.Trigger(CorgiEngineEventTypes.UnPause);
            }
        }

        public virtual void OnMMEvent(CorgiEngineEvent engineEvent)
        {
            switch (engineEvent.EventType)
            {
                case CorgiEngineEventTypes.Pause:
                    UIPanel.monitorSelection = false;
                    break;

                case CorgiEngineEventTypes.UnPause:
                    StartCoroutine(Unpause(false));
                    break;
            }
        }

        protected IEnumerator Unpause(bool reenableCorgiComponents)
        {
            yield return null;
            UIPanel.monitorSelection = true;
            EventSystem.current.sendNavigationEvents = true;
            if (reenableCorgiComponents) SetCorgiComponents(true);
        }

        /// <summary>
        /// Pause Corgi Engine, for example to show a quest log window.
        /// </summary>
        /// <param name="allowAutoFocus"></param>
        public void PauseCorgi(bool allowAutoFocus)
        {
            GameManager.Instance.Pause();
            SetCorgiComponents(false);
            _prevSendNavEvents = EventSystem.current.sendNavigationEvents;
            EventSystem.current.sendNavigationEvents = true;
            if (allowAutoFocus) UIPanel.monitorSelection = true;
        }

        /// <summary>
        /// Unpause Corgi Engine, for example when closing the quest log window.
        /// </summary>
        public void UnpauseCorgi()
        {
            GameManager.Instance.UnPause();
            EventSystem.current.sendNavigationEvents = _prevSendNavEvents;
            StartCoroutine(Unpause(true));
        }

        protected virtual void SetCorgiComponents(bool value)
        {
            if (value == true)
            {
                MoreMountains.CorgiEngine.LevelManager.Instance.UnFreezeCharacters();
            }
            else
            {
                MoreMountains.CorgiEngine.LevelManager.Instance.FreezeCharacters();
            }
            if (cameraController != null) cameraController.enabled = value;
            if (inventoryInputManager != null) inventoryInputManager.enabled = value;
        }

        public virtual void OnMMEvent(MMGameEvent gameEvent)
        {
            if (gameEvent.EventName == "Save")
            {
                SaveDialogueSystem();
            }
            if (gameEvent.EventName == "Load")
            {
                LoadDialogueSystem();
            }
        }

        protected const string _saveFolderName = "DialogueSystem/";
        protected const string _saveFileExtension = ".data";

        /// <summary>
        /// Saves the Dialogue System's state to Corgi's MMSaveLoadManager.
        /// </summary>
        public void SaveDialogueSystem()
        {
            var data = SaveSystem.hasInstance ? SaveSystem.Serialize(SaveSystem.RecordSavedGameData())
                : PersistentDataManager.GetSaveData();
            MMSaveLoadManager.Save(data, gameObject.name + _saveFileExtension, _saveFolderName);

        }

        /// <summary>
        /// Loads the Dialogue System's state from data previously saved in Corgi's MMSaveLoadManager.
        /// </summary>
        public void LoadDialogueSystem()
        {
            string data = (string)MMSaveLoadManager.Load(typeof(string), gameObject.name + _saveFileExtension, _saveFolderName);
            if (SaveSystem.hasInstance)
            {
                var savedGameData = SaveSystem.Deserialize<SavedGameData>(data);
                if (savedGameData != null) SaveSystem.ApplySavedGameData(savedGameData);
            }
            else
            {
                PersistentDataManager.ApplySaveData(data);
            }
        }

    }
}
