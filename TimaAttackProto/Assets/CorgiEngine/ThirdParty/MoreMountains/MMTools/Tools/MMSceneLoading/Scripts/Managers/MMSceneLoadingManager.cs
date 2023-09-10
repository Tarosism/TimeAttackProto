using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using System;


namespace MoreMountains.Tools
{
    /// <summary>
    /// A class to load scenes using a loading screen instead of just the default API
    /// This class used to be known as LoadingSceneManager, and has now been renamed to MMSceneLoadingManager for consistency
    /// </summary>
    public class MMSceneLoadingManager : MonoBehaviour
    {
        public enum LoadingStatus
        {
            LoadStarted, BeforeEntryFade, EntryFade, AfterEntryFade, UnloadOriginScene, LoadDestinationScene,
            LoadProgressComplete, InterpolatedLoadProgressComplete, BeforeExitFade, ExitFade, DestinationSceneActivation, UnloadSceneLoader, LoadTransitionComplete
        }

        public struct LoadingSceneEvent
        {
            public LoadingStatus Status;
            public string SceneName;
            public LoadingSceneEvent(string sceneName, LoadingStatus status)
            {
                Status = status;
                SceneName = sceneName;
            }
            static LoadingSceneEvent e;
            public static void Trigger(string sceneName, LoadingStatus status)
            {
                e.Status = status;
                e.SceneName = sceneName;
                MMEventManager.TriggerEvent(e);
            }
        }
        //!이벤트를 위한
        public static event Action OnLoadingStarted;
        public static event Action OnLoadingCompleted;
        //!이벤트를 위한
        //?매 로딩마다 팁을 띄웁니다
        public List<string> DialogueScripts = new List<string>
{
    "팁 1: 장애물을 피하세요!",
    "팁 2: 보너스 아이템을 찾으세요!",
    "팁 3: 시간을 효율적으로 활용하세요!",
};
        //?매 로딩마다 팁을 띄웁니다

        [Header("Binding")]
        /// The name of the scene to load while the actual target scene is loading (usually a loading screen)
        public static string LoadingScreenSceneName = "LoadingScreen";

        [Header("GameObjects")]
        /// the text object where you want the loading message to be displayed
        public Text LoadingText;
        /// the canvas group containing the progress bar
        public CanvasGroup LoadingProgressBar;
        /// the canvas group containing the animation
        public CanvasGroup LoadingAnimation;
        /// the canvas group containing the animation to play when loading is complete
        public CanvasGroup LoadingCompleteAnimation;

        [Header("Time")]
        /// the duration (in seconds) of the initial fade in
        public float StartFadeDuration = 0.2f;
        /// the speed of the progress bar
        public float ProgressBarSpeed = 2f;
        /// the duration (in seconds) of the load complete fade out
        public float ExitFadeDuration = 0.2f;
        /// the delay (in seconds) before leaving the scene when complete
        public float LoadCompleteDelay = 0.5f;

        protected AsyncOperation _asyncOperation;
        protected static string _sceneToLoad = "";
        protected float _fadeDuration = 0.5f;
        protected float _fillTarget = 0f;
        protected string _loadingTextValue;
        protected Image _progressBarImage;

        protected static MMTweenType _tween;

        /// <summary>
        /// Call this static method to load a scene from anywhere
        /// </summary>
        /// <param name="sceneToLoad">Level name.</param>
        public static void LoadScene(string sceneToLoad)
        {
            _sceneToLoad = sceneToLoad;
            Application.backgroundLoadingPriority = ThreadPriority.High;
            if (LoadingScreenSceneName != null)
            {
                OnLoadingStarted?.Invoke();  //! 이 줄 추가
                LoadingSceneEvent.Trigger(sceneToLoad, LoadingStatus.LoadStarted);
                SceneManager.LoadScene(LoadingScreenSceneName);
            }
        }

