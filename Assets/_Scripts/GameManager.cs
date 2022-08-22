using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Threading.Tasks;

namespace ARLuft
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private RectTransform _fadeCanvas;
        [SerializeField] private List<GameObject> inactiveMainSceneObjects = new();

        [HideInInspector]
        public UnityEvent OnSwitchToGraph;
        [HideInInspector]
        public int MainSceneBuildIndex = 0;
        [HideInInspector]
        public int ARSceneBuildIndex = 1;
        [HideInInspector]
        public int MiniGameState = 0;

        private readonly WaitForSeconds _wait = new(0.00001f);

        private void Start()
        {
            gameObject.transform.parent = null;
            FadeInOut(false, _fadeCanvas);
            DontDestroyOnLoad(gameObject);
        }
        private void OnEnable()
        {
            SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
            {
                SceneManager.SetActiveScene(scene);
            };
        }

        public void SwitchToGraph()
        {
            MiniGameState = 1;
            OnSwitchToGraph?.Invoke();
        }

        public async void SwitchScene(int scene)
        {
            FadeInOut(true, _fadeCanvas, 2, true);

            //Wait for transition effect
            await Task.Delay(1000);

            //activate objects in main scene
            foreach (GameObject go in inactiveMainSceneObjects)
                go.SetActive(scene == MainSceneBuildIndex);

            if (scene != MainSceneBuildIndex)
                SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            else
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                MiniGameState = 0;
            }
        }


        public void FadeInOut(bool fadeIn, RectTransform obj, float fadeInEndTransparency = 1f, bool fadeAlsoOut = false, int secondsInBetween = 1)
        {
            obj.LeanCancel();

            //Fade in or out
            if (fadeIn)
            {
                obj.gameObject.SetActive(true);
                obj.LeanAlpha(fadeInEndTransparency, 0.5f);
            }
            else
                obj.LeanAlpha(0, 0.5f).setOnComplete(() => obj.gameObject.SetActive(false));

            // If after fading in, fading out wanted, wait and fade out then
            if (fadeAlsoOut)
                obj.LeanAlpha(0, 0.5f).setDelay(secondsInBetween).setOnComplete(() => obj.gameObject.SetActive(false));
        }
    }
}
