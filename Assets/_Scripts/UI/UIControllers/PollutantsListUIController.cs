using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ARLuft.Data;
using TMPro;

namespace ARLuft.UI
{
    public class PollutantsListUIController : ListUIController
    {
        [Header("Pollutants list UI")]
        [SerializeField] private TextMeshProUGUI _stationTitle;
        [SerializeField] private Button _backButton;
        [SerializeField] private GameObject _noDataWarning;


        private PollutantData[] _pollData;
        protected override void Start()
        {
            base.Start();
            _noDataWarning.SetActive(false);
        }
        private void OnEnable()
        {
            DataManager.Instance.PollutionDataAvailable.AddListener(CreatePollutantsList);
            DataManager.Instance.NoPollutionDataAvailable.AddListener(ShowNoDataFoundWarning);
            _backButton.onClick.AddListener(GoBack);
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveAllListeners();
        }

        public void ShowPollutantsListUI(string stationCode)
        {
            DestroyOldListEntries();
            _stationTitle.text = DataManager.Instance.GetStationName(stationCode);
            listViewUI.LeanMoveLocalX(0, 0.5f);
        }
        public void SwitchToARParticlesScene(PollutantData pollutant)
        {
            DataManager.Instance.SetPollutantForAR(pollutant);
            GameManager.Instance.SwitchScene(GameManager.Instance.ARSceneBuildIndex);
        }

        //UI intern methods
        private void GoBack()
        {
            _noDataWarning.SetActive(false);
            listViewUI.LeanMoveLocalX(Screen.width, 0.5f);
        }
        private void CreatePollutantsList()
        {
            _noDataWarning.SetActive(false);
            _pollData = DataManager.Instance.RootPollution.data;

            Vector2 tempPos = new(0f, _templateListEntry.rect.position.y);
            string firstCore = _pollData[^1].core;

            //List entries creation
            for (int i = _pollData.Length - 1; i > 0; i--)
            {
                if (i != _pollData.Length - 1 && _pollData[i].core == firstCore)
                    break;

                //Show only when data available
                if (_pollData[i].value == 0)
                    continue;

                //Instantiation in position
                GameObject go = CreateListEntry(tempPos);
                tempPos.y -= 80f;

                //Fill list entry with data
                PollutantListEntry listEntry = go.GetComponent<PollutantListEntry>();
                listEntry.SetDataInUI(_pollData[i]);
            }

            _templateListEntry.gameObject.SetActive(false);
            _pollData = null;
        }
        private void ShowNoDataFoundWarning()
        {
            _templateListEntry.gameObject.SetActive(false);
            _noDataWarning.SetActive(true);
        }

    }
}

