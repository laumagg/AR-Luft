using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ARLuft.UI
{
    public class ARUITutorials : MonoBehaviour
    {
        [SerializeField] private Button startARButton;
        [SerializeField] private GameObject particlesTutorial;
        [SerializeField] private GameObject graphTutorial;


        [HideInInspector]
        public delegate void TutorialState(bool open);
        [HideInInspector]
        public event TutorialState OnTutorialStateChange;

        [HideInInspector] public bool TutorialOpen;

        private void Start()
        {
            particlesTutorial.SetActive(true);
            graphTutorial.SetActive(false);
            startARButton.onClick.AddListener(StartAR);
            ShowTutorial(true);
        }

        private void StartAR()
        {
            ShowTutorial(false);

            if (GameManager.Instance.MiniGameState == 0)
                ARUIController.Instance.StartParticlesGame();
            else
                ARUIController.Instance.StartGraph();
        }

        public void ShowTutorial(bool enable)
        {
            TutorialOpen = enable;

            if (enable)
                gameObject.LeanMoveLocalY(0, 0.5f).setEaseOutExpo().delay = 0.1f;
            else
                gameObject.LeanMoveLocalY(-Screen.height, 0.5f).setEaseInExpo();

            OnTutorialStateChange?.Invoke(enable);
        }
        public void ChangeToGraphTutorial()
        {
            graphTutorial.SetActive(true);
            particlesTutorial.SetActive(false);

            ShowTutorial(true);
        }

        private void OnDestroy()
        {
            startARButton.onClick.RemoveAllListeners();
        }
    }
}