        /// <summary>
        /// Call this static method to load a scene from anywhere
        /// </summary>
        /// <param name="sceneToLoad">Level name.</param>
        public static void LoadScene(string sceneToLoad, string loadingSceneName)
        {
            _sceneToLoad = sceneToLoad;
            Application.backgroundLoadingPriority = ThreadPriority.High;
            SceneManager.LoadScene(loadingSceneName);
            OnLoadingStarted?.Invoke();  //! 이 줄 추가

        }

        /// <summary>
        /// On Start(), we start loading the new level asynchronously
        /// </summary>
        protected virtual void Start()
        {
            _tween = new MMTweenType(MMTween.MMTweenCurve.EaseOutCubic);
            _progressBarImage = LoadingProgressBar.GetComponent<Image>();

            //!랜덤 다이스 후 띄우기
            System.Random rand = new System.Random();
            int index = rand.Next(DialogueScripts.Count);
            LoadingText.text = DialogueScripts[index];
            //!랜덤 다이스 후 띄우기

            if (!string.IsNullOrEmpty(_sceneToLoad))
            {
                StartCoroutine(LoadAsynchronously());
            }
        }

        /// <summary>
        /// Every frame, we fill the bar smoothly according to loading progress
        /// </summary>
        protected virtual void Update()
        {
            Time.timeScale = 1f;
            _progressBarImage.fillAmount = MMMaths.Approach(_progressBarImage.fillAmount, _fillTarget, Time.deltaTime * ProgressBarSpeed);
        }

        /// <summary>
        /// Loads the scene to load asynchronously.
        /// </summary>
        protected virtual IEnumerator LoadAsynchronously()
        {
            // we setup our various visual elements
            LoadingSetup();

            // we fade from black
            MMFadeOutEvent.Trigger(StartFadeDuration, _tween);
            yield return new WaitForSeconds(StartFadeDuration);

            // we start loading the scene
            _asyncOperation = SceneManager.LoadSceneAsync(_sceneToLoad, LoadSceneMode.Single);
            _asyncOperation.allowSceneActivation = false;
            // while the scene loads, we assign its progress to a target that we'll use to fill the progress bar smoothly
            while (_asyncOperation.progress < 0.9f)
            {
                _fillTarget = _asyncOperation.progress;
                yield return null;
            }
            // when the load is close to the end (it'll never reach it), we set it to 100%
            _fillTarget = 1f;

            // we wait for the bar to be visually filled to continue
            while (_progressBarImage.fillAmount != _fillTarget)
            {
                yield return null;
            }

            // the load is now complete, we replace the bar with the complete animation
            LoadingComplete();
            yield return new WaitForSeconds(LoadCompleteDelay);

            // we fade to black
            MMFadeInEvent.Trigger(ExitFadeDuration, _tween);
            yield return new WaitForSeconds(ExitFadeDuration);

            // we switch to the new scene
            _asyncOperation.allowSceneActivation = true;
            LoadingSceneEvent.Trigger(_sceneToLoad, LoadingStatus.LoadTransitionComplete);
            OnLoadingCompleted?.Invoke();
        }

        /// <summary>
        /// Sets up all visual elements, fades from black at the start
        /// </summary>
        protected virtual void LoadingSetup()
        {
            LoadingCompleteAnimation.alpha = 0;
            _progressBarImage.fillAmount = 0f;
            LoadingText.text = _loadingTextValue;
        }

        /// <summary>
        /// Triggered when the actual loading is done, replaces the progress bar with the complete animation
        /// </summary>
        protected virtual void LoadingComplete()
        {
            LoadingSceneEvent.Trigger(_sceneToLoad, LoadingStatus.InterpolatedLoadProgressComplete);
            LoadingCompleteAnimation.gameObject.SetActive(true);
            StartCoroutine(MMFade.FadeCanvasGroup(LoadingProgressBar, 0.1f, 0f));
            StartCoroutine(MMFade.FadeCanvasGroup(LoadingAnimation, 0.1f, 0f));
            StartCoroutine(MMFade.FadeCanvasGroup(LoadingCompleteAnimation, 0.1f, 1f));

        }
    }
}