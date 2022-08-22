using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using ARLuft.Data;
using System;
using System.Threading.Tasks;

namespace ARLuft.UI
{
    public class ARUIController : Singleton<ARUIController>
    {
        [Header("General references")]
        [SerializeField] private ARUITutorials tutorialsController;
        [SerializeField] private ParticlesManager particlesManager;
        [SerializeField] private GraphManager graphManager;
        [SerializeField] private SecurityZone securityZone;
        [SerializeField] private RectTransform backgroundOverlay;
        [SerializeField] private PausePanelController pauseMenu;

        [Header("AR UI")]
        [SerializeField] private Button pauseButtonAR;
        [SerializeField] private ParticlesCounter counter;


        [HideInInspector] public bool GamePaused = false;
        private void Start()
        {
            //Listeners AR UI
            pauseButtonAR.onClick.AddListener(() => pauseMenu.ShowMenu(true));
            tutorialsController.OnTutorialStateChange += PauseGame;

            //Security
            securityZone.OnOutOfSecurityZone.AddListener(() => PauseGame(true));
        }

        //General methods
        public void PauseGame(bool pause)
        {
            ShowBackgroundOverlay(pause);
            GamePaused = pause;
            if (pause)
                particlesManager.StopParticles();

            //Background UI
            if (!graphManager.GraphActive)
                counter.ShowCounter(!pause);

            pauseButtonAR.interactable = !pause;
        }
        public void GoToLastScene()
        {
            graphManager.ChangeState(false);
            GameManager.Instance.SwitchScene(0);
        }
        public void Play()
        {
            if (tutorialsController.TutorialOpen) return;

            if (GameManager.Instance.MiniGameState == 0)
                PlayParticlesGame();
            else
                PlayGraphGame();
        }
        public void Restart()
        {
            RestartParticlesGame();
        }
        private void ShowBackgroundOverlay(bool enable)
        {
            GameManager.Instance.FadeInOut(enable, backgroundOverlay, enable ? 0.5f : 0f);
        }


        //Particles game
        public void StartParticlesGame()
        {
            particlesManager.SpawnParticles();
        }
        private void RestartParticlesGame()
        {
            particlesManager.RestartParticles();
            counter.SetCounterValue();
            pauseMenu.ShowMenu(false);
        }
        private void PlayParticlesGame()
        {
            particlesManager.PlayParticles();
            pauseMenu.ShowMenu(false);
        }
        public void DecreaseCounter()
        {
            counter.DecreaseCounterByOne();
        }
        public void ShowWinUI()
        {
            pauseMenu.ChangeMenu(true);
            pauseMenu.ShowMenu(true);
        }

        //Graph game
        public void StartGraph()
        {
            graphManager.ShowGraph();
        }
        public async void ActivateGraphUI()
        {
            //Hide UI
            pauseMenu.ShowMenu(false);
            counter.ShowCounter(false);

            //Prepare for graph game
            particlesManager.gameObject.SetActive(false);
            GameManager.Instance.SwitchToGraph();
            tutorialsController.ChangeToGraphTutorial();


            await Task.Delay(500);
            pauseMenu.ChangeMenu(false);
        }
        private void PlayGraphGame()
        {
            pauseMenu.ShowMenu(false);
        }



        private void OnDestroy()
        {
            pauseButtonAR.onClick.RemoveAllListeners();
        }
    }
}
