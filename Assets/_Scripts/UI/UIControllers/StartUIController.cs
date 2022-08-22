using ARLuft.Data;
using ARLuft.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ARLuft.UI
{
    public class StartUIController : MonoBehaviour
    {
        [Header("Start information overlay")]
        [SerializeField] private Transform startInfoUI;
        [SerializeField] private RectTransform backgroundOverlay;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button urlButton;

        [Header("Navigation Buttons")]
        [SerializeField] private Button listViewButton;
        [SerializeField] private Button informationButton;

        [Header("Map view")]
        [SerializeField] private Transform mapUI;
        [SerializeField] private Animator animator;
        [SerializeField] private RectTransform attributionLogo;

        [Header("Station information overlay")]
        [SerializeField] private Button stationInfoButton;
        [SerializeField] private StationListEntry listEntry;


        [HideInInspector] public string selectedStationName;

        private void Start()
        {
            //Start Information overlay
            closeButton.onClick.AddListener(() => ShowHideInformationOverlay(false));
            urlButton.onClick.AddListener(OpenBlumeWebsite);

            //Map view
            informationButton.onClick.AddListener(() => ShowHideInformationOverlay(true));
            stationInfoButton.onClick.AddListener(ShowHideStationInfo);

            //List view
            listViewButton.onClick.AddListener(() => SwitchStationView(true));
        }
        private void OpenBlumeWebsite()
        {
            Application.OpenURL("https://luftdaten.berlin.de/lqi");
        }
        private void ShowHideInformationOverlay(bool enableInfo)
        {
            //Start info panel
            if (enableInfo)
            {
                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Open")
                    ShowHideStationInfo();

                startInfoUI.LeanMoveLocalY(0, 0.5f).setEaseOutExpo().delay = 0.1f;
            }
            else
                startInfoUI.LeanMoveLocalY(-Screen.height, 0.5f).setEaseInExpo();

            GameManager.Instance.FadeInOut(enableInfo, backgroundOverlay, 0.5f);

            //Disable map view features
            informationButton.interactable = !enableInfo;
            listViewButton.interactable = !enableInfo;
            stationInfoButton.enabled = !enableInfo;
        }
        private void ShowHideStationInfo()
        {
            bool show = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Open";
            string state = show ? "Close" : "Open";

            animator.SetTrigger(state);
            GameManager.Instance.FadeInOut(show, attributionLogo);
        }
        public void SwitchStationView(bool enableList)
        {
            if (enableList)
            {
                UIController.Instance.StationListUIController.listViewUI.LeanMoveLocalX(0, 0.5f)
                    .setOnComplete(() =>
                    {
                        gameObject.SetActive(false);
                        attributionLogo.transform.parent.gameObject.SetActive(false);
                    });

                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Open")
                    ShowHideStationInfo();
            }
            else
            {
                gameObject.SetActive(true);
                attributionLogo.transform.parent.gameObject.SetActive(true);
                UIController.Instance.StationListUIController.listViewUI.LeanMoveLocalX(Screen.width, 0.5f);
            }
        }

        public void ShowStationData(StationsData data)
        {
            listEntry.SetDataInUI(data);
            selectedStationName = data.name;
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Closed")
                ShowHideStationInfo();
        }

        private void OnDestroy()
        {
            //Start info
            closeButton.onClick.RemoveAllListeners();
            urlButton.onClick.RemoveAllListeners();

            //Start UI
            informationButton.onClick.RemoveAllListeners();
            stationInfoButton.onClick.RemoveAllListeners();
            listViewButton.onClick.RemoveAllListeners();
        }
    }
}
