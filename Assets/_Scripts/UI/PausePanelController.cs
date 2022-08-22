using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ARLuft.UI
{
    public class PausePanelController : MonoBehaviour
    {
        [Header("Floating panel")]
        [SerializeField] private GameObject floatingPanel;
        [SerializeField] private TextMeshProUGUI panelTitle;

        [Header("Titles")]
        [SerializeField] private string titleTextPause = "Pausiert";
        [SerializeField] private string titleTextWin = "Gut gemacht!";

        [Header("Buttons")]
        [SerializeField] private Button backButtonPause;
        [SerializeField] private Button restartButtonPause;
        [SerializeField] private Button alternateLeftButton;

        [Header("Images")]
        [SerializeField] private Sprite playImage;
        [SerializeField] private Sprite graphImage;

        private ARUIController mainController;
        private void Start()
        {
            mainController = ARUIController.Instance;

            //Buttons listeners
            backButtonPause.onClick.AddListener(mainController.GoToLastScene);
            restartButtonPause.onClick.AddListener(mainController.Restart);
            alternateLeftButton.onClick.AddListener(mainController.Play);
        }

        public void ShowMenu(bool enable)
        {
            if (enable)
            {
                floatingPanel.LeanMoveLocalY(0, 0.5f).setEaseOutExpo().delay = 0.1f;
            }
            else
                floatingPanel.LeanMoveLocalY(-Screen.height, 0.5f).setEaseInExpo();

            mainController.PauseGame(enable);
        }

        public void ChangeMenu(bool win)
        {
            //Change appearence
            panelTitle.text = win ? titleTextWin : titleTextPause;
            Sprite newSprite = win ? graphImage : playImage;
            alternateLeftButton.GetComponent<Image>().sprite = newSprite;

            //Change listener
            alternateLeftButton.onClick.RemoveAllListeners();
            alternateLeftButton.onClick.AddListener(
                win ? mainController.ActivateGraphUI : mainController.Play);

            //Restart button not needed in graph
            if (!win)
                ReorderButtonsForGraph();
        }

        private void ReorderButtonsForGraph()
        {
            //left button
            RectTransform rectTransform = backButtonPause.GetComponent<RectTransform>();
            Vector3 pos = rectTransform.position;
            pos.x += 60;
            rectTransform.position = pos;

            //middle button
            restartButtonPause.gameObject.SetActive(false);

            //right button
            rectTransform = alternateLeftButton.GetComponent<RectTransform>();
            pos = rectTransform.position;
            pos.x -= 60;
            rectTransform.position = pos;
        }
        private void OnDestroy()
        {
            backButtonPause.onClick.RemoveAllListeners();
            alternateLeftButton.onClick.RemoveAllListeners();
            restartButtonPause.onClick.RemoveAllListeners();
        }
    }
}
